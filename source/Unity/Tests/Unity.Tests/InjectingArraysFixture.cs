// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
    [TestClass]
    public class InjectingArraysFixture
    {
        [TestMethod]
        public void CanConfigureContainerToCallConstructorWithArrayParameter()
        {
            ILogger o1 = new MockLogger();
            ILogger o2 = new SpecialLogger();

            IUnityContainer container
                = new UnityContainer()
                .RegisterType<TypeWithArrayConstructorParameter>(
                    new InjectionConstructor(typeof(ILogger[])))
                .RegisterInstance<ILogger>("o1", o1)
                .RegisterInstance<ILogger>("o2", o2);

            TypeWithArrayConstructorParameter resolved = container.Resolve<TypeWithArrayConstructorParameter>();

            Assert.IsNotNull(resolved.Loggers);
            Assert.AreEqual(2, resolved.Loggers.Length);
            Assert.AreSame(o1, resolved.Loggers[0]);
            Assert.AreSame(o2, resolved.Loggers[1]);
        }

        [TestMethod]
        public void CanConfigureContainerToCallConstructorWithArrayParameterWithNonGenericVersion()
        {
            ILogger o1 = new MockLogger();
            ILogger o2 = new SpecialLogger();

            IUnityContainer container = new UnityContainer()
                .RegisterType<TypeWithArrayConstructorParameter>(new InjectionConstructor(typeof(ILogger[])))
                .RegisterInstance<ILogger>("o1", o1)
                .RegisterInstance<ILogger>("o2", o2);

            TypeWithArrayConstructorParameter resolved = container.Resolve<TypeWithArrayConstructorParameter>();

            Assert.IsNotNull(resolved.Loggers);
            Assert.AreEqual(2, resolved.Loggers.Length);
            Assert.AreSame(o1, resolved.Loggers[0]);
            Assert.AreSame(o2, resolved.Loggers[1]);
        }

        [TestMethod]
        public void CanConfigureContainerToInjectSpecificValuesIntoAnArray()
        {
            ILogger logger2 = new SpecialLogger();

            IUnityContainer container = new UnityContainer()
                .RegisterType<TypeWithArrayConstructorParameter>(
                new InjectionConstructor(
                    new ResolvedArrayParameter<ILogger>(
                        new ResolvedParameter<ILogger>("log1"),
                        typeof(ILogger),
                        logger2)))
                .RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, SpecialLogger>("log1");

            TypeWithArrayConstructorParameter result = container.Resolve<TypeWithArrayConstructorParameter>();

            Assert.AreEqual(3, result.Loggers.Length);
            Assert.IsInstanceOfType(result.Loggers[0], typeof(SpecialLogger));
            Assert.IsInstanceOfType(result.Loggers[1], typeof(MockLogger));
            Assert.AreSame(logger2, result.Loggers[2]);
        }

        [TestMethod]
        public void CanConfigureContainerToInjectSpecificValuesIntoAnArrayWithNonGenericVersion()
        {
            ILogger logger2 = new SpecialLogger();

            IUnityContainer container = new UnityContainer()
                .RegisterType<TypeWithArrayConstructorParameter>(
                new InjectionConstructor(
                    new ResolvedArrayParameter(
                        typeof(ILogger),
                        new ResolvedParameter<ILogger>("log1"),
                        typeof(ILogger),
                        logger2)))
                .RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, SpecialLogger>("log1");

            TypeWithArrayConstructorParameter result = container.Resolve<TypeWithArrayConstructorParameter>();

            Assert.AreEqual(3, result.Loggers.Length);
            Assert.IsInstanceOfType(result.Loggers[0], typeof(SpecialLogger));
            Assert.IsInstanceOfType(result.Loggers[1], typeof(MockLogger));
            Assert.AreSame(logger2, result.Loggers[2]);
        }

        [TestMethod]
        public void CreatingResolvedArrayParameterWithValuesOfNonCompatibleType()
        {
            ILogger logger2 = new SpecialLogger();

            AssertExtensions.AssertException<InvalidOperationException>(() =>
                {
                    new ResolvedArrayParameter<ILogger>(
                        new ResolvedParameter<ILogger>("log1"),
                        typeof(int),
                        logger2);
                });
        }

        [TestMethod]
        public void ContainerAutomaticallyResolvesAllWhenInjectingArrays()
        {
            ILogger[] expected = new ILogger[] { new MockLogger(), new SpecialLogger() };
            IUnityContainer container = new UnityContainer()
                .RegisterInstance("one", expected[0])
                .RegisterInstance("two", expected[1]);

            TypeWithArrayConstructorParameter result = container.Resolve<TypeWithArrayConstructorParameter>();

            CollectionAssert.AreEqual(expected, result.Loggers);
        }

        [TestMethod]
        public void ContainerAutomaticallyResolvesAllWhenInjectingGenericArrays()
        {
            ILogger[] expected = new ILogger[] { new MockLogger(), new SpecialLogger() };
            IUnityContainer container = new UnityContainer()
                .RegisterInstance("one", expected[0])
                .RegisterInstance("two", expected[1])
                .RegisterType(typeof(GenericTypeWithArrayProperty<>),
                    new InjectionProperty("Prop"));

            var result = container.Resolve<GenericTypeWithArrayProperty<ILogger>>();
            result.Prop.AssertContainsInAnyOrder(expected[0], expected[1]);
        }

        public class TypeWithArrayConstructorParameter
        {
            public readonly ILogger[] Loggers;

            public TypeWithArrayConstructorParameter(ILogger[] loggers)
            {
                this.Loggers = loggers;
            }
        }

        public class GenericTypeWithArrayProperty<T>
        {
            public T[] Prop { get; set; }
        }
    }
}
