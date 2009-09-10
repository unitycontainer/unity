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
using Microsoft.Practices.Unity.ObjectBuilder;

namespace Microsoft.Practices.Unity
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
            //
            // Main strategy chain
            //
            Context.Strategies.AddNew<BuildKeyMappingStrategy>(UnityBuildStage.TypeMapping);
            Context.Strategies.AddNew<HierarchicalLifetimeStrategy>(UnityBuildStage.Lifetime);
            Context.Strategies.AddNew<LifetimeStrategy>(UnityBuildStage.Lifetime);

            Context.Strategies.AddNew<ArrayResolutionStrategy>(UnityBuildStage.Creation);
            Context.Strategies.AddNew<BuildPlanStrategy>(UnityBuildStage.Creation);

            //
            // Build plan strategy chain
            //
            Context.BuildPlanStrategies.AddNew<DynamicMethodConstructorStrategy>(
                UnityBuildStage.Creation);
            Context.BuildPlanStrategies.AddNew<DynamicMethodPropertySetterStrategy>(
                UnityBuildStage.Initialization);
            Context.BuildPlanStrategies.AddNew<DynamicMethodCallStrategy>(
                UnityBuildStage.Initialization);
            //
            // Policies - mostly used by the build plan strategies
            //
            Context.Policies.SetDefault<IConstructorSelectorPolicy>(
                new DefaultUnityConstructorSelectorPolicy());
            Context.Policies.SetDefault<IPropertySelectorPolicy>(
                new DefaultUnityPropertySelectorPolicy());
            Context.Policies.SetDefault<IMethodSelectorPolicy>(
                new DefaultUnityMethodSelectorPolicy());

            SetDynamicBuilderMethodCreatorPolicy();

            Context.Policies.SetDefault<IBuildPlanCreatorPolicy>(
                new DynamicMethodBuildPlanCreatorPolicy(Context.BuildPlanStrategies));
        }
    }
}
