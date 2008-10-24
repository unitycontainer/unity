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

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Implementation if <see cref="IInstanceInterceptionPolicy"/>
    /// </summary>
    public class InstanceInterceptionPolicy : IInstanceInterceptionPolicy
    {
        private readonly IInstanceInterceptor interceptor;

        /// <summary>
        /// Create a new instance of <see cref="InstanceInterceptionPolicy"/>.
        /// </summary>
        /// <param name="interceptor">Interceptor to store.</param>
        public InstanceInterceptionPolicy(IInstanceInterceptor interceptor)
        {
            this.interceptor = interceptor;
        }

        /// <summary>
        /// Interceptor to use.
        /// </summary>
        public IInstanceInterceptor Interceptor
        {
            get { return interceptor; }
        }
    }
}
