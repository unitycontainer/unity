// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Unity.Tests
{
    [TestClass]
    public class WhenUsingHierarchicalLifetimeWithChildContainers
    {
        private IUnityContainer child1;
        private IUnityContainer child2;
        private IUnityContainer parentContainer;

        [TestInitialize]
        public void Setup()
        {
            parentContainer = new UnityContainer();
            child1 = parentContainer.CreateChildContainer();
            child2 = parentContainer.CreateChildContainer();
            parentContainer.RegisterType<TestClass>(new HierarchicalLifetimeManager());
        }

        [TestMethod]
        public void ThenResolvingInParentActsLikeContainerControlledLifetime()
        {
            var o1 = parentContainer.Resolve<TestClass>();
            var o2 = parentContainer.Resolve<TestClass>();
            Assert.AreSame(o1, o2);
        }

        [TestMethod]
        public void ThenParentAndChildResolveDifferentInstances()
        {
            var o1 = parentContainer.Resolve<TestClass>();
            var o2 = child1.Resolve<TestClass>();
            Assert.AreNotSame(o1, o2);
        }

        [TestMethod]
        public void ThenChildResolvesTheSameInstance()
        {
            var o1 = child1.Resolve<TestClass>();
            var o2 = child1.Resolve<TestClass>();
            Assert.AreSame(o1, o2);
        }

        [TestMethod]
        public void ThenSiblingContainersResolveDifferentInstances()
        {
            var o1 = child1.Resolve<TestClass>();
            var o2 = child2.Resolve<TestClass>();
            Assert.AreNotSame(o1, o2);
        }

        [TestMethod]
        public void ThenDisposingOfChildContainerDisposesOnlyChildObject()
        {
            var o1 = parentContainer.Resolve<TestClass>();
            var o2 = child1.Resolve<TestClass>();

            child1.Dispose();
            Assert.IsFalse(o1.Disposed);
            Assert.IsTrue(o2.Disposed);
        }

        public class TestClass : IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }
    }
}
