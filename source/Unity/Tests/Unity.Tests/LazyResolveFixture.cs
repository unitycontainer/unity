// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity.TestSupport;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.Tests
{
    /// <summary>
    /// Summary description for DeferredResolveFixture
    /// </summary>
    [TestClass]
    public class LazyResolveFixture
    {
        [TestMethod]
        public void CanResolveALazy()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            var lazy = container.Resolve<Lazy<ILogger>>();
            Assert.IsNotNull(lazy);
        }

        [TestMethod]
        public void ResolvedLazyHasNoValue()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            var lazy = container.Resolve<Lazy<ILogger>>();
            Assert.IsFalse(lazy.IsValueCreated);
        }

        [TestMethod]
        public void ResolvedLazyResolvesThroughContainer()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            var lazy = container.Resolve<Lazy<ILogger>>();
            var logger = lazy.Value;

            AssertExtensions.IsInstanceOfType(logger, typeof(MockLogger));
        }

        [TestMethod]
        public void ResolvedLazyGetsInjectedAsADependency()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            var result = container.Resolve<ObjectThatGetsALazy>();

            Assert.IsNotNull(result.LoggerLazy);
            AssertExtensions.IsInstanceOfType(result.LoggerLazy.Value, typeof(MockLogger));
        }

        [TestMethod]
        public void CanResolveLazyWithName()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, SpecialLogger>("special");

            var lazy = container.Resolve<Lazy<ILogger>>("special");

            Assert.IsNotNull(lazy);
        }

        [TestMethod]
        public void ResolvedLazyWithNameResolvedThroughContainerWithName()
        {
            var container = new UnityContainer();

            var lazy = container.Resolve<Lazy<ILogger>>("special");

            container
                .RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, SpecialLogger>("special");

            var result = lazy.Value;

            Assert.IsNotNull(result);
            AssertExtensions.IsInstanceOfType(result, typeof(SpecialLogger));
        }

        [TestMethod]
        public void DifferentResolveCallsReturnDifferentLazyInstances()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            var lazy1 = container.Resolve<Lazy<ILogger>>();
            var lazy2 = container.Resolve<Lazy<ILogger>>();

            Assert.AreNotSame(lazy1.Value, lazy2.Value);
        }

        [TestMethod]
        public void DifferentLazyGenericsGetTheirOwnBuildPlan()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>()
                .RegisterInstance<string>("the instance");

            var lazy1 = container.Resolve<Lazy<ILogger>>();
            var lazy2 = container.Resolve<Lazy<string>>();

            AssertExtensions.IsInstanceOfType(lazy1.Value, typeof(ILogger));
            Assert.AreEqual("the instance", lazy2.Value);
        }

        [TestMethod]
        public void ObservesPerResolveSingleton()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>()
                .RegisterType(typeof(Lazy<>), new PerResolveLifetimeManager());

            var result = container.Resolve<ObjectThatGetsMultipleLazy>();

            Assert.IsNotNull(result.LoggerLazy1);
            Assert.IsNotNull(result.LoggerLazy2);
            Assert.AreSame(result.LoggerLazy1, result.LoggerLazy2);
            AssertExtensions.IsInstanceOfType(result.LoggerLazy1.Value, typeof(MockLogger));
            AssertExtensions.IsInstanceOfType(result.LoggerLazy2.Value, typeof(MockLogger));
            Assert.AreSame(result.LoggerLazy1.Value, result.LoggerLazy2.Value);

            Assert.AreNotSame(result.LoggerLazy1.Value, container.Resolve<Lazy<ILogger>>().Value);
        }

        [TestMethod]
        public void ResolvingLazyOfIEnumerableCallsResolveAll()
        {
            var container = new UnityContainer()
                .RegisterInstance("one", "first")
                .RegisterInstance("two", "second")
                .RegisterInstance("three", "third");

            var lazy = container.Resolve<Lazy<IEnumerable<string>>>();
            var result = lazy.Value;

            result.AssertContainsInAnyOrder("first", "second", "third");
        }

        public class ObjectThatGetsALazy
        {
            [Dependency]
            public Lazy<ILogger> LoggerLazy { get; set; }
        }

        public class ObjectThatGetsMultipleLazy
        {
            [Dependency]
            public Lazy<ILogger> LoggerLazy1 { get; set; }

            [Dependency]
            public Lazy<ILogger> LoggerLazy2 { get; set; }
        }
    }
}
