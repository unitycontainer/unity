// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Unity.Tests.ContainerRegistration
{
    [TestClass]
    public class GivenContainerRegistrationFixtureCorrectUsageFixture
    {
        private IUnityContainer container;

        [TestInitialize]
        public void Init()
        {
            container = new UnityContainer();
        }

        [TestMethod]
        public void WhenRegistrationsAreRetrievedFromAContainer()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var registrations = container.Registrations;

            var count = registrations.Count();

            Assert.AreEqual(3, count);

            var @default = registrations.SingleOrDefault(c => c.Name == null &&
                                                           c.RegisteredType == typeof(ITypeInterface));

            Assert.IsNotNull(@default);
            Assert.AreEqual(typeof(TypeImplementation), @default.MappedToType);

            var foo = registrations.SingleOrDefault(c => c.Name == "foo");

            Assert.IsNotNull(foo);
            Assert.AreEqual(typeof(TypeImplementation), @default.MappedToType);
        }

        [TestMethod]
        public void WhenRegistrationsAreRetrievedFromANestedContainer()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var child = container.CreateChildContainer();

            child.RegisterType<ITypeAnotherInterface, AnotherTypeImplementation>(new InjectionConstructor("default"));
            child.RegisterType<ITypeAnotherInterface, AnotherTypeImplementation>("another", new InjectionConstructor("another"));

            var registrations = container.Registrations;

            var count = registrations.Count();

            var childCount = child.Registrations.Count();

            Assert.AreEqual(3, count);
            Assert.AreEqual(5, childCount);

            var mappedCount = child.Registrations.Where(c => c.MappedToType == typeof(AnotherTypeImplementation)).Count();

            Assert.AreEqual(2, mappedCount);
        }

        [TestMethod]
        public void WhenDefaultRegistrationsAreRetrievedFromANestedContainer()
        {
            var child = container.CreateChildContainer();

            var registrations = container.Registrations;

            var count = registrations.Count();

            var childCount = child.Registrations.Count();

            Assert.AreEqual(1, count);
            Assert.AreEqual(1, childCount);

            var mappedCount = child.Registrations.Where(c => c.MappedToType == typeof(IUnityContainer)).Count();

            Assert.AreEqual(1, mappedCount);
        }

        [TestMethod]
        public void WhenRegistrationsAreRetrievedFromAContainerByLifeTimeManager()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new PerResolveLifetimeManager(), new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new PerResolveLifetimeManager(), new InjectionConstructor("foo"));

            var registrations = container.Registrations;

            var count = registrations.Where(c => c.LifetimeManagerType == typeof(PerResolveLifetimeManager)).Count();

            Assert.AreEqual(2, count);
        }
    }
}
