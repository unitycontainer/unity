// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public IInstanceInterceptor GetInterceptor(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            return (IInstanceInterceptor) context.NewBuildUp(buildKey);
        }

        #endregion
    }
}
