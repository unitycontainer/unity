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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.ObjectBuilder;
using Unity.Tests;
using Unity.Tests.TestDoubles;

namespace Microsoft.Practices.Unity.Tests
{
    [TestClass]
    public class UnityExtensionFixture
    {
        [TestMethod]
        public void ContainerCallsExtensionsInitializeMethod()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer();
            container.AddExtension(extension);

            Assert.IsTrue(extension.InitializeWasCalled);
        }

        [TestMethod]
        public void ExtensionReceivesExtensionContextInInitialize()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer();
            container.AddExtension(extension);

            Assert.IsNotNull(extension.Context);
            Assert.AreSame(container, extension.Context.Container);
        }

        [TestMethod]
        public void CanGetConfigurationInterfaceFromExtension()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            IMockConfiguration config = container.Configure<IMockConfiguration>();

            Assert.AreSame(extension, config);
            Assert.AreSame(container, config.Container);
        }

        [TestMethod]
        public void CanGetConfigurationWithoutGenericMethod()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            IMockConfiguration config = (IMockConfiguration)container.Configure(typeof(IMockConfiguration));

            Assert.AreSame(extension, config);
            Assert.AreSame(container, config.Container);
        }

        [TestMethod]
        public void ExtensionCanAddStrategy()
        {
            SpyStrategy spy = new SpyStrategy();
            SpyExtension extension = new SpyExtension(spy, UnityBuildStage.PostInitialization);

            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            object result = container.Resolve<object>();
            Assert.IsTrue(spy.BuildUpWasCalled);
            Assert.AreSame(result, spy.Existing);
        }

        [TestMethod]
        public void ExtensionCanAddPolicy()
        {
            SpyStrategy spy = new SpyStrategy();
            SpyPolicy spyPolicy = new SpyPolicy();

            SpyExtension extension =
                new SpyExtension(spy, UnityBuildStage.PostInitialization, spyPolicy, typeof(SpyPolicy));

            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            container.Resolve<object>();

            Assert.IsTrue(spyPolicy.WasSpiedOn);
        }

        // TODO: Verify
        //[TestMethod]
        //public void CanReinstallDefaultBehavior()
        //{
        //    IUnityContainer container = new UnityContainer()
        //        .RemoveAllExtensions()
        //        .AddNewExtension<UnityDefaultBehaviorExtension>()
        //        .AddNewExtension<UnityDefaultStrategiesExtension>();

        //    object result = container.Resolve<object>();
        //    Assert.IsNotNull(result);
        //}

        [TestMethod]
        public void CanLookupExtensionByClassName()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer();
            container.AddExtension(extension);

            MockContainerExtension result = container.Configure<MockContainerExtension>();

            Assert.AreSame(extension, result);
        }

    }
}
