// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Linq;
using System.Reflection;
using DifferentInterfacesInDifferentNamespace;
using Microsoft.Practices.Unity;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using RegistrationByConventionAssembly1;
using RegistrationByConventionAssembly2;

namespace Unity.Tests.RegistrationByConvention
{
    [TestClass]
    public class RegistrationByConventionFixture
    {
        private const string RegistrationByConventionAssembly1Name = "RegistrationByConventionAssembly1";
        private const string RegistrationByConventionAssembly2Name = "RegistrationByConventionAssembly2";

        [TestMethod]
        public void CanResolveMappedInterfacesFromAnAssemblyWithTypeNames()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(AllClasses.FromAssemblies(Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name))),
                WithMappings.FromAllInterfacesInSameAssembly,
                WithName.TypeName,
                null);

            var resolved1 = container.Resolve<ITypeImplementingI1>("TypeImplementingI1");
            var resolved2 = container.Resolve<IInterface2>("TypeImplementingI2");
            var resolved3 = container.Resolve<ITypeImplementingI1>("TypeImplementingI12");
            var resolved4 = container.Resolve<IInterface2>("TypeImplementingI12");

            Assert.IsInstanceOfType(resolved1, typeof(TypeImplementingI1));
            Assert.IsInstanceOfType(resolved2, typeof(TypeImplementingI2));
            Assert.IsInstanceOfType(resolved3, typeof(TypeImplementingI12));
            Assert.IsInstanceOfType(resolved4, typeof(TypeImplementingI12));
        }

        [TestMethod]
        public void WhenRegisteringMappedInterfacesFromAnAssemblyWithTypeNamesInterfaceRegistrationNameIsNotMismatched()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name))),
                WithMappings.FromAllInterfacesInSameAssembly,
                WithName.TypeName,
                null);

            AssertHelper.ThrowsException<ResolutionFailedException>(() => container.Resolve<ITypeImplementingI1>("TypeImplementingI2"));
        }

        [TestMethod]
        public void WhenRegisteringMappedInterfacesFromAnAssemblyWithTypeNamesInterfaceRegistrationNameIsNotMismatched2()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name))),
                WithMappings.FromAllInterfacesInSameAssembly,
                WithName.TypeName,
                null);

            AssertHelper.ThrowsException<ResolutionFailedException>(() => container.Resolve<IInterface2>("TypeImplementingI1"));
        }

        [TestMethod]
        public void WhenMultipleMappingsToSameInterfaceLastOneWinsWins()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name))),
                WithMappings.FromAllInterfacesInSameAssembly,
                WithName.Default,
                null,
                null,
                true);

            var resolved1 = container.Resolve<ITypeImplementingI1>();
            var resolved2 = container.Resolve<IInterface2>();

            Assert.IsInstanceOfType(resolved1, typeof(TypeImplementingI12));
            Assert.IsInstanceOfType(resolved2, typeof(TypeImplementingI12));
        }

        [TestMethod]
        public void WhenMultipleMappingsToSameInterfaceRegistrationNameIsNotMismatched()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name))),
                WithMappings.FromAllInterfacesInSameAssembly,
                WithName.Default,
                null,
                null,
                true);

            AssertHelper.ThrowsException<ResolutionFailedException>(() => container.Resolve<ITypeImplementingI1>("TypeImplementingI1"));
        }

        [TestMethod]
        public void WhenMultipleMappingsToSameInterfaceRegistrationNameIsNotMismatched2()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name))),
                WithMappings.FromAllInterfacesInSameAssembly,
                WithName.Default,
                null,
                null,
                true);

            AssertHelper.ThrowsException<ResolutionFailedException>(() => container.Resolve<IInterface2>("TypeImplementingI2"));
        }

        [TestMethod]
        public void WhenMappingToInterfacesInSameAssemblyWithDefaultNames()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name))),
                WithMappings.FromAllInterfacesInSameAssembly,
                WithName.Default,
                null,
                null,
                true);

            var resolved1 = container.Resolve<IInterface3>();
            var resolved2 = container.Resolve<IInterface4>();

            Assert.IsInstanceOfType(resolved1, typeof(TypeImplementingI3));
            Assert.IsInstanceOfType(resolved2, typeof(TypeImplementingI4));
        }

        [TestMethod]
        public void WhenMappingToInterfaceIndifferentAsseembly()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name)),
                Assembly.Load(new AssemblyName(RegistrationByConventionAssembly2Name))),
                WithMappings.FromAllInterfaces,
                WithName.TypeName,
                null);

            var resolved = container.Resolve<ITypeImplementingI1>("TypeToRegisterInDifferentAssembly");

            Assert.IsInstanceOfType(resolved, typeof(TypeToRegisterInDifferentAssembly));
        }

        [TestMethod]
        public void CanLoadFromMultipleAssemblies()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name)),
                Assembly.Load(new AssemblyName(RegistrationByConventionAssembly2Name))),
                WithMappings.FromAllInterfaces,
                WithName.TypeName,
                null);

            var resolved1 = container.Resolve<ITypeImplementingI1>("TypeToRegisterInDifferentAssembly");
            var resolved2 = container.Resolve<ITypeImplementingI1>("TypeImplementingI1");
            var resolved3 = container.Resolve<IInterface2>("TypeImplementingI2");

            Assert.IsInstanceOfType(resolved1, typeof(TypeToRegisterInDifferentAssembly));
            Assert.IsInstanceOfType(resolved2, typeof(TypeImplementingI1));
            Assert.IsInstanceOfType(resolved3, typeof(TypeImplementingI2));
        }

        [TestMethod]
        public void WhenFilteringToInterfaceInSameAssembly()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(
                    Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name)),
                    Assembly.Load(new AssemblyName(RegistrationByConventionAssembly2Name))),
                WithMappings.FromAllInterfacesInSameAssembly,
                WithName.TypeName,
                null);

            AssertHelper.ThrowsException<ResolutionFailedException>(() => container.Resolve<ITypeImplementingI1>("TypeToRegisterInDifferentAssembly"));
        }

        [TestMethod]
        public void WhenFilteringToMatchingInterface()
        {
            IUnityContainer container = new UnityContainer();

            var mappings = WithMappings.FromMatchingInterface(typeof(TypeImplementing8));

            container.RegisterTypes(
                AllClasses.FromAssemblies(Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name))),
                WithMappings.FromMatchingInterface,
                WithName.TypeName,
                WithLifetime.Transient,
                null,
                true);

            var resolved = container.Resolve<ITypeImplementing8>("TypeImplementing8");

            Assert.IsInstanceOfType(resolved, typeof(TypeImplementing8));
        }

        [TestMethod]
        public void WhenFilteringToNoTypes()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(
                    Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name)),
                    Assembly.Load(new AssemblyName(RegistrationByConventionAssembly2Name))),
                (t) => { return WithMappings.None(typeof(TypeImplementingI12)); },
                WithName.TypeName,
                null);

            foreach (var reg in container.Registrations)
            {
                System.Diagnostics.Debug.WriteLine(reg.Name);
            }

            var resolved = container.Resolve<TypeImplementingI12>("TypeImplementingI12");

            Assert.IsInstanceOfType(resolved, typeof(TypeImplementingI12));

            AssertHelper.ThrowsException<ResolutionFailedException>(() => container.Resolve<ITypeImplementingI1>("TypeImplementingI12"));
        }

        [TestMethod]
        public void WhenMappedToIsNull()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(
                    Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name)),
                    Assembly.Load(new AssemblyName(RegistrationByConventionAssembly2Name))),
                    null,
                    WithName.TypeName,
                    WithLifetime.ContainerControlled);

            foreach (var reg in container.Registrations)
            {
                System.Diagnostics.Debug.WriteLine(reg.Name);
            }

            var resolved = container.Resolve<TypeImplementingI12>("TypeImplementingI12");

            Assert.IsInstanceOfType(resolved, typeof(TypeImplementingI12));

            AssertHelper.ThrowsException<ResolutionFailedException>(() => container.Resolve<ITypeImplementingI1>("TypeImplementingI12"));
        }

        [TestMethod]
        public void WhenMappedToIsNullWithNoLifeTime()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(
                    Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name)),
                    Assembly.Load(new AssemblyName(RegistrationByConventionAssembly2Name))),
                null,
                WithName.TypeName,
                null);

            Assert.AreEqual<int>(1, container.Registrations.Count());
        }

        [TestMethod]
        public void WhenMappedToTypeHasNoInterface()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(
                    Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name)),
                    Assembly.Load(new AssemblyName(RegistrationByConventionAssembly2Name))),
                (t) => WithMappings.FromAllInterfaces(typeof(TypeWithNoInterface)),
                WithName.TypeName,
                null);

            Assert.AreEqual<int>(1, container.Registrations.Count());
        }

        [TestMethod]
        public void WhenMappedToTypeHasNoInterfaceAndMappedToNoType()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(
                    Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name)),
                    Assembly.Load(new AssemblyName(RegistrationByConventionAssembly2Name))),
                (t) => WithMappings.None(typeof(TypeWithNoInterface)),
                WithName.TypeName,
                null);

            var resolved = container.Resolve<TypeWithNoInterface>("TypeWithNoInterface");

            Assert.IsInstanceOfType(resolved, typeof(TypeWithNoInterface));
        }

        [TestMethod]
        public void WhenMappedToAllInterfacesIsNull()
        {
            IUnityContainer container = new UnityContainer();

            AssertHelper.ThrowsException<ArgumentNullException>(() => container.RegisterTypes(
                AllClasses.FromAssemblies(
                    Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name)),
                    Assembly.Load(new AssemblyName(RegistrationByConventionAssembly2Name))),
                (t) => WithMappings.FromAllInterfaces((null)),
                WithName.TypeName,
                WithLifetime.ContainerControlled));
        }

        [TestMethod]
        public void WhenMappedToInterfacesInSameAssemblyIsNull()
        {
            IUnityContainer container = new UnityContainer();

            AssertHelper.ThrowsException<ArgumentNullException>(() => container.RegisterTypes(
                AllClasses.FromAssemblies(
                    Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name)),
                    Assembly.Load(new AssemblyName(RegistrationByConventionAssembly2Name))),
                (t) => WithMappings.FromAllInterfacesInSameAssembly((null)),
                WithName.TypeName,
                WithLifetime.ContainerControlled));
        }

        [TestMethod]
        public void WhenRegisteringFromAssemblies()
        {
            var container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name))),
                WithMappings.FromAllInterfaces,
                WithName.TypeName,
                null);

            var reg = container.Resolve<IInterface3>("TypeImplementingI3");

            Assert.IsInstanceOfType(reg, typeof(TypeImplementingI3));

            AssertHelper.ThrowsException<ResolutionFailedException>(() => container.Resolve<ITypeImplementingI1>("TypeToRegisterInDifferentAssembly"));
        }

        #region Withlifetime
        [TestMethod]
        public void WithLifetimeNotSetIsTransient()
        {
            var container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name))),
                WithMappings.FromAllInterfaces,
                WithName.TypeName,
                null);

            var reg = container.Resolve<IInterface3>("TypeImplementingI3");
            var reg1 = container.Resolve<IInterface3>("TypeImplementingI3");

            Assert.AreNotSame(reg, reg1);
        }

        [TestMethod]
        public void WithLifetimeTransient()
        {
            var container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name))),
                WithMappings.FromAllInterfaces,
                WithName.TypeName,
                WithLifetime.Transient);

            var reg = container.Resolve<IInterface3>("TypeImplementingI3");
            var reg1 = container.Resolve<IInterface3>("TypeImplementingI3");

            Assert.AreNotSame(reg, reg1);
        }

        [TestMethod]
        public void WithLifetimeContainerControlled()
        {
            var container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name))),
                WithMappings.FromAllInterfaces,
                WithName.TypeName,
                WithLifetime.ContainerControlled);

            var reg = container.Resolve<IInterface3>("TypeImplementingI3");
            var reg1 = container.Resolve<IInterface3>("TypeImplementingI3");

            Assert.AreSame(reg, reg1);
        }

        [TestMethod]
        public void WithLifetimeExternallyControlled()
        {
            var container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromAssemblies(Assembly.Load(new AssemblyName(RegistrationByConventionAssembly1Name))),
                WithMappings.FromAllInterfaces,
                WithName.TypeName,
                WithLifetime.ExternallyControlled);

            var reg = container.Resolve<IInterface3>("TypeImplementingI3");
            var reg1 = container.Resolve<IInterface3>("TypeImplementingI3");

            Assert.AreSame(reg, reg1);
            reg = null;
            reg1 = null;
            LifetimeManager lifetimeManager = null;

            foreach (var registration in container.Registrations)
            {
                if (registration == null || registration.Name == null)
                {
                    continue;
                }

                if (registration.Name.Contains("TypeImplementingI3"))
                {
                    lifetimeManager = registration.LifetimeManager;
                }
            }

            Assert.IsInstanceOfType(lifetimeManager, typeof(ExternallyControlledLifetimeManager));
        }

        #endregion
    }
}
