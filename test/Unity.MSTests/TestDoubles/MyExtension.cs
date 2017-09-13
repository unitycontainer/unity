// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using ObjectBuilder2;
using System;
using Unity.ObjectBuilder;
using Unity.Tests.TestObjects;

namespace Unity.Tests.TestDoubles
{
    internal class MyExtension : UnityContainerExtension, IUnityContainerExtensionConfigurator
    {
        private UnityContainer container;
        private MyExtensionContext context;
        
        public MyExtension()
        {
            MyExtensionContext myExtensionContext = new MyExtensionContext(this.container);
            this.InitializeExtension(myExtensionContext);
        }

        public void InitializeExtension(MyExtensionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.context = context;
            this.Initialize();
        }

        protected override void Initialize()
        {
            // Main strategy chain
            this.context.Strategies.AddNew<BuildKeyMappingStrategy>(UnityBuildStage.TypeMapping);
            this.context.Strategies.AddNew<LifetimeStrategy>(UnityBuildStage.Lifetime);

            this.context.Strategies.AddNew<BuildPlanStrategy>(UnityBuildStage.Creation);

            // Build plan strategy chain
            this.context.BuildPlanStrategies.AddNew<DynamicMethodConstructorStrategy>(
                UnityBuildStage.Creation);
            this.context.BuildPlanStrategies.AddNew<DynamicMethodPropertySetterStrategy>(
                UnityBuildStage.Initialization);
            this.context.BuildPlanStrategies.AddNew<DynamicMethodCallStrategy>(
                UnityBuildStage.Initialization);

            // Policies - mostly used by the build plan strategies
            this.context.Policies.SetDefault<IConstructorSelectorPolicy>(
                new MyUnityContainerSelectorPolicy());
            this.context.Policies.SetDefault<IPropertySelectorPolicy>(
                new DefaultUnityPropertySelectorPolicy());
            this.context.Policies.SetDefault<IMethodSelectorPolicy>(
                new DefaultUnityMethodSelectorPolicy());

            this.context.Policies.SetDefault<IBuildPlanCreatorPolicy>(
                new DynamicMethodBuildPlanCreatorPolicy(this.context.BuildPlanStrategies));
        }
    }
}
