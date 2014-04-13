// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents a strategy for mapping build keys in the build up operation.
    /// </summary>
    public class BuildKeyMappingStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation.  Looks for the <see cref="IBuildKeyMappingPolicy"/>
        /// and if found maps the build key for the current operation.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        //[SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            //Justification = "Validation done by Guard class")]
        public override void PreBuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            var policy = context.Policies.Get<IBuildKeyMappingPolicy>(context.BuildKey);

            if (policy != null)
            {
                context.BuildKey = policy.Map(context.BuildKey, context);
            }
        }
    }
}
