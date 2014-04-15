// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
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
    public partial class RegistrationByConventionFixture
    {
        [TestMethod]
        public void DoesNotRegisterTypeWithNoLifetimeOrInjectionMembers()
        {
            var container = new UnityContainer();
            container.RegisterTypes(new[] { typeof(MockLogger) }, getName: t => "name");

            Assert.IsFalse(container.Registrations.Any(r => r.MappedToType == typeof(MockLogger)));
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
        public void RegistersMappingAndImplementationTypeWithLifetimeAndMixedInjectionMembers()
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
        public void RegistersUsingTheHelperMethods()
        {
            var container = new UnityContainer();
            container.RegisterTypes(AllClasses.FromAssemblies(typeof(MockLogger).GetTypeInfo().Assembly).Where(t => t == typeof(MockLogger)), WithMappings.FromAllInterfaces, WithName.Default, WithLifetime.ContainerControlled);
            var registrations = container.Registrations.Where(r => r.MappedToType == typeof(MockLogger)).ToArray();

            Assert.AreEqual(2, registrations.Length);

            var mappingRegistration = registrations.Single(r => r.RegisteredType == typeof(ILogger));
            var implementationRegistration = registrations.Single(r => r.RegisteredType == typeof(MockLogger));

            Assert.AreSame(typeof(ILogger), mappingRegistration.RegisteredType);
            Assert.AreSame(typeof(MockLogger), mappingRegistration.MappedToType);
            Assert.AreEqual(null, mappingRegistration.Name);
            Assert.IsInstanceOfType(mappingRegistration.LifetimeManager, typeof(ContainerControlledLifetimeManager));

            Assert.AreSame(typeof(MockLogger), implementationRegistration.RegisteredType);
            Assert.AreSame(typeof(MockLogger), implementationRegistration.MappedToType);
            Assert.AreEqual(null, implementationRegistration.Name);
            Assert.IsInstanceOfType(implementationRegistration.LifetimeManager, typeof(ContainerControlledLifetimeManager));
        }

#if !NETFX_CORE

        [TestMethod]
        public void RegistersAllTypesWithHelperMethods()
        {
            var container = new UnityContainer();
            container.RegisterTypes(AllClasses.FromLoadedAssemblies(), WithMappings.FromAllInterfaces, WithName.TypeName, WithLifetime.ContainerControlled, overwriteExistingMappings: true);
            var registrations = container.Registrations.Where(r => r.MappedToType == typeof(MockLogger)).ToArray();

            Assert.AreEqual(2, registrations.Length);

            var mappingRegistration = registrations.Single(r => r.RegisteredType == typeof(ILogger));
            var implementationRegistration = registrations.Single(r => r.RegisteredType == typeof(MockLogger));

            Assert.AreSame(typeof(ILogger), mappingRegistration.RegisteredType);
            Assert.AreSame(typeof(MockLogger), mappingRegistration.MappedToType);
            Assert.AreEqual("MockLogger", mappingRegistration.Name);
            Assert.IsInstanceOfType(mappingRegistration.LifetimeManager, typeof(ContainerControlledLifetimeManager));

            Assert.AreSame(typeof(MockLogger), implementationRegistration.RegisteredType);
            Assert.AreSame(typeof(MockLogger), implementationRegistration.MappedToType);
            Assert.AreEqual("MockLogger", implementationRegistration.Name);
            Assert.IsInstanceOfType(implementationRegistration.LifetimeManager, typeof(ContainerControlledLifetimeManager));
        }
#endif

        public void CanResolveTypeRegisteredWithAllInterfaces()
        {
            var container = new UnityContainer();
            container.RegisterTypes(AllClasses.FromAssemblies(typeof(MockLogger).GetTypeInfo().Assembly).Where(t => t == typeof(MockLogger)), WithMappings.FromAllInterfaces, WithName.Default, WithLifetime.ContainerControlled);

            var logger1 = container.Resolve<ILogger>();
            var logger2 = container.Resolve<ILogger>();

            Assert.IsInstanceOfType(logger1, typeof(MockLogger));
            Assert.AreSame(logger1, logger2);
        }

        public void CanResolveGenericTypeMappedWithMatchingInterface()
        {
            var container = new UnityContainer();
            container.RegisterTypes(AllClasses.FromAssemblies(typeof(IList<>).GetTypeInfo().Assembly), WithMappings.FromMatchingInterface, WithName.Default, WithLifetime.None);

            var list = container.Resolve<IList<int>>();

            Assert.IsInstanceOfType(list, typeof(List<int>));
        }

        [TestMethod]
        public void RegistersTypeAccordingToConvention()
        {
            var container = new UnityContainer();
            container.RegisterTypes(new TestConventionWithAllInterfaces());

            var registrations = container.Registrations.Where(r => r.MappedToType == typeof(MockLogger) || r.MappedToType == typeof(SpecialLogger)).ToArray();

            Assert.AreEqual(4, registrations.Length);
        }

        [TestMethod]
        public void OverridingExistingMappingWithDifferentMappingThrowsByDefault()
        {
            var container = new UnityContainer();
            container.RegisterType<object, string>();

            AssertExtensions.AssertException<DuplicateTypeMappingException>(
                () => container.RegisterTypes(new[] { typeof(int) }, t => new[] { typeof(object) }));
        }

        [TestMethod]
        public void OverridingNewMappingWithDifferentMappingThrowsByDefault()
        {
            var container = new UnityContainer();

            AssertExtensions.AssertException<DuplicateTypeMappingException>(
                () => container.RegisterTypes(new[] { typeof(string), typeof(int) }, t => new[] { typeof(object) }));
        }

        [TestMethod]
        public void OverridingExistingMappingWithSameMappingDoesNotThrow()
        {
            var container = new UnityContainer();
            container.RegisterInstance("a string");
            container.RegisterType<object, string>();

            container.RegisterTypes(new[] { typeof(string) }, t => new[] { typeof(object) });

            Assert.AreEqual("a string", container.Resolve<object>());
        }

        [TestMethod]
        public void CanOverrideExistingMappingWithMappingForDifferentName()
        {
            var container = new UnityContainer();
            container.RegisterType<object, string>("string");
            container.RegisterInstance("string", "a string");
            container.RegisterInstance("int", 42);

            container.RegisterTypes(new[] { typeof(int) }, t => new[] { typeof(object) }, t => "int");

            Assert.AreEqual("a string", container.Resolve<object>("string"));
            Assert.AreEqual(42, container.Resolve<object>("int"));
        }

        [TestMethod]
        public void OverridingExistingMappingWithDifferentMappingReplacesMappingIfAllowed()
        {
            var container = new UnityContainer();
            container.RegisterType<object, string>();
            container.RegisterInstance("a string");
            container.RegisterInstance(42);

            container.RegisterTypes(new[] { typeof(int) }, t => new[] { typeof(object) }, overwriteExistingMappings: true);

            Assert.AreEqual(42, container.Resolve<object>());
        }

        [TestMethod]
        public void OverridingNewMappingWithDifferentMappingReplacesMappingIfAllowed()
        {
            var container = new UnityContainer();
            container.RegisterInstance("a string");
            container.RegisterInstance(42);

            container.RegisterTypes(new[] { typeof(string), typeof(int) }, t => new[] { typeof(object) }, overwriteExistingMappings: true);

            Assert.AreEqual(42, container.Resolve<object>());
        }

        public class TestConventionWithAllInterfaces : RegistrationConvention
        {
            public override System.Collections.Generic.IEnumerable<System.Type> GetTypes()
            {
                yield return typeof(MockLogger);
                yield return typeof(SpecialLogger);
            }

            public override System.Func<System.Type, System.Collections.Generic.IEnumerable<System.Type>> GetFromTypes()
            {
                return t => t.GetTypeInfo().ImplementedInterfaces;
            }

            public override System.Func<System.Type, string> GetName()
            {
                return t => t.Name;
            }

            public override System.Func<System.Type, LifetimeManager> GetLifetimeManager()
            {
                return t => new ContainerControlledLifetimeManager();
            }

            public override System.Func<System.Type, System.Collections.Generic.IEnumerable<InjectionMember>> GetInjectionMembers()
            {
                return null;
            }
        }
    }
}