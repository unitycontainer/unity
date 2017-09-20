// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Unity.InterceptionExtension
{
    /// <summary>
    /// Interface for interceptors that generate separate proxy
    /// objects to implement interception on instances.
    /// </summary>
    public interface IInstanceInterceptor : IInterceptor
    {
        /// <summary>
        /// Create a proxy object that provides interception for <paramref name="target"/>.
        /// </summary>
        /// <param name="t">Type to generate the proxy of.</param>
        /// <param name="target">Object to create the proxy for.</param>
        /// <param name="additionalInterfaces">Additional interfaces the proxy must implement.</param>
        /// <returns>The proxy object.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t", Justification = "Parameter name is meaningful enough in context")]
        IInterceptingProxy CreateProxy(Type t, object target, params Type[] additionalInterfaces);
    }
}
