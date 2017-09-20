// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.Configuration.Tests.ConfigFiles;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Configuration.Tests
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
            Assert.IsNotNull(this.section);
        }

        [TestMethod]
        public void Then_ContainersPropertyIsSet()
        {
            Assert.IsNotNull(section.Containers);
        }

        [TestMethod]
        public void Then_ThereIsOneContainerInSection()
        {
            Assert.AreEqual(1, section.Containers.Count);
        }
    }
}
