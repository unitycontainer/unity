// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for SingleSectionSingleContainerFixture
    /// </summary>
    [TestClass]
    public class When_LoadingConfigWithSingleContainer : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingConfigWithSingleContainer()
            : base("SingleSectionSingleContainer")
        {
            
        }

        [TestMethod]
        public void Then_SectionIsNotNull()
        {
            Assert.IsNotNull(Section);
        }

        [TestMethod]
        public void Then_ContainersPropertyIsSet()
        {
            Assert.IsNotNull(Section.Containers);
        }

        [TestMethod]
        public void Then_ThereIsOneContainerInSection()
        {
            Assert.AreEqual(1, Section.Containers.Count);
        }
    }
}
