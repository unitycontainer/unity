//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// An implementation of <see cref="IServiceLocator"/> that wraps a Unity container.
    /// </summary>
    public sealed class UnityServiceLocator : ServiceLocatorImplBase, IDisposable
    {
        private IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityServiceLocator"/> class for a container.
        /// </summary>
        /// <param name="container">The <see cref="IUnityContainer"/> to wrap with the <see cref="IServiceLocator"/>
        /// interface implementation.</param>
        public UnityServiceLocator(IUnityContainer container)
        {
            this.container = container;
            container.RegisterInstance<IServiceLocator>(this, new ExternallyControlledLifetimeManager());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly",
            Justification="Object is not finalizable, no reason to call SuppressFinalize")]
        public void Dispose()
        {
            if (container != null)
            {
                container.Dispose();
                container = null;
            }
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of resolving
        ///             the requested service instance.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param><param name="key">Name of registered service you want. May be null.</param>
        /// <returns>
        /// The requested service instance.
        /// </returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (container == null) throw new ObjectDisposedException("container");
            return container.Resolve(serviceType, key);
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of
        ///             resolving all the requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>
        /// Sequence of service instance objects.
        /// </returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            if (container == null) throw new ObjectDisposedException("container");
            return container.ResolveAll(serviceType);
        }
    }
}
