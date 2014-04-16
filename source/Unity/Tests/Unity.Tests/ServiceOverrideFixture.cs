// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.ObjectBuilder;
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
    /// Test cases around service overrides and passing parameters to
    /// the container's Resolve method.
    /// </summary>
    [TestClass]
    public class ServiceOverrideFixture
    {
        [TestMethod]
        public void CanProvideConstructorParameterViaResolveCall()
        {
            const int ConfiguredValue = 15; // Just need a number, value has no signficance.
            const int ExpectedValue = 42; // Just need a number, value has no significance.
            var container = new UnityContainer()
                .RegisterType<SimpleTestObject>(new InjectionConstructor(ConfiguredValue));

            var result =
                container.Resolve<SimpleTestObject>(new ParameterOverride("x", ExpectedValue));

            Assert.AreEqual(ExpectedValue, result.X);
        }

        [TestMethod]
        public void OverrideDoesntLastAfterResolveCall()
        {
            const int ConfiguredValue = 15; // Just need a number, value has no signficance.
            const int OverrideValue = 42; // Just need a number, value has no significance.
            var container = new UnityContainer()
                .RegisterType<SimpleTestObject>(new InjectionConstructor(ConfiguredValue));

            container.Resolve<SimpleTestObject>(new ParameterOverride("x", OverrideValue).OnType<SimpleTestObject>());

            var result = container.Resolve<SimpleTestObject>();

            Assert.AreEqual(ConfiguredValue, result.X);
        }

        [TestMethod]
        public void OverrideIsUsedInRecursiveBuilds()
        {
            const int ExpectedValue = 42; // Just need a number, value has no significance.
            var container = new UnityContainer();

            var result = container.Resolve<ObjectThatDependsOnSimpleObject>(
                new ParameterOverride("x", ExpectedValue));

            Assert.AreEqual(ExpectedValue, result.TestObject.X);
        }

        [TestMethod]
        public void NonMatchingOverridesAreIgnored()
        {
            const int ExpectedValue = 42; // Just need a number, value has no significance.
            var container = new UnityContainer();

            var result = container.Resolve<SimpleTestObject>(
                new ParameterOverrides 
                {
                    { "y", ExpectedValue * 2 },
                    { "x", ExpectedValue } 
                }.OnType<SimpleTestObject>());

            Assert.AreEqual(ExpectedValue, result.X);
        }

        [TestMethod]
        public void DependencyOverrideOccursEverywhereTypeMatches()
        {
            var container = new UnityContainer()
                .RegisterType<ObjectThatDependsOnSimpleObject>(
                    new InjectionProperty("OtherTestObject"))
                .RegisterType<SimpleTestObject>(new InjectionConstructor());

            var overrideValue = new SimpleTestObject(15); // arbitrary value

            var result = container.Resolve<ObjectThatDependsOnSimpleObject>(
                new DependencyOverride<SimpleTestObject>(overrideValue));

            Assert.AreSame(overrideValue, result.TestObject);
            Assert.AreSame(overrideValue, result.OtherTestObject);
        }

        [TestMethod]
        public void ParameterOverrideMatchesWhenCurrentOperationIsResolvingMatchingParameter()
        {
            var context = new MockBuilderContext
            {
                CurrentOperation = new ConstructorArgumentResolveOperation(typeof(SimpleTestObject), "int x", "x")
            };

            var overrider = new ParameterOverride("x", 42);

            var resolver = overrider.GetResolver(context, typeof(int));

            Assert.IsNotNull(resolver);
            AssertExtensions.IsInstanceOfType(resolver, typeof(LiteralValueDependencyResolverPolicy));

            var result = (int)resolver.Resolve(context);
            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public void ParameterOverrideCanResolveOverride()
        {
            var container = new UnityContainer()
                .RegisterType<ISomething, Something1>()
                .RegisterType<ISomething, Something2>("other");

            var result = container.Resolve<ObjectTakingASomething>(
                new ParameterOverride("something", new ResolvedParameter<ISomething>("other")));

            AssertExtensions.IsInstanceOfType(result.MySomething, typeof(Something2));
        }

        [TestMethod]
        public void CanOverridePropertyValue()
        {
            var container = new UnityContainer()
                .RegisterType<ObjectTakingASomething>(
                    new InjectionConstructor(),
                    new InjectionProperty("MySomething"))
                .RegisterType<ISomething, Something1>()
                .RegisterType<ISomething, Something2>("other");

            var result = container.Resolve<ObjectTakingASomething>(
                new PropertyOverride("MySomething", new ResolvedParameter<ISomething>("other")).OnType<ObjectTakingASomething>());

            Assert.IsNotNull(result.MySomething);
            AssertExtensions.IsInstanceOfType(result.MySomething, typeof(Something2));
        }

        [TestMethod]
        public void PropertyValueOverrideForTypeDifferentThanResolvedTypeIsIgnored()
        {
            var container = new UnityContainer()
                .RegisterType<ObjectTakingASomething>(
                    new InjectionConstructor(),
                    new InjectionProperty("MySomething"))
                .RegisterType<ISomething, Something1>()
                .RegisterType<ISomething, Something2>("other");

            var result = container.Resolve<ObjectTakingASomething>(
                new PropertyOverride("MySomething", new ResolvedParameter<ISomething>("other")).OnType<ObjectThatDependsOnSimpleObject>());

            Assert.IsNotNull(result.MySomething);
            AssertExtensions.IsInstanceOfType(result.MySomething, typeof(Something1));
        }

        [TestMethod]
        public void CanOverridePropertyValueWithNullWithExplicitInjectionParameter()
        {
            var container = new UnityContainer()
                .RegisterType<ObjectTakingASomething>(
                    new InjectionConstructor(),
                    new InjectionProperty("MySomething"))
                .RegisterType<ISomething, Something1>()
                .RegisterType<ISomething, Something2>("other");

            var result = container.Resolve<ObjectTakingASomething>(
                new PropertyOverride("MySomething", new InjectionParameter<ISomething>(null)).OnType<ObjectTakingASomething>());

            Assert.IsNull(result.MySomething);
        }

        [TestMethod]
        public void CreatingAPropertyOverrideForANullValueThrows()
        {
            AssertExtensions.AssertException<ArgumentNullException>(() =>
                {
                    new PropertyOverride("ignored", null);
                });
        }

        [TestMethod]
        public void CanOverrideDependencyWithExplicitInjectionParameterValue()
        {
            var container = new UnityContainer()
                .RegisterType<Outer>(new InjectionConstructor(typeof(Inner), 10))
                .RegisterType<Inner>(new InjectionConstructor(20, "ignored"));

            // resolves overriding only the parameter for the Bar instance

            var instance = container.Resolve<Outer>(new DependencyOverride<int>(new InjectionParameter(50)).OnType<Inner>());

            Assert.AreEqual(10, instance.LogLevel);
            Assert.AreEqual(50, instance.Inner.LogLevel);
        }

        [TestMethod]
        public void CreatingATypeOverrideForANullTypeThrows()
        {
            AssertExtensions.AssertException<ArgumentNullException>(() =>
                {
                    new TypeBasedOverride(null, new PropertyOverride("ignored", 10));
                });
        }

        [TestMethod]
        public void CreatingATypeOverrideForANullOverrideThrows()
        {
            AssertExtensions.AssertException<ArgumentNullException>(() =>
                {
                    new TypeBasedOverride(typeof(object), null);
                });
        }

        public class SimpleTestObject
        {
            public SimpleTestObject()
            {
            }

            public SimpleTestObject(int x)
            {
                X = x;
            }

            public int X { get; private set; }
        }

        public class ObjectThatDependsOnSimpleObject
        {
            public SimpleTestObject TestObject { get; set; }

            public ObjectThatDependsOnSimpleObject(SimpleTestObject testObject)
            {
                TestObject = testObject;
            }

            public SimpleTestObject OtherTestObject { get; set; }
        }

        public interface ISomething { }
        public class Something1 : ISomething { }
        public class Something2 : ISomething { }

        public class ObjectTakingASomething
        {
            public ISomething MySomething { get; set; }
            public ObjectTakingASomething()
            {
            }

            public ObjectTakingASomething(ISomething something)
            {
                MySomething = something;
            }
        }

        public class Outer
        {
            public Outer(Inner inner, int logLevel)
            {
                this.Inner = inner;
                this.LogLevel = logLevel;
            }

            public int LogLevel { get; private set; }
            public Inner Inner { get; private set; }
        }

        public class Inner
        {
            public Inner(int logLevel, string label)
            {
                this.LogLevel = logLevel;
            }

            public int LogLevel { get; private set; }
        }
    }
}
