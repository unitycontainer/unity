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
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Implementation of <see cref="ITypeInterceptionPolicy"/> that returns a precreated
    /// interceptor object.
    /// </summary>
    public class FixedTypeInterceptionPolicy : ITypeInterceptionPolicy
    {
        private readonly ITypeInterceptor interceptor;

        /// <summary>
        /// Create a new instance of <see cref="FixedTypeInterceptionPolicy"/> that
        /// uses the given <see cref="ITypeInterceptor"/>.
        /// </summary>
        /// <param name="interceptor">Interceptor to use.</param>
        public FixedTypeInterceptionPolicy(ITypeInterceptor interceptor)
        {
            this.interceptor = interceptor;
        }

        /// <summary>
        /// Interceptor to use to create type proxy
        /// </summary>
        /// <param name="context">Context for current build operation.</param>
        public ITypeInterceptor GetInterceptor(IBuilderContext context)
        {
            return interceptor;
        }

        /// <summary>
        /// Cache for proxied type.
        /// </summary>
        public Type ProxyType { get; set; }
    }
}
