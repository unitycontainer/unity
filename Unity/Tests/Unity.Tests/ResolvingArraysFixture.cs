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

using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Microsoft.Practices.Unity.Tests
{
    [TestClass]
    public class ResolvingArraysFixture
    {
        [TestMethod]
        public void ResolverReturnsEmptyArrayIfNoObjectsRegistered()
        {
            IUnityContainer container = new UnityContainer();
            BuilderContext context = GetContext(container, typeof(object));

            ResolvedArrayResolverPolicy resolver = new ResolvedArrayResolverPolicy(typeof(object));

            object[] results = (object[])resolver.Resolve(context);

            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Length);
        }

        [TestMethod]
        public void ResolverReturnsRegisteredObjects()
        {
            IUnityContainer container = new UnityContainer();
            object o1 = new object();
            object o2 = new object();

            container
                .RegisterInstance<object>("o1", o1)
                .RegisterInstance<object>("o2", o2);

            BuilderContext context = GetContext(container, typeof(object));

            ResolvedArrayResolverPolicy resolver = new ResolvedArrayResolverPolicy(typeof(object));

            object[] results = (object[])resolver.Resolve(context);

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Length);
            Assert.AreSame(o1, results[0]);
            Assert.AreSame(o2, results[1]);
        }

        [TestMethod]
        public void ResolverReturnsRegisteredObjectsForBaseClass()
        {
            IUnityContainer container = new UnityContainer();
            ILogger o1 = new MockLogger();
            ILogger o2 = new SpecialLogger();

            container
                .RegisterInstance<ILogger>("o1", o1)
                .RegisterInstance<ILogger>("o2", o2);

            BuilderContext context = GetContext(container, typeof(object));

            ResolvedArrayResolverPolicy resolver = new ResolvedArrayResolverPolicy(typeof(ILogger));

            ILogger[] results = (ILogger[])resolver.Resolve(context);

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Length);
            Assert.AreSame(o1, results[0]);
            Assert.AreSame(o2, results[1]);
        }

        [TestMethod]
        public void ResolverWithElementsReturnsEmptyArrayIfThereAreNoElements()
        {
            IUnityContainer container = new UnityContainer();
            object o1 = new object();
            object o2 = new object();
            object o3 = new object();

            container
                .RegisterInstance<object>("o1", o1)
                .RegisterInstance<object>("o2", o2);

            BuilderContext context = GetContext(container, typeof(object));

            ResolvedArrayWithElementsResolverPolicy resolver
                = new ResolvedArrayWithElementsResolverPolicy(typeof(object));

            object[] results = (object[])resolver.Resolve(context);

            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Length);
        }

        [TestMethod]
        public void ResolverWithElementsReturnsLiteralElements()
        {
            IUnityContainer container = new UnityContainer();
            object o1 = new object();
            object o2 = new object();
            object o3 = new object();

            container
                .RegisterInstance<object>("o1", o1)
                .RegisterInstance<object>("o2", o2);

            ResolvedArrayWithElementsResolverPolicy resolver
                = new ResolvedArrayWithElementsResolverPolicy(
                    typeof(object),
                    new LiteralValueDependencyResolverPolicy(o1),
                    new LiteralValueDependencyResolverPolicy(o3));
            container.AddExtension(new InjectedObjectConfigurationExtension(resolver));

            object[] results = (object[])container.Resolve<InjectedObject>().injectedValue;

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Length);
            Assert.AreSame(o1, results[0]);
            Assert.AreSame(o3, results[1]);
        }

        [TestMethod]
        public void ResolverWithElementsReturnsResolvedElements()
        {
            IUnityContainer container = new UnityContainer();
            object o1 = new object();
            object o2 = new object();
            object o3 = new object();

            container
                .RegisterInstance<object>("o1", o1)
                .RegisterInstance<object>("o2", o2);

            ResolvedArrayWithElementsResolverPolicy resolver
                = new ResolvedArrayWithElementsResolverPolicy(
                    typeof(object),
                    new NamedTypeDependencyResolverPolicy(typeof(object), "o1"),
                    new NamedTypeDependencyResolverPolicy(typeof(object), "o2"));
            container.AddExtension(new InjectedObjectConfigurationExtension(resolver));

            object[] results = (object[])container.Resolve<InjectedObject>().injectedValue;

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Length);
            Assert.AreSame(o1, results[0]);
            Assert.AreSame(o2, results[1]);
        }

        [TestMethod]
        public void ResolverWithElementsReturnsResolvedElementsForBaseClass()
        {
            IUnityContainer container = new UnityContainer();
            ILogger o1 = new MockLogger();
            ILogger o2 = new SpecialLogger();

            container
                .RegisterInstance<ILogger>("o1", o1)
                .RegisterInstance<ILogger>("o2", o2);

            ResolvedArrayWithElementsResolverPolicy resolver
                = new ResolvedArrayWithElementsResolverPolicy(
                    typeof(ILogger),
                    new NamedTypeDependencyResolverPolicy(typeof(ILogger), "o1"),
                    new NamedTypeDependencyResolverPolicy(typeof(ILogger), "o2"));
            container.AddExtension(new InjectedObjectConfigurationExtension(resolver));

            ILogger[] results = (ILogger[])container.Resolve<InjectedObject>().injectedValue;

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Length);
            Assert.AreSame(o1, results[0]);
            Assert.AreSame(o2, results[1]);
        }

        private BuilderContext GetContext(IUnityContainer container, object buildKey)
        {
            StrategyChain strategies = new StrategyChain();
            strategies.Add(new ReturnContainerStrategy(container));
            PolicyList persistentPolicies = new PolicyList();
            PolicyList transientPolicies = new PolicyList(persistentPolicies);
            return new BuilderContext(strategies, null, null, persistentPolicies, transientPolicies, buildKey, null);
        }

        class InjectedObjectConfigurationExtension : UnityContainerExtension
        {
            private readonly IDependencyResolverPolicy resolverPolicy;

            public InjectedObjectConfigurationExtension(IDependencyResolverPolicy resolverPolicy)
            {
                this.resolverPolicy = resolverPolicy;
            }

            protected override void Initialize()
            {
                this.Context.Policies.Set<IConstructorSelectorPolicy>(
                    new InjectedObjectSelectorPolicy(this.resolverPolicy), NamedTypeBuildKey.Make<InjectedObject>());
            }
        }

        class InjectedObjectSelectorPolicy : IConstructorSelectorPolicy
        {
            private readonly IDependencyResolverPolicy resolverPolicy;

            public InjectedObjectSelectorPolicy(IDependencyResolverPolicy resolverPolicy)
            {
                this.resolverPolicy = resolverPolicy;
            }

            public SelectedConstructor SelectConstructor(IBuilderContext context)
            {
                SelectedConstructor selectedConstructor
                    = new SelectedConstructor(typeof(InjectedObject).GetConstructor(new Type[] { typeof(object) }));
                selectedConstructor.AddParameterKey("InjectedObject_injectedValue");
                context.PersistentPolicies.Set<IDependencyResolverPolicy>(this.resolverPolicy, "InjectedObject_injectedValue");

                return selectedConstructor;
            }
        }

        internal class InjectedObject
        {
            public readonly object injectedValue;

            public InjectedObject(object injectedValue)
            {
                this.injectedValue = injectedValue;
            }
        }
    }

    class ReturnContainerStrategy : BuilderStrategy
    {
        private IUnityContainer container;

        public ReturnContainerStrategy(IUnityContainer container)
        {
            this.container = container;
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            if ((NamedTypeBuildKey)context.BuildKey == NamedTypeBuildKey.Make<IUnityContainer>())
            {
                context.Existing = container;
                context.BuildComplete = true;
            }
        }
    }
}
