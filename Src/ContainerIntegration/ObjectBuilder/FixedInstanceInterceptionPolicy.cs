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
