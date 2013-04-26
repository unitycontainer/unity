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
using System.Collections.Generic;
using Microsoft.Practices.Unity.TestSupport;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.Tests
{
    /// <summary>
    /// Summary description for DeferredResolveFixture
    /// </summary>
    [TestClass]
    public class DeferredResolveFixture
    {
        [TestMethod]
        public void CanResolveAFunc()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            var resolver = container.Resolve<Func<ILogger>>();
            Assert.IsNotNull(resolver);
        }

        [TestMethod]
        public void ResolvedFuncResolvesThroughContainer()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            var resolver = container.Resolve<Func<ILogger>>();
            var logger = resolver();

            Assert.IsInstanceOfType(logger, typeof (MockLogger));
        }

        [TestMethod]
        public void ResolvedFuncGetsInjectedAsADependency()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            var result = container.Resolve<ObjectThatGetsAResolver>();

            Assert.IsNotNull(result.LoggerResolver);
            Assert.IsInstanceOfType(result.LoggerResolver(), typeof(MockLogger));
        }

        [TestMethod]
        public void CanResolveFuncWithName()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, SpecialLogger>("special");

            var resolver = container.Resolve<Func<ILogger>>("special");

            Assert.IsNotNull(resolver);
        }

        [TestMethod]
        public void ResolvedFuncWithNameResolvedThroughContainerWithName()
        {
            var container = new UnityContainer();

            var resolver = container.Resolve<Func<ILogger>>("special");

            container.RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, SpecialLogger>("special");

            var result = resolver();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(SpecialLogger));
        }

        [TestMethod]
        public void ResolvingFuncOfIEnumerableCallsResolveAll()
        {
            var container = new UnityContainer()
                .RegisterInstance("one", "first")
                .RegisterInstance("two", "second")
                .RegisterInstance("three", "third");

            var resolver = container.Resolve<Func<IEnumerable<string>>>();
            var result = resolver();

            result.AssertContainsInAnyOrder("first", "second", "third");
        }

        public class ObjectThatGetsAResolver
        {
            [Dependency]
            public Func<ILogger> LoggerResolver { get; set; }
        }
    }
}
