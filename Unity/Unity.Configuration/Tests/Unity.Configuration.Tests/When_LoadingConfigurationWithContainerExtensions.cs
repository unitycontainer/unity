using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_LoadingConfigurationWithContainerExtensions
    /// </summary>
    [TestClass]
    public class When_LoadingConfigurationWithContainerExtensions : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingConfigurationWithContainerExtensions() : base("ContainerExtensions")
        {
        }

        private ContainerElement defaultContainer;
        private ContainerElement newSchemaContainer;

        protected override void Act()
        {
            base.Act();
            defaultContainer = Section.Containers.Default;
            newSchemaContainer = Section.Containers["newSchema"];
        }

        [TestMethod]
        public void Then_ContainerElementContainsOneExtension()
        {
            Assert.AreEqual(1, defaultContainer.Extensions.Count);
        }

        [TestMethod]
        public void Then_ExtensionElementHasExpectedType()
        {
            Assert.AreEqual("MockExtension",
                defaultContainer.Extensions[0].TypeName);
        }

        [TestMethod]
        public void Then_NewSchemaContainerContainsOneExtension()
        {
            Assert.AreEqual(1, newSchemaContainer.Extensions.Count);
            
        }

        [TestMethod]
        public void Then_NewSchemaContainerExtensionElementHasExpectedType()
        {
            Assert.AreEqual("MockExtension",
                newSchemaContainer.Extensions[0].TypeName);
        }
    }
}
