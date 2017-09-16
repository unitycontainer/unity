using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics;

namespace Unity.Mvc
{
    public class ServiceScope : ServiceProvider, IServiceScope
    {
        private readonly List<IDisposable> scope = new List<IDisposable>();

        public ServiceScope(IUnityContainer container)
            : base(container.CreateChildContainer())
        {
        }

        IServiceProvider IServiceScope.ServiceProvider => this;


        public override object GetService(Type serviceType)
        {
            try
            {
                return container.Resolve(serviceType);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }

        void IDisposable.Dispose()
        {
            foreach (IDisposable disposable in scope.ToArray())
                disposable.Dispose();

            scope.Clear();
            container.Dispose();
        }
    }
}