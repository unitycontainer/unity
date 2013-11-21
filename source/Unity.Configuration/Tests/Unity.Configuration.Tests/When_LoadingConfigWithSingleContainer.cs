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
