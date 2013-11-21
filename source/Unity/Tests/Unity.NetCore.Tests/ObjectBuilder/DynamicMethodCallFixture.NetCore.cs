// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2.Tests.TestDoubles;
using Microsoft.Practices.ObjectBuilder2.Tests.TestObjects;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using DependencyAttribute=Microsoft.Practices.ObjectBuilder2.Tests.TestDoubles.DependencyAttribute;
using InjectionConstructorAttribute=Microsoft.Practices.ObjectBuilder2.Tests.TestDoubles.InjectionConstructorAttribute;
using InjectionMethodAttribute=Microsoft.Practices.ObjectBuilder2.Tests.TestDoubles.InjectionMethodAttribute;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public class DynamicMethodCallFixture
    {
        [TestMethod]
        public void CallsMethodsMarkedWithInjectionAttribute()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<ObjectWithInjectionMethod>();
            object intValue = 42;
            object stringValue = "Hello world";

            SetSingleton(context, typeof(int), intValue);
            SetSingleton(context, typeof(string), stringValue);

            IBuildPlanPolicy plan =
                GetPlanCreator(context).CreatePlan(context, key);

            ObjectWithInjectionMethod existing = new ObjectWithInjectionMethod();

            context.BuildKey = key;
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
                MockBuilderContext context = GetContext();
                GetPlanCreator(context).CreatePlan(context, new NamedTypeBuildKey(typeof(ObjectWithGenericInjectionMethod)));
                Assert.Fail();
            }
            catch (IllegalInjectionMethodException)
            {
                // This is what we want
            }
        }

        [TestMethod]
        public void ThrowsWhenBuildingPlanWithMethodWithOutParam()
        {
            try
            {
                MockBuilderContext context = GetContext();
                GetPlanCreator(context).CreatePlan(context, new NamedTypeBuildKey(typeof(ObjectWithOutParamMethod)));
                Assert.Fail();
            }
            catch (IllegalInjectionMethodException)
            {
            }
        }

        [TestMethod]
        public void ThrowsWhenBuildingPlanWithMethodWithRefParam()
        {
            try
            {
                MockBuilderContext context = GetContext();
                GetPlanCreator(context).CreatePlan(context, new NamedTypeBuildKey(typeof(ObjectWithRefParamMethod)));
                Assert.Fail();
            }
            catch (IllegalInjectionMethodException)
            {
            }
        }

        [TestMethod]
        public void TheCurrentOperationIsNullAfterSuccessfullyExecutingTheBuildPlan()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<ObjectWithSingleInjectionMethod>();
            context.BuildKey = key;
            context.Existing = new ObjectWithSingleInjectionMethod();

            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, key);
            plan.BuildUp(context);

            Assert.IsNull(context.CurrentOperation);
        }

        [TestMethod]
        public void ExceptionThrownWhileInvokingTheInjectionMethodIsBubbledUpAndTheCurrentOperationIsNotCleared()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<ObjectWithSingleThrowingInjectionMethod>();
            context.BuildKey = key;
            context.Existing = new ObjectWithSingleThrowingInjectionMethod();

            IBuildPlanPolicy plan =
                GetPlanCreator(context).CreatePlan(context, key);

            try
            {
                plan.BuildUp(context);
                Assert.Fail("failure expected");
            }
            catch (Exception e)
            {
                Assert.AreSame(ObjectWithSingleThrowingInjectionMethod.injectionMethodException, e);
                var operation = (InvokingMethodOperation) context.CurrentOperation;
                Assert.IsNotNull(operation);

                Assert.AreSame(typeof(ObjectWithSingleThrowingInjectionMethod), operation.TypeBeingConstructed);
            }
        }

        [TestMethod]
        public void ResolvingAParameterSetsTheCurrentOperation()
        {
            var resolverPolicy = new CurrentOperationSensingResolverPolicy<object>();

            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<ObjectWithSingleInjectionMethod>();
            context.BuildKey = key;
            context.Existing = new ObjectWithSingleInjectionMethod();

            context.Policies.Set<IMethodSelectorPolicy>(
                new TestSingleArgumentMethodSelectorPolicy<ObjectWithSingleInjectionMethod>(resolverPolicy),
                key);

            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, key);

            plan.BuildUp(context);

            Assert.IsNotNull(resolverPolicy.currentOperation);
        }

        [TestMethod]
        public void ExceptionThrownWhileResolvingAParameterIsBubbledUpAndTheCurrentOperationIsNotCleared()
        {
            var exception = new ArgumentException();
            var resolverPolicy = new ExceptionThrowingTestResolverPolicy(exception);

            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<ObjectWithSingleInjectionMethod>();
            context.BuildKey = key;
            context.Existing = new ObjectWithSingleInjectionMethod();

            context.Policies.Set<IMethodSelectorPolicy>(
                new TestSingleArgumentMethodSelectorPolicy<ObjectWithSingleInjectionMethod>(resolverPolicy),
                key);

            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, key);

            try
            {
                plan.BuildUp(context);
                Assert.Fail("failure expected");
            }
            catch (Exception e)
            {
                Assert.AreSame(exception, e);
                var operation = (MethodArgumentResolveOperation) context.CurrentOperation;
                Assert.IsNotNull(operation);

                Assert.AreSame(typeof(ObjectWithSingleInjectionMethod), operation.TypeBeingConstructed);
                Assert.AreEqual("parameter", operation.ParameterName);
            }
        }

        private MockBuilderContext GetContext()
        {
            StagedStrategyChain<BuilderStage> chain = new StagedStrategyChain<BuilderStage>();
            chain.AddNew<DynamicMethodCallStrategy>(BuilderStage.Initialization);

            DynamicMethodBuildPlanCreatorPolicy policy =
                new DynamicMethodBuildPlanCreatorPolicy(chain);

            MockBuilderContext context = new MockBuilderContext();

            context.Strategies.Add(new LifetimeStrategy());

            // Not used in Win8
            //context.PersistentPolicies.SetDefault<IDynamicBuilderMethodCreatorPolicy>(
            //    DynamicBuilderMethodCreatorFactory.CreatePolicy());
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
            var policy = new ContainerControlledLifetimeManager();
            policy.SetValue(value);
            context.PersistentPolicies.Set<ILifetimePolicy>(policy, new NamedTypeBuildKey(t));
        }

        public class ObjectWithInjectionMethod
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

        public class ObjectWithGenericInjectionMethod
        {
            [InjectionMethod]
            public void DoSomethingGeneric<T>(T input)
            {

            }
        }

        public class ObjectWithOutParamMethod
        {
            [InjectionMethod]
            public void DoSomething(out int result)
            {
                result = 42;
            }
        }

        public class ObjectWithRefParamMethod
        {
            [InjectionMethod]
            public void DoSomething(ref int result)
            {
                result *= 2;
            }
        }

        public class ObjectWithSingleInjectionMethod
        {
            [InjectionMethod]
            public void InjectionMethod(object parameter)
            {
            }
        }

        public class ObjectWithSingleThrowingInjectionMethod
        {
            public static Exception injectionMethodException = new ArgumentException();

            [InjectionMethod]
            public void InjectionMethod(object parameter)
            {
                throw injectionMethodException;
            }
        }

        public class TestSingleArgumentMethodSelectorPolicy<T> : IMethodSelectorPolicy
        {
            private IDependencyResolverPolicy resolverPolicy;

            public TestSingleArgumentMethodSelectorPolicy(IDependencyResolverPolicy resolverPolicy)
            {
                this.resolverPolicy = resolverPolicy;
            }

            public IEnumerable<SelectedMethod> SelectMethods(IBuilderContext context, IPolicyList resolverPolicyDestination)
            {
                var key = Guid.NewGuid().ToString();
                resolverPolicyDestination.Set<IDependencyResolverPolicy>(this.resolverPolicy, key);
                var method =
                    new SelectedMethod(
                        // .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)[0]
                        typeof(T).GetTypeInfo().DeclaredMethods.First(m => m.IsPublic && !m.IsStatic));
                method.AddParameterKey(key);

                yield return method;
            }
        }

    }
}
