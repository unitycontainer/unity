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

using Microsoft.Practices.ObjectBuilder2.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public class DynamicMethodPropertySetterFixture
    {
        [TestMethod]
        public void CanInjectProperties()
        {
            TestingBuilderContext context = GetContext();
            object existingObject = new object();
            SingletonLifetimePolicy lifetimePolicy = new SingletonLifetimePolicy();
            lifetimePolicy.SetValue(existingObject);
            context.Policies.Set<ILifetimePolicy>(lifetimePolicy, typeof(object));

            IBuildPlanPolicy plan =
                GetPlanCreator(context).CreatePlan(context, typeof(OnePropertyClass));

            OnePropertyClass existing = new OnePropertyClass();
            context.Existing = existing;
            context.BuildKey = typeof(OnePropertyClass);
            plan.BuildUp(context);

            Assert.IsNotNull(existing.Key);
            Assert.AreSame(existingObject, existing.Key);
        }

        [TestMethod]
        public void GetExpectedExceptionMessageWhenPropertiesAreNotResolved()
        {
            TestingBuilderContext context = TestingBuilderContext.GetFullContext();
            ClassThatTakesInterface existing = new ClassThatTakesInterface();

            try
            {
                context.ExecuteBuildUp(typeof(ClassThatTakesInterface), existing);
                Assert.Fail();
            }
            catch(BuildFailedException ex)
            {
                StringAssert.Contains(ex.Message, "property \"Foo\"");
                return;
            }
        }

        private TestingBuilderContext GetContext()
        {
            StagedStrategyChain<BuilderStage> chain = new StagedStrategyChain<BuilderStage>();
            chain.AddNew<DynamicMethodPropertySetterStrategy>(BuilderStage.Initialization);

            DynamicMethodBuildPlanCreatorPolicy policy =
                new DynamicMethodBuildPlanCreatorPolicy(chain);

            TestingBuilderContext context = new TestingBuilderContext();

            context.Strategies.Add(new LifetimeStrategy());

            context.PersistentPolicies.SetDefault<IDynamicBuilderMethodCreatorPolicy>(
                new DefaultDynamicBuilderMethodCreatorPolicy());
            context.Policies.SetDefault<IConstructorSelectorPolicy>(
                new ConstructorSelectorPolicy<InjectionConstructorAttribute>());
            context.Policies.SetDefault<IPropertySelectorPolicy>(
                new PropertySelectorPolicy<DependencyAttribute>());
            context.Policies.SetDefault<IBuildPlanCreatorPolicy>(policy);

            return context;
        }

        private IBuildPlanCreatorPolicy GetPlanCreator(IBuilderContext context)
        {
            return context.Policies.Get<IBuildPlanCreatorPolicy>(null);
        }


        internal class OnePropertyClass
        {
            private object key;

            [Dependency]
            public object Key
            {
                get { return key; }
                set { key = value; }
            }
        }

        internal interface IFoo
        {
        }

        internal class ClassThatTakesInterface
        {
            [Dependency]
            public IFoo Foo
            {
                get { return null;  }
                set { }
            }
        }
    }
}
