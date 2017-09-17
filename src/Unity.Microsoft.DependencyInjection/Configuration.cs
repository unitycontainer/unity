using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.Microsoft.DependencyInjection
{
    public static class Configuration
    {
        #region Public Members

        public static IServiceProvider CreateServiceProvider(this IServiceCollection serviceCollection)
        {
            var container = new UnityContainer();
            return container.CreateServiceProvider(serviceCollection);
        }

        public static IServiceProvider CreateServiceProvider(this IUnityContainer container, IServiceCollection services = null)
        {
            ServiceProvider provider = new ServiceProvider(container);

            if (null == services) return provider;

            var aggregateTypes = GetAggregateTypes(services);
            var registerInstance = RegisterInstance();
            var lifetime = GetLifetime();

            // Configure all registrations into Unity
            foreach (var serviceDescriptor in services)
            {
                RegisterType(container, lifetime, serviceDescriptor, aggregateTypes, registerInstance);
            }

            return provider;
        }

        #endregion


        #region Implementatin

        // TODO: Verify
        private static MethodInfo RegisterInstance()
        {
            var miRegisterInstanceOpen = 
                typeof (UnityContainerExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                                 .Single(mi => (mi.Name == "RegisterInstance") && mi.IsGenericMethod && (mi.GetParameters().Length == 4));
            return miRegisterInstanceOpen;
        }

        // TODO: Verify
        private static HashSet<Type> GetAggregateTypes(IServiceCollection services)
        {
            var aggregateTypes = new HashSet<Type> (
                services.GroupBy(serviceDescriptor => serviceDescriptor.ServiceType,
                                 serviceDescriptor => serviceDescriptor)
                        .Where(typeGrouping => typeGrouping.Count() > 1)
                        .Select(type => type.Key)
                );

            return aggregateTypes;
        }

        // TODO: Verify
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

        // TODO: Verify
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

        // TODO: Verify
        private static void RegisterImplementation(IUnityContainer _container, ServiceDescriptor serviceDescriptor,
            bool isAggregateType, Func<ServiceDescriptor, LifetimeManager> fetchLifetime)
        {
            if (isAggregateType)
            {
                _container.RegisterType(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType,
                    serviceDescriptor.ImplementationType.AssemblyQualifiedName, fetchLifetime(serviceDescriptor));
            }
            else
            {
                _container.RegisterType(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType,
                    fetchLifetime(serviceDescriptor));
            }
        }

        // TODO: Verify
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

            #endregion

        }

        // TODO: Verify
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