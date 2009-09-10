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
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A <see cref="IInterceptionBehaviorDescriptor"/> which just returns a pre-configured 
    /// <see cref="IInterceptionBehavior"/>.
    /// </summary>
    public class ExistingInterceptionBehaviorDescriptor : IInterceptionBehaviorDescriptor
    {
        private readonly IInterceptionBehavior interceptionBehavior;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExistingInterceptionBehaviorDescriptor"/> class
        /// with a given <see cref="IInterceptionBehavior"/>.
        /// </summary>
        /// <param name="interceptionBehavior">The interception behavior for the new instance.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptionBehavior"/> is 
        /// <see langword="null"/>.</exception>
        public ExistingInterceptionBehaviorDescriptor(IInterceptionBehavior interceptionBehavior)
        {
            Guard.ArgumentNotNull(interceptionBehavior, "interceptionBehavior");

            this.interceptionBehavior = interceptionBehavior;
        }

        /// <summary>
        /// Returns the behavior configured for this descriptor.
        /// </summary>
        /// <param name="interceptor">The <see cref="IInterceptor"/> used to create the proxy which will receive the 
        /// returned behavior.</param>
        /// <param name="interceptedType">The type for which a proxy is created.</param>
        /// <param name="implementationType">The type of the intercepted object.</param>
        /// <param name="container">A <see cref="IUnityContainer"/> from which any necessary objects can be resolved
        /// to create the behavior.</param>
        /// <returns>The pre-configured interception behavior.</returns>
        public IInterceptionBehavior GetInterceptionBehavior(
            IInterceptor interceptor,
            Type interceptedType,
            Type implementationType,
            IUnityContainer container)
        {
            return this.interceptionBehavior;
        }
    }
}
