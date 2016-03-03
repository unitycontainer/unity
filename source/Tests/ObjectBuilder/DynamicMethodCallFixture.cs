// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using ObjectBuilder2.Tests.TestDoubles;
using ObjectBuilder2.Tests.TestObjects;
using Unity;
using Unity.TestSupport;
using Xunit;
using DependencyAttribute = ObjectBuilder2.Tests.TestDoubles.DependencyAttribute;
using InjectionConstructorAttribute = ObjectBuilder2.Tests.TestDoubles.InjectionConstructorAttribute;
using InjectionMethodAttribute = ObjectBuilder2.Tests.TestDoubles.InjectionMethodAttribute;

namespace ObjectBuilder2.Tests
{
     
    public class DynamicMethodCallFixture
    {
        [Fact]
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

            var existing = new ObjectWithInjectionMethod();

            context.BuildKey = key;
            context.Existing = existing;
            plan.BuildUp(context);

            GC.KeepAlive(intValue);
            GC.KeepAlive(stringValue);

            Assert.True(existing.WasInjected);
            Assert.Equal(intValue, existing.IntValue);
            Assert.Equal(stringValue, existing.StringValue);
        }

        [Fact]
        public void ThrowsWhenBuildingPlanWithGenericInjectionMethod()
        {
            try
            {
                MockBuilderContext context = GetContext();
                GetPlanCreator(context).CreatePlan(context, new NamedTypeBuildKey(typeof(ObjectWithGenericInjectionMethod)));
                Assert.True(false);
            }
            catch (IllegalInjectionMethodException)
            {
                // This is what we want
            }
        }

        [Fact]
        public void ThrowsWhenBuildingPlanWithMethodWithOutParam()
        {
            try
            {
                MockBuilderContext context = GetContext();
                GetPlanCreator(context).CreatePlan(context, new NamedTypeBuildKey(typeof(ObjectWithOutParamMethod)));
                Assert.True(false);
            }
            catch (IllegalInjectionMethodException)
            {
            }
        }

        [Fact]
        public void ThrowsWhenBuildingPlanWithMethodWithRefParam()
        {
            try
            {
                MockBuilderContext context = GetContext();
                GetPlanCreator(context).CreatePlan(context, new NamedTypeBuildKey(typeof(ObjectWithRefParamMethod)));
                Assert.True(false);
            }
            catch (IllegalInjectionMethodException)
            {
            }
        }

        [Fact]
        public void TheCurrentOperationIsNullAfterSuccessfullyExecutingTheBuildPlan()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<ObjectWithSingleInjectionMethod>();
            context.BuildKey = key;
            context.Existing = new ObjectWithSingleInjectionMethod();

            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, key);
            plan.BuildUp(context);

            Assert.Null(context.CurrentOperation);
        }

        [Fact]
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
                Assert.True(false, string.Format("failure expected"));
            }
            catch (Exception e)
            {
                Assert.Same(ObjectWithSingleThrowingInjectionMethod.InjectionMethodException, e);
                var operation = (InvokingMethodOperation)context.CurrentOperation;
                Assert.NotNull(operation);

                Assert.Same(typeof(ObjectWithSingleThrowingInjectionMethod), operation.TypeBeingConstructed);
            }
        }

        [Fact]
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

            Assert.NotNull(resolverPolicy.CurrentOperation);
        }

        [Fact]
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
                Assert.True(false, string.Format("failure expected"));
            }
            catch (Exception e)
            {
                Assert.Same(exception, e);
                var operation = (MethodArgumentResolveOperation)context.CurrentOperation;
                Assert.NotNull(operation);

                Assert.Same(typeof(ObjectWithSingleInjectionMethod), operation.TypeBeingConstructed);
                Assert.Equal("parameter", operation.ParameterName);
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
            public static Exception InjectionMethodException = new ArgumentException();

            [InjectionMethod]
            public void InjectionMethod(object parameter)
            {
                throw InjectionMethodException;
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
                var method =
                    new SelectedMethod(
                        typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)[0]);
                method.AddParameterResolver(this.resolverPolicy);

                yield return method;
            }
        }
    }
}
