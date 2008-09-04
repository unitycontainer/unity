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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity.TestSupport;

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
                .Configure<InjectedMembers>()
                    .ConfigureInjectionFor<TypeWithArrayConstructorParameter>(
                            new InjectionConstructor(new ResolvedArrayParameter<ILogger>())).Container
                .RegisterInstance<ILogger>("o1", o1)
                .RegisterInstance<ILogger>("o2", o2);

            TypeWithArrayConstructorParameter resolved = container.Resolve<TypeWithArrayConstructorParameter>();

            Assert.IsNotNull(resolved.loggers);
            Assert.AreEqual(2, resolved.loggers.Length);
            Assert.AreSame(o1, resolved.loggers[0]);
            Assert.AreSame(o2, resolved.loggers[1]);
        }

        [TestMethod]
        public void CanConfigureContainerToCallConstructorWithArrayParameterWithNonGenericVersion()
        {
            ILogger o1 = new MockLogger();
            ILogger o2 = new SpecialLogger();

            IUnityContainer container
                = new UnityContainer()
                .Configure<InjectedMembers>()
                    .ConfigureInjectionFor<TypeWithArrayConstructorParameter>(
                            new InjectionConstructor(new ResolvedArrayParameter(typeof(ILogger)))).Container
                .RegisterInstance<ILogger>("o1", o1)
                .RegisterInstance<ILogger>("o2", o2);

            TypeWithArrayConstructorParameter resolved = container.Resolve<TypeWithArrayConstructorParameter>();

            Assert.IsNotNull(resolved.loggers);
            Assert.AreEqual(2, resolved.loggers.Length);
            Assert.AreSame(o1, resolved.loggers[0]);
            Assert.AreSame(o2, resolved.loggers[1]);
        }

        [TestMethod]
        public void CanConfigureContainerToCallConstructorWithIEnumerableParameter()
        {
            ILogger o1 = new MockLogger();
            ILogger o2 = new SpecialLogger();

            IUnityContainer container
                = new UnityContainer()
                .Configure<InjectedMembers>()
                    .ConfigureInjectionFor<TypeWithIEnumerableConstructorParameter>(
                            new InjectionConstructor(new ResolvedArrayParameter<ILogger>())).Container
                .RegisterInstance<ILogger>("o1", o1)
                .RegisterInstance<ILogger>("o2", o2);

            TypeWithIEnumerableConstructorParameter resolved = container.Resolve<TypeWithIEnumerableConstructorParameter>();

            Assert.IsNotNull(resolved.loggers);
            List<ILogger> loggers = new List<ILogger>(resolved.loggers);
            Assert.AreEqual(2, loggers.Count);
            Assert.AreSame(o1, loggers[0]);
            Assert.AreSame(o2, loggers[1]);
        }

        [TestMethod]
        public void CanConfigureContainerToInjectSpecificValuesIntoAnArray()
        {
            ILogger logger2 = new SpecialLogger();

            IUnityContainer container = new UnityContainer()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<TypeWithArrayConstructorParameter>(
                new InjectionConstructor(
                    new ResolvedArrayParameter<ILogger>(
                        new ResolvedParameter<ILogger>("log1"),
                        typeof (ILogger),
                        logger2)))
                .Container
                .RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, SpecialLogger>("log1");

            TypeWithArrayConstructorParameter result = container.Resolve<TypeWithArrayConstructorParameter>();

            Assert.AreEqual(3, result.loggers.Length);
            Assert.IsInstanceOfType(result.loggers[0], typeof (SpecialLogger));
            Assert.IsInstanceOfType(result.loggers[1], typeof (MockLogger));
            Assert.AreSame(logger2, result.loggers[2]);
        }

        [TestMethod]
        public void CanConfigureContainerToInjectSpecificValuesIntoAnArrayWithNonGenericVersion()
        {
            ILogger logger2 = new SpecialLogger();

            IUnityContainer container = new UnityContainer()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<TypeWithArrayConstructorParameter>(
                new InjectionConstructor(
                    new ResolvedArrayParameter(
                        typeof(ILogger),
                        new ResolvedParameter<ILogger>("log1"),
                        typeof(ILogger),
                        logger2)))
                .Container
                .RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, SpecialLogger>("log1");

            TypeWithArrayConstructorParameter result = container.Resolve<TypeWithArrayConstructorParameter>();

            Assert.AreEqual(3, result.loggers.Length);
            Assert.IsInstanceOfType(result.loggers[0], typeof(SpecialLogger));
            Assert.IsInstanceOfType(result.loggers[1], typeof(MockLogger));
            Assert.AreSame(logger2, result.loggers[2]);
        }

        [TestMethod]
        public void CreatingResolvedArrayParameterWithValuesOfNonCompatibleType()
        {
            ILogger logger2 = new SpecialLogger();

            try
            {
                new ResolvedArrayParameter<ILogger>(
                    new ResolvedParameter<ILogger>("log1"),
                    typeof(int),
                    logger2);
                Assert.Fail("Should have thrown");
            }
            catch (InvalidOperationException)
            {
            }
        }


        public class TypeWithArrayConstructorParameter
        {
            public readonly ILogger[] loggers;

            public TypeWithArrayConstructorParameter(ILogger[] loggers)
            {
                this.loggers = loggers;
            }
        }

        public class TypeWithIEnumerableConstructorParameter
        {
            public readonly IEnumerable<ILogger> loggers;

            public TypeWithIEnumerableConstructorParameter(IEnumerable<ILogger> loggers)
            {
                this.loggers = loggers;
            }
        }
    }
}
