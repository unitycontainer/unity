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

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A <see cref="IInterceptionBehaviorDescriptor"/> which resolves its <see cref="IInterceptionBehavior"/>
    /// from a container.
    /// </summary>
    public class ContainerResolvedInterceptionBehaviorDescriptor : IInterceptionBehaviorDescriptor
    {
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerResolvedInterceptionBehaviorDescriptor"/> with the
        /// name to use when resolving its behavior.
        /// </summary>
        /// <param name="name">The name to use when resolving the <see cref="IInterceptionBehavior"/>.</param>
        public ContainerResolvedInterceptionBehaviorDescriptor(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Returns a behavior resolved from <paramref name="container"/> using a pre-configured name.
        /// </summary>
        /// <param name="interceptor">The <see cref="IInterceptor"/> used to create the proxy which will receive the 
        /// returned behavior.</param>
        /// <param name="interceptedType">The type for which a proxy is created.</param>
        /// <param name="implementationType">The type of the intercepted object.</param>
        /// <param name="container">A <see cref="IUnityContainer"/> from which any necessary objects can be resolved
        /// to create the behavior.</param>
        /// <returns>The resolved interception behavior.</returns>
        public IInterceptionBehavior GetInterceptionBehavior(
            IInterceptor interceptor,
            Type interceptedType,
            Type implementationType,
            IUnityContainer container)
        {
            return container.Resolve<IInterceptionBehavior>(this.name);
        }
    }
}
