// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// An <see cref="IBuildPlanCreatorPolicy"/> implementation
    /// that constructs a build plan via dynamic IL emission.
    /// </summary>
    public class DynamicMethodBuildPlanCreatorPolicy : IBuildPlanCreatorPolicy
    {
        private IStagedStrategyChain strategies;

        /// <summary>
        /// Construct a <see cref="DynamicMethodBuildPlanCreatorPolicy"/> that
        /// uses the given strategy chain to construct the build plan.
        /// </summary>
        /// <param name="strategies">The strategy chain.</param>
        public DynamicMethodBuildPlanCreatorPolicy(IStagedStrategyChain strategies)
        {
            this.strategies = strategies;
        }

        /// <summary>
        /// Construct a build plan.
        /// </summary>
        /// <param name="context">The current build context.</param>
        /// <param name="buildKey">The current build key.</param>
        /// <returns>The created build plan.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class")]
        public IBuildPlanPolicy CreatePlan(IBuilderContext context, NamedTypeBuildKey buildKey)
        {
            Guard.ArgumentNotNull(buildKey, "buildKey");

            DynamicBuildPlanGenerationContext generatorContext =
                new DynamicBuildPlanGenerationContext(buildKey.Type);

            IBuilderContext planContext = GetContext(context, buildKey, generatorContext);

            planContext.Strategies.ExecuteBuildUp(planContext);

            return new DynamicMethodBuildPlan(generatorContext.GetBuildMethod());
        }

        private IBuilderContext GetContext(IBuilderContext originalContext, NamedTypeBuildKey buildKey, DynamicBuildPlanGenerationContext ilContext)
        {
            return new BuilderContext(
                strategies.MakeStrategyChain(),
                originalContext.Lifetime,
                originalContext.PersistentPolicies,
                originalContext.Policies,
                buildKey,
                ilContext);
        }
    }
}
