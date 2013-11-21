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
    /// A <see cref="IBuilderPolicy"/> that can create and return an <see cref="IBuildPlanPolicy"/>
    /// for the given build key.
    /// </summary>
    public interface IBuildPlanCreatorPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Create a build plan using the given context and build key.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="buildKey">Current build key.</param>
        /// <returns>The build plan.</returns>
        IBuildPlanPolicy CreatePlan(IBuilderContext context, NamedTypeBuildKey buildKey);
    }
}
