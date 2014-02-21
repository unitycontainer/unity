// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using Microsoft.Practices.Unity;

namespace Unity.Tests.CollectionSupport
{
    [TestClass]
    public class CollectionSupportFixture
    {
        [TestMethod]
        public void ResolvingAnArrayTypeSucceedsIfItWasNotRegistered()
        {
            IUnityContainer container = new UnityContainer();

            CollectionSupportTestClass[] resolved = container.Resolve<CollectionSupportTestClass[]>();
        }

        [TestMethod]
        public void ResolvingAnArrayTypeSucceedsIfItWasRegistered()
        {
            IUnityContainer container = new UnityContainer();
            CollectionSupportTestClass[] array = new CollectionSupportTestClass[0];
            container.RegisterInstance<CollectionSupportTestClass[]>(array);

            CollectionSupportTestClass[] resolved = container.Resolve<CollectionSupportTestClass[]>();

            Assert.AreSame(array, resolved);
        }

        [TestMethod]
        public void ResolvingAllRegistratiosnForaTypeReturnsAnEmptyArrayWhenNothingIsRegisterd()
        {
            IUnityContainer container = new UnityContainer();

            IEnumerable<CollectionSupportTestClass> resolved = container.ResolveAll<CollectionSupportTestClass>();
            List<CollectionSupportTestClass> resolvedList = new List<CollectionSupportTestClass>(resolved);

            Assert.AreEqual(0, resolvedList.Count);
        }

        [TestMethod]
        public void ResolvingAllRegistratiosnForaTypeReturnsAnEquivalentArrayWhenItemsAreRegisterd()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<CollectionSupportTestClass>("Element1", new ContainerControlledLifetimeManager());
            container.RegisterType<CollectionSupportTestClass>("Element2", new ContainerControlledLifetimeManager());
            container.RegisterType<CollectionSupportTestClass>("Element3", new ContainerControlledLifetimeManager());

            IEnumerable<CollectionSupportTestClass> resolved = container.ResolveAll<CollectionSupportTestClass>();
            List<CollectionSupportTestClass> resolvedList = new List<CollectionSupportTestClass>(resolved);

            Assert.AreEqual(3, resolvedList.Count);
        }

        [TestMethod]
        public void InjectingAnArrayTypeSucceedsIfItWasNotRegistered()
        {
            IUnityContainer container = new UnityContainer();

            CollectionSupportTestClassWithDependency resolved = container.Resolve<CollectionSupportTestClassWithDependency>();
        }

        [TestMethod]
        public void InjectingAnArrayTypeSucceedsIfItWasRegistered()
        {
            IUnityContainer container = new UnityContainer();
            CollectionSupportTestClass[] array = new CollectionSupportTestClass[0];
            container.RegisterInstance<CollectionSupportTestClass[]>(array);

            CollectionSupportTestClassWithDependency resolved = container.Resolve<CollectionSupportTestClassWithDependency>();

            Assert.AreSame(array, resolved.Dependency);
        }

        [TestMethod]
        public void InjectingAnArrayDependencySucceedsIfNoneWereRegistered()
        {
            IUnityContainer container = new UnityContainer();

            CollectionSupportTestClassWithDependencyArrayProperty resolved = container.Resolve<CollectionSupportTestClassWithDependencyArrayProperty>();

            Assert.AreEqual(0, resolved.Dependency.Length);
        }

        [TestMethod]
        public void InjectingAnArrayDependencySucceedsIfSomeWereRegistered()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<CollectionSupportTestClass>("Element1", new ContainerControlledLifetimeManager());
            container.RegisterType<CollectionSupportTestClass>("Element2", new ContainerControlledLifetimeManager());
            container.RegisterType<CollectionSupportTestClass>("Element3", new ContainerControlledLifetimeManager());

            CollectionSupportTestClassWithDependencyArrayProperty resolved = container.Resolve<CollectionSupportTestClassWithDependencyArrayProperty>();

            Assert.AreEqual(3, resolved.Dependency.Length);
        }

        [TestMethod]
        public void ConstructingAnDependencyArrayWithNoRegisteredElementsSucceeds()
        {
            IUnityContainer container = new UnityContainer();

            CollectionSupportTestClassWithDependencyArrayConstructor resolved = container.Resolve<CollectionSupportTestClassWithDependencyArrayConstructor>();

            Assert.AreEqual(0, resolved.Dependency.Length);
        }

        [TestMethod]
        public void ConstructingAnDependencyArrayWithRegisteredElementsSucceeds()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<CollectionSupportTestClass>("Element1", new ContainerControlledLifetimeManager());
            container.RegisterType<CollectionSupportTestClass>("Element2", new ContainerControlledLifetimeManager());
            container.RegisterType<CollectionSupportTestClass>("Element3", new ContainerControlledLifetimeManager());

            CollectionSupportTestClassWithDependencyArrayConstructor resolved = container.Resolve<CollectionSupportTestClassWithDependencyArrayConstructor>();

            Assert.AreEqual(3, resolved.Dependency.Length);
        }

        [TestMethod]
        public void ConstructingAnDependencyArrayTypeSucceedsIfItWasNotRegistered()
        {
            IUnityContainer container = new UnityContainer();

            CollectionSupportTestClassWithDependencyTypeConstructor resolved = container.Resolve<CollectionSupportTestClassWithDependencyTypeConstructor>();
        }

        [TestMethod]
        public void ConstructingWithMethodInjectionAnDependencyArrayWithNoRegisteredElementsSucceeds()
        {
            IUnityContainer container = new UnityContainer();

            CollectionSupportTestClassWithDependencyArrayMethod resolved = container.Resolve<CollectionSupportTestClassWithDependencyArrayMethod>();

            Assert.AreEqual(0, resolved.Dependency.Length);
        }

        [TestMethod]
        public void ConstructingWithMethodInjectionAnDependencyArrayWithRegisteredElementsSucceeds()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<CollectionSupportTestClass>("Element1", new ContainerControlledLifetimeManager());
            container.RegisterType<CollectionSupportTestClass>("Element2", new ContainerControlledLifetimeManager());
            container.RegisterType<CollectionSupportTestClass>("Element3", new ContainerControlledLifetimeManager());

            CollectionSupportTestClassWithDependencyArrayMethod resolved = container.Resolve<CollectionSupportTestClassWithDependencyArrayMethod>();

            Assert.AreEqual(3, resolved.Dependency.Length);
        }

        [TestMethod]
        public void ConstructingWithMethodInjectionAnDependencyArrayTypeSucceedsIfItWasNotRegistered()
        {
            IUnityContainer container = new UnityContainer();

            CollectionSupportTestClassWithDependencyTypeMethod resolved = container.Resolve<CollectionSupportTestClassWithDependencyTypeMethod>();
        }
    }
}
