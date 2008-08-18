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

using System;
using Microsoft.Practices.ObjectBuilder2.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public class DynamicMethodCallFixture
    {
        [TestMethod]
        public void CallsMethodsMarkedWithInjectionAttribute()
        {
            TestingBuilderContext context = GetContext();
            object intValue = 42;
            object stringValue = "Hello world";

            SetSingleton(context, typeof(int), intValue);
            SetSingleton(context, typeof(string), stringValue);

            IBuildPlanPolicy plan =
                GetPlanCreator(context).CreatePlan(context, typeof(ObjectWithInjectionMethod));

            ObjectWithInjectionMethod existing = new ObjectWithInjectionMethod();

            context.BuildKey = typeof(ObjectWithInjectionMethod);
            context.Existing = existing;
            plan.BuildUp(context);

            GC.KeepAlive(intValue);
            GC.KeepAlive(stringValue);

            Assert.IsTrue(existing.WasInjected);
            Assert.AreEqual(intValue, existing.IntValue);
            Assert.AreEqual(stringValue, existing.StringValue);
        }

        [TestMethod]
        public void ThrowsWhenBuildingPlanWithGenericInjectionMethod()
        {
            try
            {
                TestingBuilderContext context = GetContext();
                GetPlanCreator(context).CreatePlan(context, typeof(ObjectWithGenericInjectionMethod));
                Assert.Fail();
            }
            catch(BuildFailedException ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(IllegalInjectionMethodException));
            }
        }

        [TestMethod]
        public void ThrowsWhenBuildingPlanWithMethodWithOutParam()
        {
            try
            {
                TestingBuilderContext context = GetContext();
                GetPlanCreator(context).CreatePlan(context, typeof(ObjectWithOutParamMethod));
                Assert.Fail();
            }
            catch(BuildFailedException ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(IllegalInjectionMethodException));
            }
        }

        [TestMethod]
        public void ThrowsWhenBuildingPlanWithMethodWithRefParam()
        {
            try
            {
                TestingBuilderContext context = GetContext();
                GetPlanCreator(context).CreatePlan(context, typeof(ObjectWithRefParamMethod));
                Assert.Fail();
            }
            catch (BuildFailedException ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(IllegalInjectionMethodException));
            }
        }

        [TestMethod]
        public void GetProperExceptionMessageWhenFailingToResolveMethodParameter()
        {
            TestingBuilderContext context = TestingBuilderContext.GetFullContext();
            ILifetimePolicy intLifetime = new SingletonLifetimePolicy();
            intLifetime.SetValue(42);
            context.PersistentPolicies.Set(intLifetime, typeof(int));

            ObjectWithInjectionMethod existing = new ObjectWithInjectionMethod();

            try
            {
                context.ExecuteBuildUp(typeof(ObjectWithInjectionMethod), existing);
                Assert.Fail();
            }
            catch(BuildFailedException ex)
            {
                StringAssert.Contains(ex.Message, "DoSomething");
                StringAssert.Contains(ex.Message, "stringParam");
            }
        }

        private TestingBuilderContext GetContext()
        {
            StagedStrategyChain<BuilderStage> chain = new StagedStrategyChain<BuilderStage>();
            chain.AddNew<DynamicMethodCallStrategy>(BuilderStage.Initialization);

            DynamicMethodBuildPlanCreatorPolicy policy =
                new DynamicMethodBuildPlanCreatorPolicy(chain);

            TestingBuilderContext context = new TestingBuilderContext();

            context.Strategies.Add(new LifetimeStrategy());

            context.PersistentPolicies.SetDefault<IDynamicBuilderMethodCreatorPolicy>(
                new DefaultDynamicBuilderMethodCreatorPolicy());
            context.PersistentPolicies.SetDefault<IConstructorSelectorPolicy>(
                new ConstructorSelectorPolicy<InjectionConstructorAttribute>());
            context.PersistentPolicies.SetDefault<IPropertySelectorPolicy>(
                new PropertySelectorPolicy<DependencyAttribute>());
            context.PersistentPolicies.SetDefault<IMethodSelectorPolicy>(
                new MethodSelectorPolicy<InjectionMethodAttribute>());

            context.PersistentPolicies.SetDefault<IBuildPlanCreatorPolicy>(policy);

            return context;
        }

        private IBuildPlanCreatorPolicy GetPlanCreator(IBuilderContext context)
        {
            return context.Policies.Get<IBuildPlanCreatorPolicy>(null);
        }

        private void SetSingleton(IBuilderContext context, Type t, object value)
        {
            SingletonLifetimePolicy policy = new SingletonLifetimePolicy();
            policy.SetValue(value);
            context.PersistentPolicies.Set<ILifetimePolicy>(policy, t);
        }

        internal class ObjectWithInjectionMethod
        {
            private int intValue;
            private string stringValue;
            private bool wasInjected = false;

            [InjectionMethod]
            public void DoSomething(int intParam, string stringParam)
            {
                intValue = intParam;
                stringValue = stringParam;
                wasInjected = true;
            }

            public int IntValue
            {
                get { return intValue; }
            }

            public string StringValue
            {
                get { return stringValue; }
            }

            public bool WasInjected
            {
                get { return wasInjected; }
            }
        }

        private class ObjectWithGenericInjectionMethod
        {
            [InjectionMethod]
            public void DoSomethingGeneric<T>(T input)
            {
                
            }
        }

        private class ObjectWithOutParamMethod
        {
            [InjectionMethod]
            public void DoSomething(out int result)
            {
                result = 42;
            }
        }

        private class ObjectWithRefParamMethod
        {
            [InjectionMethod]
            public void DoSomething(ref int result)
            {
                result *= 2;
            }
        }
    }
}
