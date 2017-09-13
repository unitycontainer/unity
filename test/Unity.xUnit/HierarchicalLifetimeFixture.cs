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
using Xunit;
#endif

namespace Unity.Tests
{
     
    public class WhenUsingHierarchicalLifetimeWithChildContainers
    {
        private IUnityContainer child1;
        private IUnityContainer child2;
        private IUnityContainer parentContainer;

        public WhenUsingHierarchicalLifetimeWithChildContainers()
        {
            parentContainer = new UnityContainer();
            child1 = parentContainer.CreateChildContainer();
            child2 = parentContainer.CreateChildContainer();
            parentContainer.RegisterType<TestClass>(new HierarchicalLifetimeManager());
        }

        [Fact]
        public void ThenResolvingInParentActsLikeContainerControlledLifetime()
        {
            var o1 = parentContainer.Resolve<TestClass>();
            var o2 = parentContainer.Resolve<TestClass>();
            Assert.Same(o1, o2);
        }

        [Fact]
        public void ThenParentAndChildResolveDifferentInstances()
        {
            var o1 = parentContainer.Resolve<TestClass>();
            var o2 = child1.Resolve<TestClass>();
            Assert.NotSame(o1, o2);
        }

        [Fact]
        public void ThenChildResolvesTheSameInstance()
        {
            var o1 = child1.Resolve<TestClass>();
            var o2 = child1.Resolve<TestClass>();
            Assert.Same(o1, o2);
        }

        [Fact]
        public void ThenSiblingContainersResolveDifferentInstances()
        {
            var o1 = child1.Resolve<TestClass>();
            var o2 = child2.Resolve<TestClass>();
            Assert.NotSame(o1, o2);
        }

        [Fact]
        public void ThenDisposingOfChildContainerDisposesOnlyChildObject()
        {
            var o1 = parentContainer.Resolve<TestClass>();
            var o2 = child1.Resolve<TestClass>();

            child1.Dispose();
            Assert.False(o1.Disposed);
            Assert.True(o2.Disposed);
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
