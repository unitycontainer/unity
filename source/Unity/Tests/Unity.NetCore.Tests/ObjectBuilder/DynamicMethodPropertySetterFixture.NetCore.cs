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

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public class DynamicMethodPropertySetterFixture
    {
        [TestMethod]
        public void CanInjectProperties()
        {
            MockBuilderContext context = GetContext();
            object existingObject = new object();
            var lifetimePolicy = new ContainerControlledLifetimeManager();
            lifetimePolicy.SetValue(existingObject);
            context.Policies.Set<ILifetimePolicy>(lifetimePolicy, new NamedTypeBuildKey<object>());

            IBuildPlanPolicy plan =
                GetPlanCreator(context).CreatePlan(context, new NamedTypeBuildKey(typeof(OnePropertyClass)));

            OnePropertyClass existing = new OnePropertyClass();
            context.Existing = existing;
            context.BuildKey = new NamedTypeBuildKey(typeof(OnePropertyClass));
            plan.BuildUp(context);

            Assert.IsNotNull(existing.Key);
            Assert.AreSame(existingObject, existing.Key);
        }

        [TestMethod]
        public void TheCurrentOperationIsNullAfterSuccessfullyExecutingTheBuildPlan()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<OnePropertyClass>();
            context.BuildKey = key;
            context.Existing = new OnePropertyClass();

            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, key);
            plan.BuildUp(context);

            Assert.IsNull(context.CurrentOperation);
        }

        [TestMethod]
        public void ResolvingAPropertyValueSetsTheCurrentOperation()
        {
            var resolverPolicy = new CurrentOperationSensingResolverPolicy<object>();

            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<OnePropertyClass>();
            context.BuildKey = key;
            context.Existing = new OnePropertyClass();

            context.Policies.Set<IPropertySelectorPolicy>(
                new TestSinglePropertySelectorPolicy<OnePropertyClass>(resolverPolicy),
                key);

            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, key);
            plan.BuildUp(context);

            Assert.IsNotNull(resolverPolicy.currentOperation);
        }

        [TestMethod]
        public void ExceptionThrownWhileResolvingAPropertyValueIsBubbledUpAndTheCurrentOperationIsNotCleared()
        {
            var exception = new ArgumentException();
            var resolverPolicy = new ExceptionThrowingTestResolverPolicy(exception);

            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<OnePropertyClass>();
            context.BuildKey = key;
            context.Existing = new OnePropertyClass();

            context.Policies.Set<IPropertySelectorPolicy>(
                new TestSinglePropertySelectorPolicy<OnePropertyClass>(resolverPolicy),
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

                var operation = (ResolvingPropertyValueOperation) context.CurrentOperation;
                Assert.IsNotNull(operation);
                Assert.AreSame(typeof(OnePropertyClass), operation.TypeBeingConstructed);
                Assert.AreEqual("Key", operation.PropertyName);
            }
        }

        [TestMethod]
        public void ExceptionThrownWhileSettingAPropertyIsBubbledUpAndTheCurrentOperationIsNotCleared()
        {
            MockBuilderContext context = GetContext();
            var key = new NamedTypeBuildKey<OneExceptionThrowingPropertyClass>();
            context.BuildKey = key;
            context.Existing = new OneExceptionThrowingPropertyClass();

            IBuildPlanPolicy plan =
                GetPlanCreator(context).CreatePlan(context, key);

            try
            {
                plan.BuildUp(context);
                Assert.Fail("failure expected");
            }
            catch (Exception e)
            {
                Assert.AreSame(OneExceptionThrowingPropertyClass.propertySetterException, e);
                var operation = (SettingPropertyOperation) context.CurrentOperation;
                Assert.IsNotNull(operation);

                Assert.AreSame(typeof(OneExceptionThrowingPropertyClass), operation.TypeBeingConstructed);
                Assert.AreEqual("Key", operation.PropertyName);
            }
        }

        private MockBuilderContext GetContext()
        {
            StagedStrategyChain<BuilderStage> chain = new StagedStrategyChain<BuilderStage>();
            chain.AddNew<DynamicMethodPropertySetterStrategy>(BuilderStage.Initialization);

            DynamicMethodBuildPlanCreatorPolicy policy =
                new DynamicMethodBuildPlanCreatorPolicy(chain);

            MockBuilderContext context = new MockBuilderContext();

            context.Strategies.Add(new LifetimeStrategy());

            // Not on Win8
            //context.PersistentPolicies.SetDefault<IDynamicBuilderMethodCreatorPolicy>(
            //    DynamicBuilderMethodCreatorFactory.CreatePolicy());

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

        public class TestSinglePropertySelectorPolicy<T> : IPropertySelectorPolicy
        {
            private IDependencyResolverPolicy resolverPolicy;

            public TestSinglePropertySelectorPolicy(IDependencyResolverPolicy resolverPolicy)
            {
                this.resolverPolicy = resolverPolicy;
            }

            public IEnumerable<SelectedProperty> SelectProperties(IBuilderContext context, IPolicyList resolverPolicyDestination)
            {
                yield return
                    new SelectedProperty(
                        typeof(T).GetTypeInfo().DeclaredProperties.First(),
                        this.resolverPolicy);
            }
        }

        public class OnePropertyClass
        {
            private object key;

            [Dependency]
            public object Key
            {
                get { return key; }
                set { key = value; }
            }
        }

        public class OneExceptionThrowingPropertyClass
        {
            public static Exception propertySetterException = new ArgumentException();

            [Dependency]
            public object Key
            {
                set { throw propertySetterException; }
            }
        }

        public interface IStillAnotherInterface
        {
        }

        public class ClassThatTakesInterface
        {
            [Dependency]
            public IStillAnotherInterface StillAnotherInterface
            {
                get { return null; }
                set { }
            }
        }
    }
}
