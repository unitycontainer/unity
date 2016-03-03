// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using ObjectBuilder2.Tests.TestDoubles;
using ObjectBuilder2.Tests.TestObjects;
using Unity;
using Unity.TestSupport;
using Unity.Utility;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using InjectionConstructorAttribute = ObjectBuilder2.Tests.TestDoubles.InjectionConstructorAttribute;
#elif __IOS__
using NUnit.Framework;
using InjectionConstructorAttribute = ObjectBuilder2.Tests.TestDoubles.InjectionConstructorAttribute;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#else
using Xunit;
using InjectionConstructorAttribute = ObjectBuilder2.Tests.TestDoubles.InjectionConstructorAttribute;
#endif

namespace ObjectBuilder2.Tests
{
     
    public class DynamicMethodConstructionFixture
    {
        [Fact]
        public void CanBuildUpObjectWithDefaultConstructorViaBuildPlan()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<NullLogger>();
            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, key);
            context.BuildKey = key;
            plan.BuildUp(context);

            object result = context.Existing;
            Assert.NotNull(result);
            AssertExtensions.IsInstanceOfType(result, typeof(NullLogger));
        }

        [Fact]
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

            Assert.NotNull(result);
            Assert.NotNull(logger);
            Assert.Equal("C:\\Log.txt", logger.LogFile);
        }

        [Fact]
        public void TheCurrentOperationIsNullAfterSuccessfullyExecutingTheBuildPlan()
        {
            MockBuilderContext context = GetContext();
            context.BuildKey = new NamedTypeBuildKey(typeof(ConstructorInjectionTestClass));

            context.Policies.Set<IConstructorSelectorPolicy>(
                new TestSingleArgumentConstructorSelectorPolicy<ConstructorInjectionTestClass>(new CurrentOperationSensingResolverPolicy<object>()),
                typeof(ConstructorInjectionTestClass));

            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, context.BuildKey);
            plan.BuildUp(context);

            Assert.Null(context.CurrentOperation);
        }

        [Fact]
        public void ExceptionThrownWhileInvokingTheConstructorIsBubbledUpAndTheCurrentOperationIsNotCleared()
        {
            MockBuilderContext context = GetContext();
            context.BuildKey = new NamedTypeBuildKey(typeof(ThrowingConstructorInjectionTestClass));

            IBuildPlanPolicy plan =
                GetPlanCreator(context).CreatePlan(context, context.BuildKey);

            try
            {
                plan.BuildUp(context);
                Assert.True(false, string.Format("failure expected"));
            }
            catch (Exception e)
            {
                Assert.Same(ThrowingConstructorInjectionTestClass.ConstructorException, e);

                var operation = (InvokingConstructorOperation)context.CurrentOperation;
                Assert.NotNull(operation);
                Assert.Same(typeof(ThrowingConstructorInjectionTestClass), operation.TypeBeingConstructed);
            }
        }

        [Fact]
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

            Assert.NotNull(resolverPolicy.CurrentOperation);
        }

        [Fact]
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
                Assert.True(false, string.Format("failure expected"));
            }
            catch (Exception e)
            {
                Assert.Same(exception, e);

                var operation = (ConstructorArgumentResolveOperation)context.CurrentOperation;
                Assert.NotNull(operation);

                Assert.Same(typeof(ConstructorInjectionTestClass), operation.TypeBeingConstructed);
                Assert.Equal("parameter", operation.ParameterName);
            }
        }

        [Fact]
        public void ResolvingANewInstanceOfATypeWithPrivateConstructorThrows()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<NoPublicConstructorInjectionTestClass>();
            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, key);
            context.BuildKey = key;

            try
            {
                plan.BuildUp(context);
                Assert.True(false, string.Format("should have thrown"));
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Fact]
        public void ResolvingANewInstanceOfADelegateTypeThrows()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<Func<string, object>>();
            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, key);
            context.BuildKey = key;

            try
            {
                plan.BuildUp(context);
                Assert.True(false, string.Format("should have thrown"));
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Fact]
        public void CanResolveAADelegateTypeIfInstanceExists()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<Func<string, object>>();
            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, key);
            context.BuildKey = key;
            Func<string, object> existing = s => null;
            context.Existing = existing;

            plan.BuildUp(context);
            Assert.Same(existing, context.Existing);
        }

        [Fact]
        public void ResolvingANewInstanceOfAnInterfaceTypeThrows()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<IComparable>();
            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, key);
            context.BuildKey = key;

            try
            {
                plan.BuildUp(context);
                Assert.True(false, string.Format("should have thrown"));
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Fact]
        public void CanBuildUpExistingObjectWithPrivateConstructor()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<NoPublicConstructorInjectionTestClass>();
            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, key);
            context.BuildKey = key;
            context.Existing = NoPublicConstructorInjectionTestClass.CreateInstance();
            plan.BuildUp(context);

            object result = context.Existing;
            Assert.NotNull(result);
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
                var selectedConstructor = new SelectedConstructor(typeof(T).GetConstructorInfo(new[] { typeof(object) }));
                selectedConstructor.AddParameterResolver(this.parameterResolverPolicy);

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
            public static Exception ConstructorException = new ArgumentException();

            public ThrowingConstructorInjectionTestClass()
            {
                throw ConstructorException;
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
