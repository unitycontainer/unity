// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.Unity.TestSupport;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public class BuildKeyMappingStrategyTest
    {
        [TestMethod]
        public void CanMapGenericsWithIdenticalGenericParameters()
        {
            MockBuilderContext context = new MockBuilderContext();
            context.Policies.Set<IBuildKeyMappingPolicy>(new GenericTypeBuildKeyMappingPolicy(
                new NamedTypeBuildKey(typeof(ConcreteType<>))),
                new NamedTypeBuildKey(typeof(ITestType<>)));
            BuildKeyMappingStrategy strategy = new BuildKeyMappingStrategy();
            context.Strategies.Add(strategy);
            SpyStrategy spy = new SpyStrategy();
            context.Strategies.Add(spy);
            context.BuildKey = new NamedTypeBuildKey<ITestType<int>>();
            context.Strategies.ExecuteBuildUp(context);

            Assert.AreEqual(new NamedTypeBuildKey(typeof(ConcreteType<int>)), spy.BuildKey);
        }

        [TestMethod]
        public void CanMapGenericsWithANonTypeBuildKey()
        {
            MockBuilderContext context = new MockBuilderContext();
            context.Policies.Set<IBuildKeyMappingPolicy>(
                new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(typeof(ConcreteType<>), "two")),
                new NamedTypeBuildKey(typeof(ITestType<>), "one"));

            BuildKeyMappingStrategy strategy = new BuildKeyMappingStrategy();
            context.Strategies.Add(strategy);
            SpyStrategy spy = new SpyStrategy();
            context.Strategies.Add(spy);
            context.BuildKey = new NamedTypeBuildKey(typeof(ITestType<int>), "one");
            context.Strategies.ExecuteBuildUp(context);

            AssertExtensions.IsInstanceOfType(spy.BuildKey, typeof(NamedTypeBuildKey));
            Assert.AreEqual(typeof(ConcreteType<int>), spy.BuildKey.Type);
            Assert.AreEqual("two", spy.BuildKey.Name);
        }

        [TestMethod]
        public void CanMapInterfacesToConcreteTypes()
        {
            MockBuilderContext context = new MockBuilderContext();
            context.Policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(new NamedTypeBuildKey<ConcreteType>()),
                new NamedTypeBuildKey<ITestType>());
            BuildKeyMappingStrategy strategy = new BuildKeyMappingStrategy();
            context.Strategies.Add(strategy);
            SpyStrategy spy = new SpyStrategy();
            context.Strategies.Add(spy);
            context.BuildKey = new NamedTypeBuildKey<ITestType>();
            context.Strategies.ExecuteBuildUp(context);

            Assert.AreEqual(new NamedTypeBuildKey(typeof(ConcreteType)), spy.BuildKey);
        }

        [TestMethod]
        public void MappingStrategyActuallyReturnsTheBuildKeyThePolicySpecifies()
        {
            MockBuilderContext context = new MockBuilderContext();
            NamedTypeBuildKey fromKey = new NamedTypeBuildKey(typeof(ConcreteType), "id");
            NamedTypeBuildKey toKey = new NamedTypeBuildKey(typeof(ITestType), "id");
            context.Policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(toKey), fromKey);
            BuildKeyMappingStrategy strategy = new BuildKeyMappingStrategy();
            context.Strategies.Add(strategy);
            SpyStrategy spy = new SpyStrategy();
            context.Strategies.Add(spy);
            context.BuildKey = fromKey;
            context.Existing = null;
            context.Strategies.ExecuteBuildUp(context);

            AssertExtensions.IsInstanceOfType(spy.BuildKey, typeof(NamedTypeBuildKey));
            Assert.AreEqual(toKey, spy.BuildKey);
        }

        private class ConcreteType : ITestType { }

        private class ConcreteType<T> : ITestType<T> { }

        private interface ITestType { }

        private interface ITestType<T> { }

        private class SpyStrategy : BuilderStrategy
        {
            public NamedTypeBuildKey BuildKey;

            public override void PreBuildUp(IBuilderContext context)
            {
                this.BuildKey = context.BuildKey;
            }
        }
    }
}
