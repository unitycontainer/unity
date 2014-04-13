// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.ObjectBuilder2.Tests.TestDoubles;
using Microsoft.Practices.ObjectBuilder2.Tests.TestObjects;
using Microsoft.Practices.Unity.TestSupport;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public class InternalAndPrivatePlanFixture
    {
        [TestMethod]
        public void ExistingObjectIsUntouchedByConstructionPlan()
        {
            MockBuilderContext context = GetContext();
            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, new NamedTypeBuildKey(typeof(OptionalLogger)));

            var existing = new OptionalLogger("C:\\log.log");

            context.BuildKey = new NamedTypeBuildKey(typeof(OptionalLogger));
            context.Existing = existing;

            plan.BuildUp(context);
            object result = context.Existing;

            Assert.AreSame(existing, result);
            Assert.AreEqual("C:\\log.log", existing.LogFile);
        }

        [TestMethod]
        public void CanCreateObjectWithoutExplicitConstructorDefined()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<InternalObjectWithoutExplicitConstructor>();
            IBuildPlanPolicy plan =
                GetPlanCreator(context).CreatePlan(context, key);

            context.BuildKey = key;
            plan.BuildUp(context);
            var result = (InternalObjectWithoutExplicitConstructor)context.Existing;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CanCreatePlanAndExecuteItForPrivateClassWhenInFullTrust()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<PrivateClassWithoutExplicitConstructor>();
            IBuildPlanPolicy plan =
                GetPlanCreator(context).CreatePlan(context, key);

            context.BuildKey = key;
            plan.BuildUp(context);

            Assert.IsNotNull(context.Existing);
            Assert.IsInstanceOfType(context.Existing, typeof(PrivateClassWithoutExplicitConstructor));
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
