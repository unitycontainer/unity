// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// An interface that determines when to intercept instances
    /// and which interceptor to use.
    /// </summary>
    public interface IInstanceInterceptionPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Interceptor to use.
        /// </summary>
        /// <param name="context">Context for current build operation.</param>
        IInstanceInterceptor GetInterceptor(IBuilderContext context);
    }
}
