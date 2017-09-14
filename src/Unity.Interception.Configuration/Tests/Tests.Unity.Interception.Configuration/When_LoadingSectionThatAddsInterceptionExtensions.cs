// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.Configuration;
using Unity.InterceptionExtension.Configuration.Tests.ConfigFiles;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Configuration.Tests
{
    [TestClass]
    public class When_LoadingSectionThatAddsInterceptionExtensions : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingSectionThatAddsInterceptionExtensions()
            : base("SectionExtensionBasics")
        {
        }

        [TestMethod]
        public void Then_SectionExtensionIsPresent()
        {
            Assert.IsInstanceOfType(section.SectionExtensions[0].ExtensionObject,
                typeof(InterceptionConfigurationExtension));
        }

        [TestMethod]
        public void Then_InterceptionElementHasBeenAdded()
        {
            Assert.IsNotNull(ExtensionElementMap.GetContainerConfiguringElementType("interception"));
        }
    }
}
