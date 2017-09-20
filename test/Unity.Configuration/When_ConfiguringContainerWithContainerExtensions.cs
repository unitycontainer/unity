// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.Configuration.Tests.ConfigFiles;
using Unity.TestSupport;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerWithContainerExtensions
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerWithContainerExtensions : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithContainerExtensions()
            : base("ContainerExtensions")
        {
        }

        private IUnityContainer container;

        protected override void Arrange()
        {
            base.Arrange();
            this.container = new UnityContainer();
        }

        protected override void Act()
        {
            base.Act();
            this.section.Configure(this.container);
        }

        [TestMethod]
        public void Then_ContainerHasExtensionAdded()
        {
            Assert.IsNotNull(this.container.Configure<MockContainerExtension>());
        }
    }
}
