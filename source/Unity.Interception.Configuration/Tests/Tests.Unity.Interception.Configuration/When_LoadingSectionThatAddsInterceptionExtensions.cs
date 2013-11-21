// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
