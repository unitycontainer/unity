using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.Mvc
{
    public static class Configuration
    {
        static IServiceProvider _originalServiceProvider;

        public static void ConfigureDefaultServiceProvider(IServiceProvider serviceProvider)
        {
            _originalServiceProvider = serviceProvider;
        }

        public static void Register(IServiceCollection services, IUnityContainer container)
        {
            container.RegisterType<IServiceScopeFactory, ServiceScopeFactory>();
            container.RegisterType<IServiceScope, ServiceScope>();
            container.RegisterType<IServiceProvider, ServiceProvider>();

            RegisterEnumerable(container);

            var aggregateTypes = GetAggregateTypes(services);

            var registerInstance = RegisterInstance();

            var lifetime = GetLifetime();

            // Configure all registrations into Unity
            foreach (var serviceDescriptor in services)
            {
                RegisterType(container, lifetime, serviceDescriptor, aggregateTypes, registerInstance);
            }
        }

        private static MethodInfo RegisterInstance()
        {
            var miRegisterInstanceOpen =
                typeof (UnityContainerExtensions).
                    GetMethods(BindingFlags.Static | BindingFlags.Public).
                    Single(mi => (mi.Name == "RegisterInstance") && mi.IsGenericMethod && (mi.GetParameters().Length == 4));
            return miRegisterInstanceOpen;
        }

        private static HashSet<Type> GetAggregateTypes(IServiceCollection services)
        {
            var aggregateTypes = new HashSet<Type>
                (
                services.
                    GroupBy
                    (
                        serviceDescriptor => serviceDescriptor.ServiceType,
                        serviceDescriptor => serviceDescriptor
                    ).
                    Where(typeGrouping => typeGrouping.Count() > 1).
                    Select(type => type.Key)
                );
            return aggregateTypes;
        }

        private static void RegisterEnumerable(IUnityContainer _container)
        {
            _container.RegisterType
                (
                    typeof (IEnumerable<>),
                    new InjectionFactory
                        (
                        (container, enumerableType, name) =>
                        {
                            Type type = enumerableType.GetGenericArguments().Single();

                            object[] allInstances = container.ResolveAll(type).Concat
                                (
                                    (_container.IsRegistered(type) ||
                                     (type.GetGenericArguments().Length > 0 &&
                                      _container.IsRegistered(type.GetGenericTypeDefinition())))
                                        ? new object[] {container.Resolve(type)}
                                        : new object[] {}
                                ).ToArray();

                            return
                                typeof (Enumerable).
                                    GetMethod("OfType", BindingFlags.Static | BindingFlags.Public).
                                    MakeGenericMethod(new Type[] {type}).
                                    Invoke(null, new object[] {allInstances});
                        }
                        )
                );
        }

        private static Func<ServiceDescriptor, LifetimeManager> GetLifetime()
        {
            Func<ServiceDescriptor, LifetimeManager> fetchLifetime = (serviceDescriptor) =>
            {
                switch (serviceDescriptor.Lifetime)
                {
                    case ServiceLifetime.Scoped:
                        return new HierarchicalLifetimeManager();
                    case ServiceLifetime.Singleton:
                        return new ContainerControlledLifetimeManager();
                    case ServiceLifetime.Transient:
                        return new TransientLifetimeManager();
                    default:
                        throw new NotImplementedException(string.Format("Unsupported lifetime manager type '{0}'",
                            serviceDescriptor.Lifetime));
                }
            };
            return fetchLifetime;
        }

        private static void RegisterType(IUnityContainer _container, Func<ServiceDescriptor, LifetimeManager> fetchLifetime, ServiceDescriptor serviceDescriptor,
            ICollection<Type> aggregateTypes, MethodInfo miRegisterInstanceOpen)
        {
            var isAggregateType = aggregateTypes.Contains(serviceDescriptor.ServiceType);

            if (serviceDescriptor.ImplementationType != null)
            {
                RegisterImplementation(_container, serviceDescriptor, isAggregateType, fetchLifetime);
            }
            else if (serviceDescriptor.ImplementationFactory != null)
            {
                RegisterFactory(_container, serviceDescriptor, isAggregateType, fetchLifetime(serviceDescriptor));
            }
            else if (serviceDescriptor.ImplementationInstance != null)
            {
                RegisterSingleton(_container, serviceDescriptor, miRegisterInstanceOpen, isAggregateType, fetchLifetime(serviceDescriptor));
            }
            else
            {
                throw new InvalidOperationException("Unsupported registration type");
            }
        }

        private static void RegisterImplementation(IUnityContainer _container, ServiceDescriptor serviceDescriptor,
            bool isAggregateType, Func<ServiceDescriptor, LifetimeManager> fetchLifetime)
        {
            if (isAggregateType)
            {
                _container.RegisterType(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType,
                    serviceDescriptor.ImplementationType.AssemblyQualifiedName, fetchLifetime(serviceDescriptor));
            }

            _container.RegisterType(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType,
                fetchLifetime(serviceDescriptor));
        }

        private static void RegisterFactory(IUnityContainer _container, ServiceDescriptor serviceDescriptor,
            bool isAggregateType, LifetimeManager lifetimeManager)
        {
            if (isAggregateType)
            {
                _container.RegisterType
                    (
                        serviceDescriptor.ServiceType,
                        serviceDescriptor.ImplementationType.AssemblyQualifiedName,
                        lifetimeManager,
                        new InjectionFactory
                            (
                            container =>
                            {
                                var serviceProvider = container.Resolve<IServiceProvider>();
                                var instance = serviceDescriptor.ImplementationFactory(serviceProvider);
                                return instance;
                            }
                            )
                    );
            }
            else
            {
                _container.RegisterType
                    (
                        serviceDescriptor.ServiceType,
                        lifetimeManager,
                        new InjectionFactory
                            (
                            container =>
                            {
                                var serviceProvider = container.Resolve<IServiceProvider>();
                                var instance = serviceDescriptor.ImplementationFactory(serviceProvider);
                                return instance;
                            }
                            )
                    );
            }
        }

        private static void RegisterSingleton(IUnityContainer _container, ServiceDescriptor serviceDescriptor,
            MethodInfo miRegisterInstanceOpen, bool isAggregateType, LifetimeManager lifetimeManager)
        {
            if (isAggregateType)
            {
                //todo: sometimes ImplementationType is not defined
                Type implementationType = typeof(string);
                if (serviceDescriptor.ImplementationType != null)
                {
                    implementationType = serviceDescriptor.ImplementationType;
                }
                else if (serviceDescriptor.ImplementationInstance != null)
                {
                    implementationType = serviceDescriptor.ImplementationInstance.GetType();
                }

                miRegisterInstanceOpen.
                    MakeGenericMethod(new Type[] {serviceDescriptor.ServiceType}).
                    Invoke(null,
                        new object[]
                        {
                            _container, implementationType.AssemblyQualifiedName,
                            serviceDescriptor.ImplementationInstance, lifetimeManager
                        });
            }
            else
            {
                _container.RegisterInstance(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationInstance,
                    lifetimeManager);
            }
        }
    }
}