using System;
using Microsoft.Extensions.DependencyInjection;
using Unity;

namespace Unity.Mvc
{
    public class ServiceScope : IServiceScope
    {
        private readonly IUnityContainer container;
        private readonly IServiceProvider serviceProvider;

        public ServiceScope(IUnityContainer container)
        {
            this.container = container.CreateChildContainer();
            serviceProvider = this.container.Resolve<IServiceProvider>();
        }

        IServiceProvider IServiceScope.ServiceProvider
        {
            get
            {
                return serviceProvider;
            }
        }

        void IDisposable.Dispose()
        {
            container.Dispose();
        }
    }
}