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
		public override void PreBuildUp(IBuilderContext context)
		{
			var policy = context.Policies.Get<IBuildKeyMappingPolicy>(context.BuildKey);

			if (policy != null)
			{
				context.BuildKey = policy.Map(context.BuildKey, context);
			}
		}
	}
}
