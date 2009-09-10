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
    /// Describes a <see cref="IInterceptionBehavior"/>.
    /// </summary>
    public interface IInterceptionBehaviorDescriptor
    {
        /// <summary>
        /// Returns the <see cref="IInterceptionBehavior"/> represented by this descriptor.
        /// </summary>
        /// <param name="interceptor">The <see cref="IInterceptor"/> used to create the proxy which will receive the 
        /// returned behavior.</param>
        /// <param name="interceptedType">The type for which a proxy is created.</param>
        /// <param name="implementationType">The type of the intercepted object.</param>
        /// <param name="container">A <see cref="IUnityContainer"/> from which any necessary objects can be resolved
        /// to create the behavior.</param>
        /// <returns>The represented behavior, or <see langword="null"/> if the represented behavior is not 
        /// applicable for the intercepted type.</returns>
        IInterceptionBehavior GetInterceptionBehavior(
            IInterceptor interceptor,
            Type interceptedType,
            Type implementationType,
            IUnityContainer container);
    }
}
