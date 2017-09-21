using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Unity.Microsoft.DependencyInjection
{
    internal class ServiceProvider : IServiceProvider, IServiceScopeFactory, IDisposable
    {
        #region Fields

        protected IUnityContainer container;
        private static Type enumerableType = typeof(IEnumerable<>);

        #endregion


        #region Constructors

        public ServiceProvider(IUnityContainer unity)
        {
            container = unity;
            container.RegisterInstance<IServiceProvider>(this, new ExternallyControlledLifetimeManager());
            container.RegisterInstance<IServiceScopeFactory>(this, new ExternallyControlledLifetimeManager());
            container.RegisterType<IServiceScope>(new InjectionFactory(CreateScope));
        }

        ~ServiceProvider()
        {
            Dispose(false);
        }

        #endregion


        #region IServiceProvider

        public object GetService(Type serviceType)
        {
            try
            {
                return (serviceType.IsInterface && serviceType.IsGenericType &&
                        Equals(enumerableType, serviceType.GetGenericTypeDefinition()) &&
                        !container.IsRegistered(serviceType)) 
                    ? OnGetServices(serviceType.GetGenericArguments().Single()) 
                    : OnGetService(serviceType);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }

        protected virtual object OnGetServices(Type type)
        {
            var named = container.ResolveAll(type);

            if (!(container.IsRegistered(type) || (type.GetGenericArguments().Length > 0 &&
                                                   container.IsRegistered(type.GetGenericTypeDefinition()))))
                return named;

            return Aggregate(named, type);
        }

        protected virtual object OnGetService(Type type)
        {
            return container.Resolve(type);
        }

        protected object Aggregate(IEnumerable<object> seed, Type target)
        {
            return typeof(Enumerable).GetMethod("Cast")
                                     .MakeGenericMethod(target)
                                     .Invoke(null, new[]
                                     {
                                        (new[] { container.Resolve(target) }).Concat(seed)
                                     });
        }

        #endregion


        #region IServiceScopeFactory

        public IServiceScope CreateScope()
        {
            return CreateScope(container);
        }

        internal static IServiceScope CreateScope(IUnityContainer container)
        {
            return new ScopedServiceProvider(container);
        }

        #endregion


        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            var disposable = container;

            container = null;

            if (null != disposable)
                disposable.Dispose();
        }

        #endregion
    }
}