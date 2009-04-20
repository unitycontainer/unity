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

using System.Collections.Generic;
using Microsoft.Practices.Unity.StaticFactory;
using Microsoft.Practices.Unity.Tests.TestObjects;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Tests
{
    /// <summary>
    /// Tests for the hierarchical features of the UnityContainer.
    /// </summary>
    [TestClass]
    public class UnityHierarchyFixture
    {
        [TestMethod]
        public void ChildBuildsUsingParentsConfiguration()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<ILogger, MockLogger>();

            IUnityContainer child = parent.CreateChildContainer();
            ILogger logger = child.Resolve<ILogger>();

            Assert.IsNotNull(logger);
            Assert.IsInstanceOfType(logger, typeof(MockLogger));
        }

        [TestMethod]
        public void NamesRegisteredInParentAppearInChild()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<ILogger, SpecialLogger>("special");

            IUnityContainer child = parent.CreateChildContainer();

            ILogger l = child.Resolve<ILogger>("special");

            Assert.IsInstanceOfType(l, typeof(SpecialLogger));
        }

        [TestMethod]
        public void NamesRegisteredInParentAppearInChildGetAll()
        {
            string[] databases = { "northwind", "adventureworks", "fabrikam" };
            UnityContainer parent = new UnityContainer();
            parent.RegisterInstance<string>("nwnd", databases[0])
                .RegisterInstance<string>("advwks", databases[1]);

            IUnityContainer child = parent.CreateChildContainer()
                .RegisterInstance<string>("fbkm", databases[2]);

            List<string> dbs = new List<string>(child.ResolveAll<string>());
            CollectionAssert.AreEquivalent(databases, dbs);
        }

        [TestMethod]
        public void ChildConfigurationOverridesParentConfiguration()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<ILogger, MockLogger>();
            IUnityContainer child = parent.CreateChildContainer()
                .RegisterType<ILogger, SpecialLogger>();

            ILogger parentLogger = parent.Resolve<ILogger>();
            ILogger childLogger = child.Resolve<ILogger>();

            Assert.IsInstanceOfType(parentLogger, typeof(MockLogger));
            Assert.IsInstanceOfType(childLogger, typeof(SpecialLogger));
        }

        [TestMethod]
        public void ChangeInParentConfigurationIsReflectedInChild()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<ILogger, MockLogger>();
            IUnityContainer child = parent.CreateChildContainer();

            ILogger first = child.Resolve<ILogger>();
            parent.RegisterType<ILogger, SpecialLogger>();
            ILogger second = child.Resolve<ILogger>();

            Assert.IsInstanceOfType(first, typeof(MockLogger));
            Assert.IsInstanceOfType(second, typeof(SpecialLogger));
        }

        [TestMethod]
        public void ChildExtensionDoesntAffectParent()
        {
            UnityContainer parent = new UnityContainer();
            IUnityContainer child = parent.CreateChildContainer()
                .AddNewExtension<StaticFactoryExtension>();

            bool factoryWasCalled = false;
            child.Configure<IStaticFactoryConfiguration>()
                .RegisterFactory<object>(
                delegate
                {
                    factoryWasCalled = true;
                    return new object();
                });

            parent.Resolve<object>();
            Assert.IsFalse(factoryWasCalled);

            child.Resolve<object>();
            Assert.IsTrue(factoryWasCalled);
        }

        [TestMethod]
        public void DisposingParentDisposesChild()
        {
            UnityContainer parent = new UnityContainer();
            IUnityContainer child = parent.CreateChildContainer();

            DisposableObject spy = new DisposableObject();
            child.RegisterInstance<DisposableObject>(spy);

            parent.Dispose();
            Assert.IsTrue(spy.WasDisposed);
        }

        [TestMethod]
        public void CanDisposeChildWithoutDisposingParent()
        {
            DisposableObject parentSpy = new DisposableObject();
            DisposableObject childSpy = new DisposableObject();

            UnityContainer parent = new UnityContainer();
            parent.RegisterInstance(parentSpy);

            IUnityContainer child = parent.CreateChildContainer()
                .RegisterInstance(childSpy);

            child.Dispose();
            Assert.IsFalse(parentSpy.WasDisposed);
            Assert.IsTrue(childSpy.WasDisposed);

            childSpy.WasDisposed = false;

            parent.Dispose();
            Assert.IsTrue(parentSpy.WasDisposed);
            Assert.IsFalse(childSpy.WasDisposed);
        }
    }
}
