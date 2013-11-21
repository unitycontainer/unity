// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Implementation of <see cref="IInstanceInterceptionPolicy"/> that returns a
    /// pre-created interceptor.
    /// </summary>
    public class FixedInstanceInterceptionPolicy : IInstanceInterceptionPolicy
    {
        private readonly IInstanceInterceptor interceptor;

        /// <summary>
        /// Create a new instance of <see cref="FixedInstanceInterceptionPolicy"/>.
        /// </summary>
        /// <param name="interceptor">Interceptor to store.</param>
        public FixedInstanceInterceptionPolicy(IInstanceInterceptor interceptor)
        {
            this.interceptor = interceptor;
        }

        /// <summary>
        /// Interceptor to use.
        /// </summary>
        /// <param name="context">Context for current build operation.</param>
        public IInstanceInterceptor GetInterceptor(IBuilderContext context)
        {
            return interceptor;
        }
    }
}
