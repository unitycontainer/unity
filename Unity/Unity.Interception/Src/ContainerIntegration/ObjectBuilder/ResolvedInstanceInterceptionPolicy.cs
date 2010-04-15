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
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// An implementation of <see cref="IInstanceInterceptionPolicy"/> that will
    /// resolve the interceptor through the container.
    /// </summary>
    public class ResolvedInstanceInterceptionPolicy : IInstanceInterceptionPolicy
    {
        private readonly NamedTypeBuildKey buildKey;

        /// <summary>
        /// Construct a new <see cref="ResolvedInstanceInterceptionPolicy"/> that
        /// will resolve the interceptor using the given build key.
        /// </summary>
        /// <param name="buildKey">build key to resolve.</param>
        public ResolvedInstanceInterceptionPolicy(NamedTypeBuildKey buildKey)
        {
            this.buildKey = buildKey;
        }

        #region IInstanceInterceptionPolicy Members

        /// <summary>
        /// Interceptor to use.
        /// </summary>
        /// <param name="context">Context for current build operation.</param>
        public IInstanceInterceptor GetInterceptor(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            return (IInstanceInterceptor) context.NewBuildUp(buildKey);
        }

        #endregion
    }
}
