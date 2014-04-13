// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public When_LoadingConfigWithMultipleContainers()
            : base("SingleSectionMultipleNamedContainers")
        {
        }

        [TestMethod]
        public void Then_ExpectedNumberOfContainersArePresent()
        {
            Assert.AreEqual(2, section.Containers.Count);
        }

        [TestMethod]
        public void Then_FirstContainerNameIsCorrect()
        {
            Assert.AreEqual("one", section.Containers[0].Name);
        }

        [TestMethod]
        public void Then_SecondContainerNameIsCorrect()
        {
            Assert.AreEqual("two", section.Containers[1].Name);
        }

        [TestMethod]
        public void Then_EnumeratingContainersHappensInOrderOfConfigFile()
        {
            CollectionAssert.AreEqual(new[] { "one", "two" },
                section.Containers.Select(c => c.Name).ToList());
        }
    }
}
