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
using Microsoft.Practices.Unity.Configuration.Tests.TestObjects;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerWithSectionExtensions
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerWithSectionExtensions : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithSectionExtensions() : base("SectionExtensions", "")
        {
        }

        [TestMethod]
        public void Then_ExtensionValueElementIsCalled()
        {
            var result = Container.Resolve<ObjectTakingScalars>();

            Assert.AreEqual(17, result.IntValue);
        }

        [TestMethod]
        public void Then_PrefixedExtensionValueElementIsCalled()
        {
            var result = Container.Resolve<ObjectTakingScalars>("prefixedValue");

            Assert.AreEqual(17, result.IntValue);
        }


    }
}
