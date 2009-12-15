using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_LoadingConfigWithMultipleContainers
    /// </summary>
    [TestClass]
    public class When_LoadingConfigWithMultipleContainers : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingConfigWithMultipleContainers() : base("SingleSectionMultipleNamedContainers")
        {
        }

        [TestMethod]
        public void Then_ExpectedNumberOfContainersArePresent()
        {
            Assert.AreEqual(2, Section.Containers.Count);
        }

        [TestMethod]
        public void Then_FirstContainerNameIsCorrect()
        {
            Assert.AreEqual("one", Section.Containers[0].Name);
        }

        [TestMethod]
        public void Then_SecondContainerNameIsCorrect()
        {
            Assert.AreEqual("two", Section.Containers[1].Name);
        }

        [TestMethod]
        public void Then_EnumeratingContainersHappensInOrderOfConfigFile()
        {
            CollectionAssert.AreEqual(new[] { "one", "two" },
                Section.Containers.Select(c => c.Name).ToList());
        }
    }
}
