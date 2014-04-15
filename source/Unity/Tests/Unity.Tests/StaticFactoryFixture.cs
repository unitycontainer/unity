// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.Practices.Unity.StaticFactory;
using Microsoft.Practices.Unity.TestSupport;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.Tests
{
    /// <summary>
    /// Tests for the Static Factory container extension.
    /// </summary>
    [TestClass]
    public class StaticFactoryFixture
    {
        // Turn off the obsolete warning for StaticFactoryExtension
#pragma warning disable 618
        [TestMethod]
        public void CanAddExtensionToContainer()
        {
            IUnityContainer container = new UnityContainer()
                .AddNewExtension<StaticFactoryExtension>();
        }

        [TestMethod]
        public void CanGetExtensionConfigurationFromContainer()
        {
            IUnityContainer container = new UnityContainer()
                .AddNewExtension<StaticFactoryExtension>();

            IStaticFactoryConfiguration config = container.Configure<IStaticFactoryConfiguration>();

            Assert.IsNotNull(config);
        }

        [TestMethod]
        public void ConfiguredFactoryDelegateGetsCalled()
        {
            bool factoryWasCalled = false;

            IUnityContainer container = new UnityContainer()
                .AddNewExtension<StaticFactoryExtension>()
                .Configure<IStaticFactoryConfiguration>()
                    .RegisterFactory<MockLogger>(c =>
                    {
                        factoryWasCalled = true;
                        return new MockLogger();
                    })
                    .Container
                .RegisterType<ILogger, MockLogger>();

            ILogger logger = container.Resolve<ILogger>();
            AssertExtensions.IsInstanceOfType(logger, typeof(MockLogger));
            Assert.IsTrue(factoryWasCalled);
        }

        [TestMethod]
        public void CanUseContainerToResolveFactoryParameters()
        {
            bool factoryWasCalled = false;
            string connectionString = "Northwind";

            IUnityContainer container = new UnityContainer();

            container.AddNewExtension<StaticFactoryExtension>()
                .Configure<IStaticFactoryConfiguration>()
                    .RegisterFactory<MockDatabase>(c =>
                    {
                        Assert.AreSame(container, c);
                        factoryWasCalled = true;
                        string cs = c.Resolve<string>("connectionString");
                        return MockDatabase.Create(cs);
                    })
                    .Container
                .RegisterInstance<string>("connectionString", connectionString);

            MockDatabase db = container.Resolve<MockDatabase>();

            Assert.IsTrue(factoryWasCalled);
            Assert.IsNotNull(db);
            Assert.AreEqual(connectionString, db.ConnectionString);
        }

        [TestMethod]
        public void FactoryRecievesCurrentContainerWhenUsingChild()
        {
            IUnityContainer parent = new UnityContainer();

            IUnityContainer child = parent.CreateChildContainer();

            parent
                .AddNewExtension<StaticFactoryExtension>()
                .Configure<StaticFactoryExtension>()
                .RegisterFactory<MockDatabase>(
                c =>
                {
                    Assert.AreSame(child, c);
                    return MockDatabase.Create("connectionString");
                });

            var db = child.Resolve<MockDatabase>();
        }

        [TestMethod]
        public void ConfiguredFactoryGetsCalledWhenUsingInjectionFactory()
        {
            bool factoryWasCalled = false;

            var container = new UnityContainer()
                .RegisterType<MockLogger>(new InjectionFactory(c =>
                    {
                        factoryWasCalled = true;
                        return new MockLogger();
                    }))
                .RegisterType<ILogger, MockLogger>();

            ILogger logger = container.Resolve<ILogger>();
            AssertExtensions.IsInstanceOfType(logger, typeof(MockLogger));
            Assert.IsTrue(factoryWasCalled);
        }

        [TestMethod]
        public void CanUseContainerToResolveFactoryParametersWhenUsingInjectionFactory()
        {
            bool factoryWasCalled = false;
            string connectionString = "Northwind";

            IUnityContainer container = new UnityContainer();

            container.RegisterType<MockDatabase>(
                new InjectionFactory(c =>
                    {
                        Assert.AreSame(container, c);
                        factoryWasCalled = true;
                        var cs = c.Resolve<string>("connectionString");
                        return MockDatabase.Create(cs);
                    }))
                .RegisterInstance<string>("connectionString", connectionString);

            MockDatabase db = container.Resolve<MockDatabase>();

            Assert.IsTrue(factoryWasCalled);
            Assert.IsNotNull(db);
            Assert.AreEqual(connectionString, db.ConnectionString);
        }

        [TestMethod]
        public void FactoryRecievesCurrentContainerWhenUsingChildWhenUsingInjectionFactory()
        {
            bool factoryWasCalled = false;
            IUnityContainer parent = new UnityContainer();

            IUnityContainer child = parent.CreateChildContainer();

            parent.RegisterType<MockDatabase>(
                new InjectionFactory(c =>
                {
                    factoryWasCalled = true;
                    Assert.AreSame(child, c);
                    return MockDatabase.Create("connectionString");
                }));

            child.Resolve<MockDatabase>();
            Assert.IsTrue(factoryWasCalled);
        }

        [TestMethod]
        public void RegisteredFactoriesAreExecutedWhenDoingResolveAllWithOldAPI()
        {
            var container = new UnityContainer()
                .AddNewExtension<StaticFactoryExtension>()
                .Configure<StaticFactoryExtension>()
                    .RegisterFactory<string>("one", c => "this")
                    .RegisterFactory<string>("two", c => "that")
                    .RegisterFactory<string>("three", c => "the other")
                .Container;

            var result = container.ResolveAll<string>();
            result.AssertContainsInAnyOrder("this", "that", "the other");
        }

        [TestMethod]
        public void RegisteredFactoriesAreExecutedWhenDoingResolveAll()
        {
            var container = new UnityContainer()
                .RegisterType<string>("one", new InjectionFactory(c => "this"))
                .RegisterType<string>("two", new InjectionFactory(c => "that"))
                .RegisterType<string>("three", new InjectionFactory(c => "the other"));

            var result = container.ResolveAll<string>();
            result.AssertContainsInAnyOrder("this", "that", "the other");
        }

        [TestMethod]
        public void CanRegisterFactoryFunctionThatReceivesTypeAndName()
        {
            Func<IUnityContainer, Type, string, object> factory = (c, t, s) => s + t.GetTypeInfo().Name;
            var container = new UnityContainer()
                .RegisterType<string>("one", new InjectionFactory(factory))
                .RegisterType<string>("two", new InjectionFactory(factory))
                .RegisterType<string>("three", new InjectionFactory(factory));

            var result = container.ResolveAll<string>();
            result.AssertContainsInAnyOrder("oneString", "twoString", "threeString");
        }
#pragma warning restore 618
    }
}
