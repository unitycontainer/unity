// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using ObjectBuilder2;

namespace Unity.InterceptionExtension
{
    /// <summary>
    /// Implementation of <see cref="ITypeInterceptionPolicy"/> that returns a pre-created
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
