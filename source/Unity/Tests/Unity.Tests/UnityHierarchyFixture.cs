// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using Unity.StaticFactory;
using Unity.Tests.TestObjects;
using Unity.TestSupport;
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
    /// <summary>
    /// Tests for the hierarchical features of the UnityContainer.
    /// </summary>
     
    public class UnityHierarchyFixture
    {
        [Fact]
        public void ChildBuildsUsingParentsConfiguration()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<ILogger, MockLogger>();

            IUnityContainer child = parent.CreateChildContainer();
            ILogger logger = child.Resolve<ILogger>();

            Assert.NotNull(logger);
            AssertExtensions.IsInstanceOfType(logger, typeof(MockLogger));
        }

        [Fact]
        public void NamesRegisteredInParentAppearInChild()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<ILogger, SpecialLogger>("special");

            IUnityContainer child = parent.CreateChildContainer();

            ILogger l = child.Resolve<ILogger>("special");

            AssertExtensions.IsInstanceOfType(l, typeof(SpecialLogger));
        }

        [Fact]
        public void NamesRegisteredInParentAppearInChildGetAll()
        {
            string[] databases = { "northwind", "adventureworks", "fabrikam" };
            UnityContainer parent = new UnityContainer();
            parent.RegisterInstance<string>("nwnd", databases[0])
                .RegisterInstance<string>("advwks", databases[1]);

            IUnityContainer child = parent.CreateChildContainer()
                .RegisterInstance<string>("fbkm", databases[2]);

            List<string> dbs = new List<string>(child.ResolveAll<string>());
            CollectionAssertExtensions.AreEquivalent(databases, dbs);
        }

        [Fact]
        public void ChildConfigurationOverridesParentConfiguration()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<ILogger, MockLogger>();
            IUnityContainer child = parent.CreateChildContainer()
                .RegisterType<ILogger, SpecialLogger>();

            ILogger parentLogger = parent.Resolve<ILogger>();
            ILogger childLogger = child.Resolve<ILogger>();

            AssertExtensions.IsInstanceOfType(parentLogger, typeof(MockLogger));
            AssertExtensions.IsInstanceOfType(childLogger, typeof(SpecialLogger));
        }

        [Fact]
        public void ChangeInParentConfigurationIsReflectedInChild()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<ILogger, MockLogger>();
            IUnityContainer child = parent.CreateChildContainer();

            ILogger first = child.Resolve<ILogger>();
            parent.RegisterType<ILogger, SpecialLogger>();
            ILogger second = child.Resolve<ILogger>();

            AssertExtensions.IsInstanceOfType(first, typeof(MockLogger));
            AssertExtensions.IsInstanceOfType(second, typeof(SpecialLogger));
        }

        [Fact]
        public void ChildExtensionDoesntAffectParent()
        {
            bool factoryWasCalled = false;

            UnityContainer parent = new UnityContainer();
            IUnityContainer child = parent.CreateChildContainer()
                .RegisterType<object>(new InjectionFactory(c =>
                {
                    factoryWasCalled = true;
                    return new object();
                }));

            parent.Resolve<object>();
            Assert.False(factoryWasCalled);

            child.Resolve<object>();
            Assert.True(factoryWasCalled);
        }

        [Fact]
        public void DisposingParentDisposesChild()
        {
            UnityContainer parent = new UnityContainer();
            IUnityContainer child = parent.CreateChildContainer();

            DisposableObject spy = new DisposableObject();
            child.RegisterInstance<DisposableObject>(spy);

            parent.Dispose();
            Assert.True(spy.WasDisposed);
        }

        [Fact]
        public void CanDisposeChildWithoutDisposingParent()
        {
            DisposableObject parentSpy = new DisposableObject();
            DisposableObject childSpy = new DisposableObject();

            UnityContainer parent = new UnityContainer();
            parent.RegisterInstance(parentSpy);

            IUnityContainer child = parent.CreateChildContainer()
                .RegisterInstance(childSpy);

            child.Dispose();
            Assert.False(parentSpy.WasDisposed);
            Assert.True(childSpy.WasDisposed);

            childSpy.WasDisposed = false;

            parent.Dispose();
            Assert.True(parentSpy.WasDisposed);
            Assert.False(childSpy.WasDisposed);
        }
    }
}
