using System;
using System.Diagnostics;

namespace Unity.Mvc
{
    public class ServiceProvider : IServiceProvider
                                   
    {
        protected readonly IUnityContainer container;

        public ServiceProvider(IUnityContainer container)
        {
            this.container = container;
        }

        public virtual object GetService(Type serviceType)
        {
            try
            {
                return container.Resolve(serviceType);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }
    }
}