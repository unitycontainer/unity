// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Summary description for DeferredResolveFixture
    /// </summary>
     
    public class DeferredResolveFixture
    {
        [Fact]
        public void CanResolveAFunc()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            var resolver = container.Resolve<Func<ILogger>>();
            Assert.NotNull(resolver);
        }

        [Fact]
        public void ResolvedFuncResolvesThroughContainer()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            var resolver = container.Resolve<Func<ILogger>>();
            var logger = resolver();

            AssertExtensions.IsInstanceOfType(logger, typeof(MockLogger));
        }

        [Fact]
        public void ResolvedFuncGetsInjectedAsADependency()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            var result = container.Resolve<ObjectThatGetsAResolver>();

            Assert.NotNull(result.LoggerResolver);
            AssertExtensions.IsInstanceOfType(result.LoggerResolver(), typeof(MockLogger));
        }

        [Fact]
        public void CanResolveFuncWithName()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, SpecialLogger>("special");

            var resolver = container.Resolve<Func<ILogger>>("special");

            Assert.NotNull(resolver);
        }

        [Fact]
        public void ResolvedFuncWithNameResolvedThroughContainerWithName()
        {
            var container = new UnityContainer();

            var resolver = container.Resolve<Func<ILogger>>("special");

            container.RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, SpecialLogger>("special");

            var result = resolver();

            Assert.NotNull(result);
            AssertExtensions.IsInstanceOfType(result, typeof(SpecialLogger));
        }

        [Fact]
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
