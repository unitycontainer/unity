// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.ObjectBuilder;
using Unity.Tests.TestDoubles;
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
     
    public class UnityExtensionFixture
    {
        [Fact]
        public void ContainerCallsExtensionsInitializeMethod()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer();
            container.AddExtension(extension);

            Assert.True(extension.InitializeWasCalled);
        }

        [Fact]
        public void ExtensionReceivesExtensionContextInInitialize()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer();
            container.AddExtension(extension);

            Assert.NotNull(extension.Context);
            Assert.Same(container, extension.Context.Container);
        }

        [Fact]
        public void CanGetConfigurationInterfaceFromExtension()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            IMockConfiguration config = container.Configure<IMockConfiguration>();

            Assert.Same(extension, config);
            Assert.Same(container, config.Container);
        }

        [Fact]
        public void CanGetConfigurationWithoutGenericMethod()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            IMockConfiguration config = (IMockConfiguration)container.Configure(typeof(IMockConfiguration));

            Assert.Same(extension, config);
            Assert.Same(container, config.Container);
        }

        [Fact]
        public void ExtensionCanAddStrategy()
        {
            SpyStrategy spy = new SpyStrategy();
            SpyExtension extension = new SpyExtension(spy, UnityBuildStage.PostInitialization);

            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            object result = container.Resolve<object>();
            Assert.True(spy.BuildUpWasCalled);
            Assert.Same(result, spy.Existing);
        }

        [Fact]
        public void ExtensionCanAddPolicy()
        {
            SpyStrategy spy = new SpyStrategy();
            SpyPolicy spyPolicy = new SpyPolicy();

            SpyExtension extension =
                new SpyExtension(spy, UnityBuildStage.PostInitialization, spyPolicy, typeof(SpyPolicy));

            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            container.Resolve<object>();

            Assert.True(spyPolicy.WasSpiedOn);
        }

        [Fact]
        public void CanReinstallDefaultBehavior()
        {
            IUnityContainer container = new UnityContainer()
                .RemoveAllExtensions()
                .AddExtension(new UnityDefaultStrategiesExtension());

            object result = container.Resolve<object>();
            Assert.NotNull(result);
        }

        [Fact]
        public void CanLookupExtensionByClassName()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer();
            container.AddExtension(extension);

            MockContainerExtension result = container.Configure<MockContainerExtension>();

            Assert.Same(extension, result);
        }

        [Fact]
        public void ContainerRaisesChildContainerCreatedToExtension()
        {
            bool childContainerEventRaised = false;
            var mockExtension = new MockContainerExtension();

            var container = new UnityContainer()
                .AddExtension(mockExtension);

            mockExtension.Context.ChildContainerCreated += (sender, ev) =>
                {
                    childContainerEventRaised = true;
                };

            var child = container.CreateChildContainer();
            Assert.True(childContainerEventRaised);
        }

        [Fact]
        public void ChildContainerCreatedEventGivesChildContainerToExtension()
        {
            var mockExtension = new MockContainerExtension();
            ExtensionContext childContext = null;

            var container = new UnityContainer()
                .AddExtension(mockExtension);

            mockExtension.Context.ChildContainerCreated += (sender, ev) =>
            {
                childContext = ev.ChildContext;
            };

            var child = container.CreateChildContainer();
            Assert.Same(child, childContext.Container);
        }

        [Fact]
        public void CanAddExtensionWithNonDefaultConstructor()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<ContainerExtensionWithNonDefaultConstructor>();
            var extension = container.Configure(typeof(ContainerExtensionWithNonDefaultConstructor));
            Assert.NotNull(extension);
        }
    }
}
