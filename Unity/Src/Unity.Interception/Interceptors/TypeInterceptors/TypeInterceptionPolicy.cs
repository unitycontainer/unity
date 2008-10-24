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
    /// Implementation of <see cref="ITypeInterceptionPolicy"/>.
    /// </summary>
    public class TypeInterceptionPolicy : ITypeInterceptionPolicy
    {
        private readonly ITypeInterceptor interceptor;
        private Type proxyType;

        /// <summary>
        /// Create a new instance of <see cref="TypeInterceptionPolicy"/> that
        /// uses the given <see cref="ITypeInterceptor"/>.
        /// </summary>
        /// <param name="interceptor">Interceptor to use.</param>
        public TypeInterceptionPolicy(ITypeInterceptor interceptor)
        {
            this.interceptor = interceptor;
        }

        /// <summary>
        /// Interceptor to use to create type proxy
        /// </summary>
        public ITypeInterceptor Interceptor
        {
            get { return interceptor; }
        }

        /// <summary>
        /// Cache for proxied type.
        /// </summary>
        public Type ProxyType
        {
            get { return proxyType; }
            set { proxyType = value; }
        }
    }
}
