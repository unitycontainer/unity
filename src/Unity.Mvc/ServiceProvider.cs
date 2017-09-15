using System;

namespace Unity.Mvc
{
    public class ServiceProvider : IServiceProvider
                                   
    {
        private readonly IUnityContainer container;

        public ServiceProvider(IUnityContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            object o = container.Resolve(serviceType);
            return o;
        }
    }
}