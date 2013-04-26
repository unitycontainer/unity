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
using Microsoft.Practices.ObjectBuilder2.Tests.TestObjects;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.TestSupport;
#if NETFX_CORE
using Microsoft.Practices.Unity.Utility;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using InjectionConstructorAttribute = Microsoft.Practices.ObjectBuilder2.Tests.TestDoubles.InjectionConstructorAttribute;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public class DynamicMethodConstructionFixture
    {
        [TestMethod]
        public void CanBuildUpObjectWithDefaultConstructorViaBuildPlan()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<NullLogger>();
            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, key);
            context.BuildKey = key;
            plan.BuildUp(context);

            object result = context.Existing;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NullLogger));
        }

        [TestMethod]
        public void CanResolveSimpleParameterTypes()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<FileLogger>();
            var lifetimePolicy = new ContainerControlledLifetimeManager();
            lifetimePolicy.SetValue("C:\\Log.txt");
            context.Policies.Set<ILifetimePolicy>(lifetimePolicy, new NamedTypeBuildKey<string>());

            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, key);
            context.BuildKey = key;

            plan.BuildUp(context);
            object result = context.Existing;
            FileLogger logger = result as FileLogger;

            Assert.IsNotNull(result);
            Assert.IsNotNull(logger);
            Assert.AreEqual("C:\\Log.txt", logger.LogFile);
        }

        [TestMethod]
        public void TheCurrentOperationIsNullAfterSuccessfullyExecutingTheBuildPlan()
        {
            MockBuilderContext context = GetContext();
            context.BuildKey = new NamedTypeBuildKey(typeof(ConstructorInjectionTestClass));

            context.Policies.Set<IConstructorSelectorPolicy>(
                new TestSingleArgumentConstructorSelectorPolicy<ConstructorInjectionTestClass>(new CurrentOperationSensingResolverPolicy<object>()),
                typeof(ConstructorInjectionTestClass));

            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, context.BuildKey);
            plan.BuildUp(context);

            Assert.IsNull(context.CurrentOperation);
        }

        [TestMethod]
        public void ExceptionThrownWhileInvokingTheConstructorIsBubbledUpAndTheCurrentOperationIsNotCleared()
        {
            MockBuilderContext context = GetContext();
            context.BuildKey = new NamedTypeBuildKey(typeof(ThrowingConstructorInjectionTestClass));

            IBuildPlanPolicy plan =
                GetPlanCreator(context).CreatePlan(context, context.BuildKey);

            try
            {
                plan.BuildUp(context);
                Assert.Fail("failure expected");
            }
            catch (Exception e)
            {
                Assert.AreSame(ThrowingConstructorInjectionTestClass.constructorException, e);

                var operation = (InvokingConstructorOperation)context.CurrentOperation;
                Assert.IsNotNull(operation);
                Assert.AreSame(typeof(ThrowingConstructorInjectionTestClass), operation.TypeBeingConstructed);
            }
        }

        [TestMethod]
        public void ResolvingAParameterSetsTheCurrentOperation()
        {
            var resolverPolicy = new CurrentOperationSensingResolverPolicy<object>();

            MockBuilderContext context = GetContext();
            context.BuildKey = new NamedTypeBuildKey(typeof(ConstructorInjectionTestClass));

            context.Policies.Set<IConstructorSelectorPolicy>(
                new TestSingleArgumentConstructorSelectorPolicy<ConstructorInjectionTestClass>(resolverPolicy),
                new NamedTypeBuildKey(typeof(ConstructorInjectionTestClass)));

            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, context.BuildKey);
            plan.BuildUp(context);

            Assert.IsNotNull(resolverPolicy.currentOperation);
        }

        [TestMethod]
        public void ExceptionThrownWhileResolvingAParameterIsBubbledUpAndTheCurrentOperationIsNotCleared()
        {
            var exception = new ArgumentException();
            var resolverPolicy = new ExceptionThrowingTestResolverPolicy(exception);

            MockBuilderContext context = GetContext();
            context.BuildKey = new NamedTypeBuildKey(typeof(ConstructorInjectionTestClass));

            context.Policies.Set<IConstructorSelectorPolicy>(
                new TestSingleArgumentConstructorSelectorPolicy<ConstructorInjectionTestClass>(resolverPolicy),
                context.BuildKey);

            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, context.BuildKey);

            try
            {
                plan.BuildUp(context);
                Assert.Fail("failure expected");
            }
            catch (Exception e)
            {
                Assert.AreSame(exception, e);

                var operation = (ConstructorArgumentResolveOperation)context.CurrentOperation;
                Assert.IsNotNull(operation);

                Assert.AreSame(typeof(ConstructorInjectionTestClass), operation.TypeBeingConstructed);
                Assert.AreEqual("parameter", operation.ParameterName);
            }
        }

        [TestMethod]
        public void ResolvingANewInstanceOfATypeWithPrivateConstructorThrows()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<NoPublicConstructorInjectionTestClass>();
            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, key);
            context.BuildKey = key;

            try
            {
                plan.BuildUp(context);
                Assert.Fail("should have thrown");
            }
            catch (InvalidOperationException)
            {

            }
        }

        [TestMethod]
        public void CanBuildUpExistingObjectWithPrivateConstructor()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<NoPublicConstructorInjectionTestClass>();
            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, key);
            context.BuildKey = key;
            context.Existing = NoPublicConstructorInjectionTestClass.CreateInstance();
            plan.BuildUp(context);

            object result = context.Existing;
            Assert.IsNotNull(result);
        }

        public class TestSingleArgumentConstructorSelectorPolicy<T> : IConstructorSelectorPolicy
        {
            private IDependencyResolverPolicy parameterResolverPolicy;

            public TestSingleArgumentConstructorSelectorPolicy(IDependencyResolverPolicy parameterResolverPolicy)
            {
                this.parameterResolverPolicy = parameterResolverPolicy;
            }

            public SelectedConstructor SelectConstructor(IBuilderContext context, IPolicyList resolverPoliciesDestination)
            {
                var selectedConstructor = new SelectedConstructor(typeof(T).GetConstructor(new[] { typeof(object) }));
                var key = Guid.NewGuid().ToString();
                selectedConstructor.AddParameterKey(key);
                resolverPoliciesDestination.Set<IDependencyResolverPolicy>(this.parameterResolverPolicy, key);

                return selectedConstructor;
            }
        }

        private MockBuilderContext GetContext()
        {
            StagedStrategyChain<BuilderStage> chain = new StagedStrategyChain<BuilderStage>();
            chain.AddNew<DynamicMethodConstructorStrategy>(BuilderStage.Creation);

            DynamicMethodBuildPlanCreatorPolicy policy =
                new DynamicMethodBuildPlanCreatorPolicy(chain);

            MockBuilderContext context = new MockBuilderContext();

            context.Strategies.Add(new LifetimeStrategy());

            context.PersistentPolicies.SetDefault<IConstructorSelectorPolicy>(
                new ConstructorSelectorPolicy<InjectionConstructorAttribute>());
            context.PersistentPolicies.SetDefault<IBuildPlanCreatorPolicy>(policy);

            return context;
        }

        private IBuildPlanCreatorPolicy GetPlanCreator(IBuilderContext context)
        {
            return context.Policies.Get<IBuildPlanCreatorPolicy>(null);
        }

        public class ConstructorInjectionTestClass
        {
            public ConstructorInjectionTestClass(object parameter)
            {
            }
        }

        public class ThrowingConstructorInjectionTestClass
        {
            public static Exception constructorException = new ArgumentException();

            public ThrowingConstructorInjectionTestClass()
            {
                throw constructorException;
            }
        }

        public class NoPublicConstructorInjectionTestClass
        {
            private NoPublicConstructorInjectionTestClass() { }

            public static NoPublicConstructorInjectionTestClass CreateInstance()
            {
                return new NoPublicConstructorInjectionTestClass();
            }
        }
    }
}