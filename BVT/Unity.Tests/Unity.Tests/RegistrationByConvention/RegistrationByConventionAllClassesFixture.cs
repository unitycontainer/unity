// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Reflection;
using DifferentInterfacesInDifferentNamespace;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegistrationByConventionAssembly1;
using RegistrationByConventionAssembly2;

namespace Unity.Tests.RegistrationByConvention
{
    [TestClass]
    public class RegistrationByConventionAllClassesFixture
    {
        [TestMethod]
        [DeploymentItem("AssemblyToLoad.dll")]
        public void WhenRegisteringFromLoadedTypes()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromLoadedAssemblies(),
                WithMappings.FromAllInterfaces,
                WithName.TypeName, null, null, true);

            foreach (var registration in container.Registrations)
            {
                if (registration.Name == null)
                {
                    continue;
                }

                Assert.IsFalse(registration.Name.Contains("TypeInAssemblyToLoad"));
            }

            Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "\\AssemblyToLoad.dll");
            container.RegisterTypes(
                AllClasses.FromLoadedAssemblies(),
                WithMappings.FromAllInterfaces,
                WithName.TypeName,
                null,
                null,
                true);

            bool found = false;
            foreach (var registration in container.Registrations)
            {
                if (registration.Name == null)
                {
                    continue;
                }

                if (registration.Name.Contains("TypeInAssemblyToLoad"))
                {
                    found = true;
                }
            }

            Assert.IsTrue(found);
        }

        [TestMethod]
        public void WhenSomeTypesCannotBeLoadedAndSkipFlagIsFalse()
        {
            IUnityContainer container = new UnityContainer();

            AssertHelper.ThrowsException<DuplicateTypeMappingException>(() =>
            {
                return container.RegisterTypes(
                    AllClasses.FromAssembliesInBasePath(false, false),
                    WithMappings.FromAllInterfaces,
                    WithName.TypeName,
                    null,
                    null,
                    false);
            });
        }

        [TestMethod]
        public void WhenSomeTypesCannotBeLoadedAndSkipFlagIsTrue()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterTypes(
                AllClasses.FromAssembliesInBasePath(false, false, true),
                WithMappings.FromAllInterfaces,
                WithName.TypeName,
                null,
                null,
                true);

            var resolved1 = container.Resolve<IInterface3>("TypeImplementingI3");
            var resolved2 = container.Resolve<ITypeImplementingI1>("TypeToRegisterInDifferentAssembly");

            Assert.IsInstanceOfType(resolved1, typeof(TypeImplementingI3));
            Assert.IsInstanceOfType(resolved2, typeof(TypeToRegisterInDifferentAssembly));
        }

        [TestMethod]
        public void WhenRegisteringFromLoadedTypesAndIncludeUnityAssemblies()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterTypes(
                AllClasses.FromLoadedAssemblies(includeUnityAssemblies: true),
                WithMappings.FromAllInterfaces,
                WithName.TypeName,
                null,
                null,
                true);

            bool isunityFound = false;
            foreach (var registration in container.Registrations)
            {
                if (registration.Name == null)
                {
                    continue;
                }

                if (registration.Name.Contains("StrategyChain"))
                {
                    isunityFound = true;
                }
            }

            Assert.IsTrue(isunityFound);
        }

        [TestMethod]
        [DeploymentItem("AssemblyWithUnityInterface.dll")]
        public void WhenRegisteringFromLoadedTypesAndSkipUnityAssemblies_FindIInterceptor()
        {
            IUnityContainer container = new UnityContainer();

            Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "\\AssemblyWithUnityInterface.dll");
            container.RegisterTypes(AllClasses.FromLoadedAssemblies(), WithMappings.FromAllInterfaces, WithName.TypeName, null, null, true);
            bool isInterceptorFound = false;
            foreach (var registration in container.Registrations)
            {
                if (registration.Name == null)
                {
                    continue;
                }

                if (registration.Name.Contains("MyInterceptorClass"))
                {
                    isInterceptorFound = true;
                }
            }
            Assert.IsTrue(isInterceptorFound);
        }
    }
}
