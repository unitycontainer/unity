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
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// An implementation of <see cref="ITypeInterceptionPolicy"/> that will
    /// resolve the interceptor through the container.
    /// </summary>
    public class ResolvedTypeInterceptionPolicy : ITypeInterceptionPolicy
    {
        private readonly NamedTypeBuildKey buildKey;

        /// <summary>
        /// construct a new <see cref="ResolvedTypeInterceptionPolicy"/> that
        /// will resolve the interceptor with the given <paramref name="buildKey"/>.
        /// </summary>
        /// <param name="buildKey">The build key to use to resolve.</param>
        public ResolvedTypeInterceptionPolicy(NamedTypeBuildKey buildKey)
        {
            this.buildKey = buildKey;
        }

        #region ITypeInterceptionPolicy Members

        /// <summary>
        /// Interceptor to use to create type proxy
        /// </summary>
        /// <param name="context">Context for current build operation.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public ITypeInterceptor GetInterceptor(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            return (ITypeInterceptor) context.NewBuildUp(buildKey);
        }

        /// <summary>
        /// Cache for proxied type.
        /// </summary>
        public Type ProxyType { get; set; }

        #endregion
    }
}
