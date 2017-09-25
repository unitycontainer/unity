// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ObjectBuilder2;
using Unity.ObjectBuilder;
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

namespace Unity.Tests
{
     
    public class ResolvingArraysFixture
    {
        [Fact]
        public void ContainerCanResolveListOfT()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType(typeof(List<>), new InjectionConstructor());

            var result = container.Resolve<List<EmptyClass>>();

            Assert.NotNull(result);
        }

        [Fact]
        public void ContainerReturnsEmptyArrayIfNoObjectsRegistered()
        {
            IUnityContainer container = new UnityContainer();
            List<object> results = new List<object>(container.ResolveAll<object>());

            Assert.NotNull(results);
            CollectionAssertExtensions.AreEqual(new object[0], results);
        }

        [Fact]
        public void ResolveAllReturnsRegisteredObjects()
        {
            IUnityContainer container = new UnityContainer();
            object o1 = new object();
            object o2 = new object();

            container
                .RegisterInstance<object>("o1", o1)
                .RegisterInstance<object>("o2", o2);

            List<object> results = new List<object>(container.ResolveAll<object>());

            CollectionAssertExtensions.AreEqual(new object[] { o1, o2 }, results);
        }

        [Fact]
        public void ResolveAllReturnsRegisteredObjectsForBaseClass()
        {
            IUnityContainer container = new UnityContainer();
            ILogger o1 = new MockLogger();
            ILogger o2 = new SpecialLogger();

            container
                .RegisterInstance<ILogger>("o1", o1)
                .RegisterInstance<ILogger>("o2", o2);

            List<ILogger> results = new List<ILogger>(container.ResolveAll<ILogger>());
            CollectionAssertExtensions.AreEqual(new ILogger[] { o1, o2 }, results);
        }

        [Fact]
        public void ResolverWithElementsReturnsEmptyArrayIfThereAreNoElements()
        {
            IUnityContainer container = new UnityContainer();
            object o1 = new object();
            object o2 = new object();
            object o3 = new object();

            container
                .RegisterInstance<object>("o1", o1)
                .RegisterInstance<object>("o2", o2);

            BuilderContext context = GetContext(container, new NamedTypeBuildKey<object>());

            ResolvedArrayWithElementsResolverPolicy resolver
                = new ResolvedArrayWithElementsResolverPolicy(typeof(object));

            object[] results = (object[])resolver.Resolve(context);

            Assert.NotNull(results);
            Assert.Equal(0, results.Length);
        }

        [Fact]
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

            object[] results = (object[])container.Resolve<InjectedObject>().InjectedValue;

            Assert.NotNull(results);
            Assert.Equal(2, results.Length);
            Assert.Same(o1, results[0]);
            Assert.Same(o3, results[1]);
        }

        [Fact]
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

            object[] results = (object[])container.Resolve<InjectedObject>().InjectedValue;

            Assert.NotNull(results);
            Assert.Equal(2, results.Length);
            Assert.Same(o1, results[0]);
            Assert.Same(o2, results[1]);
        }

        [Fact]
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

            ILogger[] results = (ILogger[])container.Resolve<InjectedObject>().InjectedValue;

            Assert.NotNull(results);
            Assert.Equal(2, results.Length);
            Assert.Same(o1, results[0]);
            Assert.Same(o2, results[1]);
        }

        private BuilderContext GetContext(IUnityContainer container, NamedTypeBuildKey buildKey)
        {
            StrategyChain strategies = new StrategyChain();
            strategies.Add(new ReturnContainerStrategy(container));
            PolicyList persistentPolicies = new PolicyList();
            PolicyList transientPolicies = new PolicyList(persistentPolicies);
            return new BuilderContext(null, strategies, null, persistentPolicies, transientPolicies, buildKey, null);
        }

        private class InjectedObjectConfigurationExtension : UnityContainerExtension
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

        private class InjectedObjectSelectorPolicy : IConstructorSelectorPolicy
        {
            private readonly IDependencyResolverPolicy resolverPolicy;

            public InjectedObjectSelectorPolicy(IDependencyResolverPolicy resolverPolicy)
            {
                this.resolverPolicy = resolverPolicy;
            }

            public SelectedConstructor SelectConstructor(IBuilderContext context, IPolicyList resolverPoliciesDestination)
            {
                var ctr = typeof(InjectedObject).GetMatchingConstructor(new[] { typeof(object) });
                var selectedConstructor = new SelectedConstructor(ctr);
                selectedConstructor.AddParameterResolver(this.resolverPolicy);

                return selectedConstructor;
            }
        }

        public class InjectedObject
        {
            public readonly object InjectedValue;

            public InjectedObject(object injectedValue)
            {
                this.InjectedValue = injectedValue;
            }
        }

        public class EmptyClass
        {
        }
    }

    internal class ReturnContainerStrategy : BuilderStrategy
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
                context.Existing = this.container;
                context.BuildComplete = true;
            }
        }
    }
}
