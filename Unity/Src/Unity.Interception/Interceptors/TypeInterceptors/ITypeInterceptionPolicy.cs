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
        ITypeInterceptor Interceptor { get; }

        /// <summary>
        /// Cache for proxied type.
        /// </summary>
        Type ProxyType { get; set; }
    }
}
