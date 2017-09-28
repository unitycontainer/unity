// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using ObjectBuilder2;
using Unity.ObjectBuilder;
using System.Collections.Generic;

namespace Unity
{
    /// <summary>
    /// This extension installs the default strategies and policies into the container
    /// to implement the standard behavior of the Unity container.
    /// </summary>
    public partial class UnityDefaultStrategiesExtension : UnityContainerExtension
    {
        /// <summary>
        /// Add the default ObjectBuilder strategies &amp; policies to the container.
        /// </summary>
        protected override void Initialize()
        {
            // Setup strategy chain

            // Main strategy chain
            Context.Strategies.AddNew<LifetimeStrategy>(UnityBuildStage.Lifetime);
            Context.Strategies.AddNew<BuildKeyMappingStrategy>(UnityBuildStage.TypeMapping);

            Context.Strategies.AddNew<ArrayResolutionStrategy>(UnityBuildStage.Creation);
            Context.Strategies.AddNew<BuildPlanStrategy>(UnityBuildStage.Creation);

            // Build plan strategy chain
            Context.BuildPlanStrategies.AddNew<DynamicMethodConstructorStrategy>(UnityBuildStage.Creation);
            Context.BuildPlanStrategies.AddNew<DynamicMethodPropertySetterStrategy>(UnityBuildStage.Initialization);
            Context.BuildPlanStrategies.AddNew<DynamicMethodCallStrategy>(UnityBuildStage.Initialization);

            // Policies - mostly used by the build plan strategies
            Context.Policies.SetDefault<IConstructorSelectorPolicy>(new DefaultUnityConstructorSelectorPolicy());
            Context.Policies.SetDefault<IPropertySelectorPolicy>(new DefaultUnityPropertySelectorPolicy());
            Context.Policies.SetDefault<IMethodSelectorPolicy>(new DefaultUnityMethodSelectorPolicy());

            Context.Policies.SetDefault<IBuildPlanCreatorPolicy>(new DynamicMethodBuildPlanCreatorPolicy(Context.BuildPlanStrategies));

            Context.Policies.Set<IBuildPlanPolicy>(new DeferredResolveBuildPlanPolicy(), typeof(Func<>));
            Context.Policies.Set<ILifetimePolicy>(new PerResolveLifetimeManager(), typeof(Func<>));

            Context.Policies.Set<IBuildPlanCreatorPolicy>(new LazyDynamicMethodBuildPlanCreatorPolicy(), typeof(Lazy<>));
            Context.Policies.Set<IBuildPlanCreatorPolicy>(new EnumerableDynamicMethodBuildPlanCreatorPolicy(), typeof(IEnumerable<>));
        }
    }
}
