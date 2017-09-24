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


namespace Unity.Tests.CollectionSupport
{
    [TestClass]
    public class CollectionSupportFixture
    {
        [TestMethod]
        public void ResolvingAnArrayTypeSucceedsIfItWasNotRegistered()
        {
            IUnityContainer container = new UnityContainer();

            TestClass[] resolved = container.Resolve<TestClass[]>();
        }

        [TestMethod]
        public void ResolvingAnArrayTypeSucceedsIfItWasRegistered()
        {
            IUnityContainer container = new UnityContainer();
            TestClass[] array = new TestClass[0];
            container.RegisterInstance<TestClass[]>(array);

            TestClass[] resolved = container.Resolve<TestClass[]>();

            Assert.AreSame(array, resolved);
        }

        [TestMethod]
        public void ResolvingAllRegistratiosnForaTypeReturnsAnEmptyArrayWhenNothingIsRegisterd()
        {
            IUnityContainer container = new UnityContainer();

            IEnumerable<TestClass> resolved = container.ResolveAll<TestClass>();
            List<TestClass> resolvedList = new List<TestClass>(resolved);

            Assert.AreEqual(0, resolvedList.Count);
        }

        [TestMethod]
        public void ResolvingAllRegistratiosnForaTypeReturnsAnEquivalentArrayWhenItemsAreRegisterd()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<TestClass>("Element1", new ContainerControlledLifetimeManager());
            container.RegisterType<TestClass>("Element2", new ContainerControlledLifetimeManager());
            container.RegisterType<TestClass>("Element3", new ContainerControlledLifetimeManager());

            IEnumerable<TestClass> resolved = container.ResolveAll<TestClass>();
            List<TestClass> resolvedList = new List<TestClass>(resolved);

            Assert.AreEqual(3, resolvedList.Count);
        }

        [TestMethod]
        public void InjectingAnArrayTypeSucceedsIfItWasNotRegistered()
        {
            IUnityContainer container = new UnityContainer();

            TestClassWithArrayDependency resolved = container.Resolve<TestClassWithArrayDependency>();
        }

        [TestMethod]
        public void InjectingAnArrayTypeSucceedsIfItWasRegistered()
        {
            IUnityContainer container = new UnityContainer();
            TestClass[] array = new TestClass[0];
            container.RegisterInstance<TestClass[]>(array);

            TestClassWithArrayDependency resolved = container.Resolve<TestClassWithArrayDependency>();

            Assert.AreSame(array, resolved.Dependency);
        }

        [TestMethod]
        public void InjectingAnArrayDependencySucceedsIfNoneWereRegistered()
        {
            IUnityContainer container = new UnityContainer();

            TestClassWithDependencyArrayProperty resolved = container.Resolve<TestClassWithDependencyArrayProperty>();

            Assert.AreEqual(0, resolved.Dependency.Length);
        }

        [TestMethod]
        public void InjectingAnArrayDependencySucceedsIfSomeWereRegistered()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<TestClass>("Element1", new ContainerControlledLifetimeManager());
            container.RegisterType<TestClass>("Element2", new ContainerControlledLifetimeManager());
            container.RegisterType<TestClass>("Element3", new ContainerControlledLifetimeManager());

            TestClassWithDependencyArrayProperty resolved = container.Resolve<TestClassWithDependencyArrayProperty>();

            Assert.AreEqual(3, resolved.Dependency.Length);
        }

        [TestMethod]
        public void ConstructingAnDependencyArrayWithNoRegisteredElementsSucceeds()
        {
            IUnityContainer container = new UnityContainer();

            TestClassWithDependencyArrayConstructor resolved = container.Resolve<TestClassWithDependencyArrayConstructor>();

            Assert.AreEqual(0, resolved.Dependency.Length);
        }

        [TestMethod]
        public void ConstructingAnDependencyArrayWithRegisteredElementsSucceeds()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<TestClass>("Element1", new ContainerControlledLifetimeManager());
            container.RegisterType<TestClass>("Element2", new ContainerControlledLifetimeManager());
            container.RegisterType<TestClass>("Element3", new ContainerControlledLifetimeManager());

            TestClassWithDependencyArrayConstructor resolved = container.Resolve<TestClassWithDependencyArrayConstructor>();

            Assert.AreEqual(3, resolved.Dependency.Length);
        }

        [TestMethod]
        public void ConstructingAnDependencyArrayTypeSucceedsIfItWasNotRegistered()
        {
            IUnityContainer container = new UnityContainer();

            TestClassWithDependencyTypeConstructor resolved = container.Resolve<TestClassWithDependencyTypeConstructor>();
        }

        [TestMethod]
        public void ConstructingWithMethodInjectionAnDependencyArrayWithNoRegisteredElementsSucceeds()
        {
            IUnityContainer container = new UnityContainer();

            TestClassWithDependencyArrayMethod resolved = container.Resolve<TestClassWithDependencyArrayMethod>();

            Assert.AreEqual(0, resolved.Dependency.Length);
        }

        [TestMethod]
        public void ConstructingWithMethodInjectionAnDependencyArrayWithRegisteredElementsSucceeds()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<TestClass>("Element1", new ContainerControlledLifetimeManager());
            container.RegisterType<TestClass>("Element2", new ContainerControlledLifetimeManager());
            container.RegisterType<TestClass>("Element3", new ContainerControlledLifetimeManager());

            TestClassWithDependencyArrayMethod resolved = container.Resolve<TestClassWithDependencyArrayMethod>();

            Assert.AreEqual(3, resolved.Dependency.Length);
        }

        [TestMethod]
        public void ConstructingWithMethodInjectionAnDependencyArrayTypeSucceedsIfItWasNotRegistered()
        {
            IUnityContainer container = new UnityContainer();

            TestClassWithDependencyTypeMethod resolved = container.Resolve<TestClassWithDependencyTypeMethod>();
        }
    }
}
