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

using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
{
    [TestClass]
    public class When_LoadingSectionThatAddsInterceptionExtensions : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingSectionThatAddsInterceptionExtensions() : base("SectionExtensionBasics")
        {
        }

        [TestMethod]
        public void Then_SectionExtensionIsPresent()
        {
            Assert.IsInstanceOfType(Section.SectionExtensions[0].ExtensionObject,
                typeof(InterceptionConfigurationExtension));
        }

        [TestMethod]
        public void Then_InterceptionElementHasBeenAdded()
        {
            Assert.IsNotNull(ExtensionElementMap.GetContainerConfiguringElementType("interception"));
        }

    }
}
