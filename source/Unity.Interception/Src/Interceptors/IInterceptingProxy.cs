// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Unity.InterceptionExtension
{
    /// <summary>
    /// This interface is implemented by all proxy objects, type or instance based.
    /// It allows for adding interception behaviors.
    /// </summary>
    public interface IInterceptingProxy
    {
        /// <summary>
        /// Adds a <see cref="IInterceptionBehavior"/> to the proxy.
        /// </summary>
        /// <param name="interceptor">The <see cref="IInterceptionBehavior"/> to add.</param>
        void AddInterceptionBehavior(IInterceptionBehavior interceptor);
    }
}
