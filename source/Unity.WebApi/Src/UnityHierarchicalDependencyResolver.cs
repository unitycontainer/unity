// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;

namespace Microsoft.Practices.Unity.WebApi
{
    /// <summary>
    /// An implementation of <see cref="IDependencyResolver"/> that wraps a Unity container and creates a new child container
    /// when <see cref="BeginScope"/> is invoked.
    /// </summary>
    /// <remarks>
    /// Because each scope creates a new child Unity container, you can benefit from using the <see cref="HierarchicalLifetimeManager"/>
    /// lifetime manager.
    /// </remarks>
    public sealed class UnityHierarchicalDependencyResolver : IDependencyResolver
    {
        private IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityDependencyResolver"/> class for a container.
        /// </summary>
        /// <param name="container">The <see cref="IUnityContainer"/> to wrap with the <see cref="IDependencyResolver"/>
        /// interface implementation.</param>
        public UnityHierarchicalDependencyResolver(IUnityContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            this.container = container;
        }

        /// <summary>
        /// Starts a resolution scope by creating a new child Unity container.
        /// </summary>
        /// <returns>The dependency scope.</returns>
        public IDependencyScope BeginScope()
        {
            return new UnityHierarchicalDependencyScope(this.container);
        }

        /// <summary>
        /// Disposes the wrapped <see cref="IUnityContainer"/>.
        /// </summary>
        public void Dispose()
        {
            this.container.Dispose();
        }

        /// <summary>
        /// Resolves an instance of the default requested type from the container.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type"/> of the object to get from the container.</param>
        /// <returns>The retrieved object.</returns>
        public object GetService(Type serviceType)
        {
            try
            {
                return this.container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        /// <summary>
        /// Resolves multiply registered services.
        /// </summary>
        /// <param name="serviceType">The type of the requested services.</param>
        /// <returns>The requested services.</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return this.container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        private sealed class UnityHierarchicalDependencyScope : IDependencyScope
        {
            private readonly IUnityContainer container;

            public UnityHierarchicalDependencyScope(IUnityContainer parentContainer)
            {
                this.container = parentContainer.CreateChildContainer();
            }

            public object GetService(Type serviceType)
            {
                return this.container.Resolve(serviceType);
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                return this.container.ResolveAll(serviceType);
            }

            public void Dispose()
            {
                this.container.Dispose();
            }
        }
    }
}
