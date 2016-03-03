// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Unity.StaticFactory;
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
    /// Tests for the Static Factory container extension.
    /// </summary>
     
    public class StaticFactoryFixture
    {
        // Turn off the obsolete warning for StaticFactoryExtension
#pragma warning disable 618
        [Fact]
        public void CanAddExtensionToContainer()
        {
            IUnityContainer container = new UnityContainer()
                .AddNewExtension<StaticFactoryExtension>();
        }

        [Fact]
        public void CanGetExtensionConfigurationFromContainer()
        {
            IUnityContainer container = new UnityContainer()
                .AddNewExtension<StaticFactoryExtension>();

            IStaticFactoryConfiguration config = container.Configure<IStaticFactoryConfiguration>();

            Assert.NotNull(config);
        }

        [Fact]
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
            Assert.True(factoryWasCalled);
        }

        [Fact]
        public void CanUseContainerToResolveFactoryParameters()
        {
            bool factoryWasCalled = false;
            string connectionString = "Northwind";

            IUnityContainer container = new UnityContainer();

            container.AddNewExtension<StaticFactoryExtension>()
                .Configure<IStaticFactoryConfiguration>()
                    .RegisterFactory<MockDatabase>(c =>
                    {
                        Assert.Same(container, c);
                        factoryWasCalled = true;
                        string cs = c.Resolve<string>("connectionString");
                        return MockDatabase.Create(cs);
                    })
                    .Container
                .RegisterInstance<string>("connectionString", connectionString);

            MockDatabase db = container.Resolve<MockDatabase>();

            Assert.True(factoryWasCalled);
            Assert.NotNull(db);
            Assert.Equal(connectionString, db.ConnectionString);
        }

        [Fact]
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
                    Assert.Same(child, c);
                    return MockDatabase.Create("connectionString");
                });

            var db = child.Resolve<MockDatabase>();
        }

        [Fact]
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
            Assert.True(factoryWasCalled);
        }

        [Fact]
        public void CanUseContainerToResolveFactoryParametersWhenUsingInjectionFactory()
        {
            bool factoryWasCalled = false;
            string connectionString = "Northwind";

            IUnityContainer container = new UnityContainer();

            container.RegisterType<MockDatabase>(
                new InjectionFactory(c =>
                    {
                        Assert.Same(container, c);
                        factoryWasCalled = true;
                        var cs = c.Resolve<string>("connectionString");
                        return MockDatabase.Create(cs);
                    }))
                .RegisterInstance<string>("connectionString", connectionString);

            MockDatabase db = container.Resolve<MockDatabase>();

            Assert.True(factoryWasCalled);
            Assert.NotNull(db);
            Assert.Equal(connectionString, db.ConnectionString);
        }

        [Fact]
        public void FactoryRecievesCurrentContainerWhenUsingChildWhenUsingInjectionFactory()
        {
            bool factoryWasCalled = false;
            IUnityContainer parent = new UnityContainer();

            IUnityContainer child = parent.CreateChildContainer();

            parent.RegisterType<MockDatabase>(
                new InjectionFactory(c =>
                {
                    factoryWasCalled = true;
                    Assert.Same(child, c);
                    return MockDatabase.Create("connectionString");
                }));

            child.Resolve<MockDatabase>();
            Assert.True(factoryWasCalled);
        }

        [Fact]
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

        [Fact]
        public void RegisteredFactoriesAreExecutedWhenDoingResolveAll()
        {
            var container = new UnityContainer()
                .RegisterType<string>("one", new InjectionFactory(c => "this"))
                .RegisterType<string>("two", new InjectionFactory(c => "that"))
                .RegisterType<string>("three", new InjectionFactory(c => "the other"));

            var result = container.ResolveAll<string>();
            result.AssertContainsInAnyOrder("this", "that", "the other");
        }

        [Fact]
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
