// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using ObjectBuilder2.Tests.TestDoubles;
using ObjectBuilder2.Tests.TestObjects;
using Unity.TestSupport;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#else
using Xunit;
#endif

namespace ObjectBuilder2.Tests
{
     
    public class InternalAndPrivatePlanFixture
    {
        [Fact]
        public void ExistingObjectIsUntouchedByConstructionPlan()
        {
            MockBuilderContext context = GetContext();
            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, new NamedTypeBuildKey(typeof(OptionalLogger)));

            var existing = new OptionalLogger("C:\\log.log");

            context.BuildKey = new NamedTypeBuildKey(typeof(OptionalLogger));
            context.Existing = existing;

            plan.BuildUp(context);
            object result = context.Existing;

            Assert.Same(existing, result);
            Assert.Equal("C:\\log.log", existing.LogFile);
        }

        [Fact]
        public void CanCreateObjectWithoutExplicitConstructorDefined()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<InternalObjectWithoutExplicitConstructor>();
            IBuildPlanPolicy plan =
                GetPlanCreator(context).CreatePlan(context, key);

            context.BuildKey = key;
            plan.BuildUp(context);
            var result = (InternalObjectWithoutExplicitConstructor)context.Existing;
            Assert.NotNull(result);
        }

        [Fact]
        public void CanCreatePlanAndExecuteItForPrivateClassWhenInFullTrust()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<PrivateClassWithoutExplicitConstructor>();
            IBuildPlanPolicy plan =
                GetPlanCreator(context).CreatePlan(context, key);

            context.BuildKey = key;
            plan.BuildUp(context);

            Assert.NotNull(context.Existing);
            AssertExtensions.IsInstanceOfType(context.Existing, typeof(PrivateClassWithoutExplicitConstructor));
        }

        private static MockBuilderContext GetContext()
        {
            var chain = new StagedStrategyChain<BuilderStage>();
            chain.AddNew<DynamicMethodConstructorStrategy>(BuilderStage.Creation);

            var policy = new DynamicMethodBuildPlanCreatorPolicy(chain);

            var context = new MockBuilderContext();

            context.Strategies.Add(new LifetimeStrategy());

            context.PersistentPolicies.SetDefault<IConstructorSelectorPolicy>(
                new ConstructorSelectorPolicy<InjectionConstructorAttribute>());
            context.PersistentPolicies.SetDefault<IBuildPlanCreatorPolicy>(policy);

            return context;
        }

        private static IBuildPlanCreatorPolicy GetPlanCreator(IBuilderContext context)
        {
            return context.Policies.Get<IBuildPlanCreatorPolicy>(null);
        }
        internal class InternalObjectWithoutExplicitConstructor
        {
            public void DoSomething()
            {
                // We do nothing!
            }
        }

        private class PrivateClassWithoutExplicitConstructor
        {
            public void DoNothing()
            {
                // Again, do nothing
            }
        }
    }
}
