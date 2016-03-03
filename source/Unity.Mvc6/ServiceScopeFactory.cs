using Microsoft.Extensions.DependencyInjection;
using Unity;

namespace Unity.Mvc
{
    public class ServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IUnityContainer container;

        public ServiceScopeFactory(IUnityContainer container)
        {
            this.container = container;
        }

        IServiceScope IServiceScopeFactory.CreateScope()
        {
            return container.Resolve<IServiceScope>();
        }
    }
}