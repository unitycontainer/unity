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
using System.Text;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Interface that controls when and how types get intercepted.
    /// </summary>
    public interface ITypeInterceptionPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Interceptor to use to create type proxy
        /// </summary>
        /// <param name="context">Context for current build operation.</param>
        ITypeInterceptor GetInterceptor(IBuilderContext context);

        /// <summary>
        /// Cache for proxied type.
        /// </summary>
        Type ProxyType { get; set; }
    }
}
