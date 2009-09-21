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

using Microsoft.Practices.Unity.StaticFactory;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                    .RegisterFactory<MockLogger>( c => {
                            factoryWasCalled = true;
                            return new MockLogger();
                        })
                    .Container
                .RegisterType<ILogger, MockLogger>();

            ILogger logger = container.Resolve<ILogger>();
            Assert.IsInstanceOfType(logger, typeof(MockLogger));
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
                    .RegisterFactory<MockDatabase>(c => {
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
            Assert.IsInstanceOfType(logger, typeof(MockLogger));
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
            IUnityContainer parent = new UnityContainer();

            IUnityContainer child = parent.CreateChildContainer();

            parent.RegisterType<MockDatabase>(
                new InjectionFactory(c => {
                        Assert.AreSame(child, c);
                        return MockDatabase.Create("connectionString");
                    }));

            child.Resolve<MockDatabase>();
        }

#pragma warning restore 618
    }
}
