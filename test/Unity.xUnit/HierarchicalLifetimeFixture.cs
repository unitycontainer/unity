// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection.Specification.Fakes;
using System;
using Xunit;

namespace Unity.Tests
{
     
    public class WhenUsingHierarchicalLifetimeWithChildContainers
    {
        private IUnityContainer child1;
        private IUnityContainer child2;
        private IUnityContainer container;

        public WhenUsingHierarchicalLifetimeWithChildContainers()
        {
            container = new UnityContainer();
            child1 = container.CreateChildContainer();
            child2 = container.CreateChildContainer();
            container.RegisterType<TestClass>(new HierarchicalLifetimeManager());
        }

        [Fact]
        public void ThenResolvingInParentActsLikeContainerControlledLifetime()
        {
            var o1 = container.Resolve<TestClass>();
            var o2 = container.Resolve<TestClass>();
            Assert.Same(o1, o2);
        }

        [Fact]
        public void ThenParentAndChildResolveDifferentInstances()
        {
            var o1 = container.Resolve<TestClass>();
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
            var o1 = container.Resolve<TestClass>();
            var o2 = child1.Resolve<TestClass>();

            child1.Dispose();
            Assert.False(o1.Disposed);
            Assert.True(o2.Disposed);
        }

        [Fact]
        public void DisposingScopeDisposesService()
        {
            // Arrange
            container.RegisterType<IFakeService, FakeService>();
            container.RegisterType<IFakeSingletonService, FakeService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFakeScopedService, FakeService>(new HierarchicalTransientLifetimeManager());

            FakeService disposableService;
            FakeService transient1;
            FakeService transient2;
            FakeService singleton;

            // Act and Assert
            var transient3 = Assert.IsType<FakeService>(container.Resolve<IFakeService>());
            using (var scope = container.CreateChildContainer())
            {
                disposableService = (FakeService)scope.Resolve<IFakeScopedService>();
                transient1 = (FakeService)scope.Resolve<IFakeService>();
                transient2 = (FakeService)scope.Resolve<IFakeService>();
                singleton = (FakeService)scope.Resolve<IFakeSingletonService>();

                Assert.False(disposableService.Disposed);
                Assert.False(transient1.Disposed);
                Assert.False(transient2.Disposed);
                Assert.False(singleton.Disposed);
            }

            Assert.True(disposableService.Disposed);
            Assert.False(transient1.Disposed);
            Assert.False(transient2.Disposed);
            Assert.False(singleton.Disposed);

            var disposableProvider = container as IDisposable;
            if (disposableProvider != null)
            {
                disposableProvider.Dispose();
                Assert.False(transient1.Disposed);
                Assert.False(transient2.Disposed);
                Assert.True(singleton.Disposed);
                Assert.False(transient3.Disposed);
            }
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
