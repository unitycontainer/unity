// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerWithContainerExtensions
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerWithContainerExtensions : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithContainerExtensions() : base("ContainerExtensions")
        {
        }

        private IUnityContainer container;

        protected override void Arrange()
        {
            base.Arrange();
            container = new UnityContainer();
        }

        protected override void Act()
        {
            base.Act();
            Section.Configure(container);
        }

        [TestMethod]
        public void Then_ContainerHasExtensionAdded()
        {
            Assert.IsNotNull(container.Configure<MockContainerExtension>());
        }
    }
}
