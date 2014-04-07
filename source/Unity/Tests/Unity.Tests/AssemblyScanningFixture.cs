// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity.TestSupport;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
using TestInitializeAttribute = NUnit.Framework.TestFixtureSetUpAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.Tests
{
    [TestClass]
    public partial class AssemblyScanningFixture
    {
        [TestMethod]
        public void DoesNotRegisterTypeWithNoLifetimeOrInjectionMembers()
        {
            var container = new UnityContainer();
            container.RegisterTypes(new[] { typeof(MockLogger) }, getName: t => "name");

            Assert.IsFalse(container.Registrations.Any(r => r.MappedToType == typeof(MockLogger)));
        }

        [TestMethod]
        public void RegisteringNullAssembliesListThrows()
        {
            AssertExtensions.AssertException<ArgumentNullException>(() =>
            {
                AllClasses.FromAssemblies(null, true);
            });
        }

        [TestMethod]
        public void RegisteringNullAssembliesArrayThrows()
        {
            AssertExtensions.AssertException<ArgumentNullException>(() =>
            {
                AllClasses.FromAssemblies(true, (Assembly[])null);
            });
        }

        [TestMethod]
        public void RegisteringNullAssemblyThrows()
        {
            AssertExtensions.AssertException<ArgumentException>(() =>
            {
                AllClasses.FromAssemblies(true, typeof(object).GetTypeInfo().Assembly, (Assembly)null);
            });
        }

        [TestMethod]
        public void RegistersTypeWithLifetime()
        {
            var container = new UnityContainer();
            container.RegisterTypes(new[] { typeof(MockLogger) }, getName: t => "name", getLifetimeManager: t => new ContainerControlledLifetimeManager());

            var registrations = container.Registrations.Where(r => r.MappedToType == typeof(MockLogger)).ToArray();

            Assert.AreEqual(1, registrations.Length);
            Assert.AreSame(typeof(MockLogger), registrations[0].MappedToType);
            Assert.AreEqual("name", registrations[0].Name);
            Assert.IsInstanceOfType(registrations[0].LifetimeManager, typeof(ContainerControlledLifetimeManager));
        }

        [TestMethod]
        public void RegistersTypeWithInjectionMembers()
        {
            var container = new UnityContainer();
            container.RegisterTypes(new[] { typeof(MockLogger) }, getName: t => "name", getInjectionMembers: t => new InjectionMember[] { new InjectionConstructor() });

            var registrations = container.Registrations.Where(r => r.MappedToType == typeof(MockLogger)).ToArray();

            Assert.AreEqual(1, registrations.Length);
            Assert.AreSame(typeof(MockLogger), registrations[0].RegisteredType);
            Assert.AreSame(typeof(MockLogger), registrations[0].MappedToType);
            Assert.AreEqual("name", registrations[0].Name);
            Assert.IsNull(registrations[0].LifetimeManager);
        }

        [TestMethod]
        public void RegistersMappingOnlyWithNoLifetimeOrInjectionMembers()
        {
            var container = new UnityContainer();
            container.RegisterTypes(new[] { typeof(MockLogger) }, getName: t => "name", getFromTypes: t => t.GetTypeInfo().ImplementedInterfaces);

            var registrations = container.Registrations.Where(r => r.MappedToType == typeof(MockLogger)).ToArray();

            Assert.AreEqual(1, registrations.Length);
            Assert.AreSame(typeof(ILogger), registrations[0].RegisteredType);
            Assert.AreSame(typeof(MockLogger), registrations[0].MappedToType);
            Assert.AreEqual("name", registrations[0].Name);
            Assert.IsNull(registrations[0].LifetimeManager);
        }

        [TestMethod]
        public void RegistersMappingAndImplementationTypeWithLifetime()
        {
            var container = new UnityContainer();
            container.RegisterTypes(new[] { typeof(MockLogger) }, getName: t => "name", getFromTypes: t => t.GetTypeInfo().ImplementedInterfaces, getLifetimeManager: t => new ContainerControlledLifetimeManager());

            var registrations = container.Registrations.Where(r => r.MappedToType == typeof(MockLogger)).ToArray();

            Assert.AreEqual(2, registrations.Length);

            var mappingRegistration = registrations.Single(r => r.RegisteredType == typeof(ILogger));
            var implementationRegistration = registrations.Single(r => r.RegisteredType == typeof(MockLogger));

            Assert.AreSame(typeof(ILogger), mappingRegistration.RegisteredType);
            Assert.AreSame(typeof(MockLogger), mappingRegistration.MappedToType);
            Assert.AreEqual("name", mappingRegistration.Name);
            Assert.IsInstanceOfType(mappingRegistration.LifetimeManager, typeof(ContainerControlledLifetimeManager));

            Assert.AreSame(typeof(MockLogger), implementationRegistration.RegisteredType);
            Assert.AreSame(typeof(MockLogger), implementationRegistration.MappedToType);
            Assert.AreEqual("name", implementationRegistration.Name);
            Assert.IsInstanceOfType(implementationRegistration.LifetimeManager, typeof(ContainerControlledLifetimeManager));
        }

        [TestMethod]
        public void GetsTypesFromAssembliesWithErrorsIfSkippingErrors()
        {
            var types = AllClasses.FromAssemblies(true, typeof(Microsoft.Practices.Unity.Tests.TestNetAssembly.PublicClass1).GetTypeInfo().Assembly).ToArray();

            Assert.AreEqual(2, types.Length);
            types.AssertContainsInAnyOrder(
                typeof(Microsoft.Practices.Unity.Tests.TestNetAssembly.PublicClass1),
                typeof(Microsoft.Practices.Unity.Tests.TestNetAssembly.DisposableClass));
        }

        [TestMethod]
        public void GettingTypesFromAssembliesWithErrorsThrowsIfNotIfNotSkippingErrors()
        {
            try
            {
                AllClasses.FromAssemblies(false, typeof(Microsoft.Practices.Unity.Tests.TestNetAssembly.PublicClass1).GetTypeInfo().Assembly).ToArray();
                Assert.Fail("should have failed");
            }
            catch (Exception e)
            {
                if (e is AssertFailedException)
                {
                    throw;
                }
            }
        }
    }
}