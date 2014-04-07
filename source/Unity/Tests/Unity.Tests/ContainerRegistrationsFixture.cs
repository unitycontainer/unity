// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
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

namespace Microsoft.Practices.Unity.Tests
{
    /// <summary>
    /// Summary description for ContainerRegistrationsFixture
    /// </summary>
    [TestClass]
    public class ContainerRegistrationsFixture
    {
        private IUnityContainer container;

        [TestInitialize]
        public void Setup()
        {
            container = new UnityContainer();
        }

        [TestMethod]
        public void ContainerListsItselfAsRegistered()
        {
            Assert.IsTrue(container.IsRegistered(typeof(IUnityContainer)));
        }

        [TestMethod]
        public void ContainerDoesNotListItselfUnderNonDefaultName()
        {
            Assert.IsFalse(container.IsRegistered(typeof(IUnityContainer), "other"));
        }

        [TestMethod]
        public void ContainerListsItselfAsRegisteredUsingGenericOverload()
        {
            Assert.IsTrue(container.IsRegistered<IUnityContainer>());
        }

        [TestMethod]
        public void ContainerDoesNotListItselfUnderNonDefaultNameUsingGenericOverload()
        {
            Assert.IsFalse(container.IsRegistered<IUnityContainer>("other"));
        }

        [TestMethod]
        public void IsRegisteredWorksForRegisteredType()
        {
            container.RegisterType<ILogger, MockLogger>();

            Assert.IsTrue(container.IsRegistered<ILogger>());
        }

        [TestMethod]
        public void ContainerIncludesItselfUnderRegistrations()
        {
            Assert.IsNotNull(container.Registrations.Where(r => r.RegisteredType == typeof(IUnityContainer)).FirstOrDefault());
        }

        [TestMethod]
        public void NewRegistrationsShowUpInRegistrationsSequence()
        {
            container.RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, MockLogger>("second");

            var registrations = (from r in container.Registrations
                                 where r.RegisteredType == typeof(ILogger)
                                 select r).ToList();

            Assert.AreEqual(2, registrations.Count);

            Assert.IsTrue(registrations.Any(r => r.Name == null));
            Assert.IsTrue(registrations.Any(r => r.Name == "second"));
        }

        [TestMethod]
        public void TypeMappingShowsUpInRegistrationsCorrectly()
        {
            container.RegisterType<ILogger, MockLogger>();

            var registration =
                (from r in container.Registrations where r.RegisteredType == typeof(ILogger) select r).First();
            Assert.AreSame(typeof(MockLogger), registration.MappedToType);
        }

        [TestMethod]
        public void NonMappingRegistrationShowsUpInRegistrationsSequence()
        {
            container.RegisterType<MockLogger>();
            var registration = (from r in container.Registrations
                                where r.RegisteredType == typeof(MockLogger)
                                select r).First();

            Assert.AreSame(registration.RegisteredType, registration.MappedToType);
            Assert.IsNull(registration.Name);
        }

        [TestMethod]
        public void RegistrationOfOpenGenericTypeShowsUpInRegistrationsSequence()
        {
            container.RegisterType(typeof(IDictionary<,>), typeof(Dictionary<,>), "test");
            var registration = container.Registrations.First(r => r.RegisteredType == typeof(IDictionary<,>));

            Assert.AreSame(typeof(Dictionary<,>), registration.MappedToType);
            Assert.AreEqual("test", registration.Name);
        }


        [TestMethod]
        public void RegistrationsInParentContainerAppearInChild()
        {
            container.RegisterType<ILogger, MockLogger>();
            var child = container.CreateChildContainer();

            var registration =
                (from r in child.Registrations where r.RegisteredType == typeof(ILogger) select r).First();

            Assert.AreSame(typeof(MockLogger), registration.MappedToType);
        }

        [TestMethod]
        public void RegistrationsInChildContainerDoNotAppearInParent()
        {
            var child = container.CreateChildContainer()
                .RegisterType<ILogger, MockLogger>("named");

            var childRegistration = child.Registrations.Where(r => r.RegisteredType == typeof(ILogger)).First();
            var parentRegistration =
                container.Registrations.Where(r => r.RegisteredType == typeof(ILogger)).FirstOrDefault();

            Assert.IsNull(parentRegistration);
            Assert.IsNotNull(childRegistration);
        }

        [TestMethod]
        public void DuplicateRegistrationsInParentAndChildOnlyShowUpOnceInChild()
        {
            container.RegisterType<ILogger, MockLogger>("one");

            var child = container.CreateChildContainer()
                .RegisterType<ILogger, SpecialLogger>("one");

            var registrations = from r in child.Registrations
                                where r.RegisteredType == typeof(ILogger)
                                select r;

            Assert.AreEqual(1, registrations.Count());

            var childRegistration = registrations.First();
            Assert.AreSame(typeof(SpecialLogger), childRegistration.MappedToType);
            Assert.AreEqual("one", childRegistration.Name);
        }
    }
}
