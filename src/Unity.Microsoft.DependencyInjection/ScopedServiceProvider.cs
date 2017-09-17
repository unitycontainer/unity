using System;
using Microsoft.Extensions.DependencyInjection;

namespace Unity.Microsoft.DependencyInjection
{
    internal class ScopedServiceProvider : ServiceProvider, IServiceScope
    {
        IUnityContainer parent;

        public ScopedServiceProvider(IUnityContainer container)
            : base(container.CreateChildContainer())
        {
            parent = container;
        }

        IServiceProvider IServiceScope.ServiceProvider => this;

        protected override void Dispose(bool disposing)
        {
            var disposable = container;

            base.Dispose(disposing);

            disposable?.Dispose();
        }
    }
}