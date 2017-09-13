// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Configuration.Tests.ConfigFiles;
using Unity.Configuration.Tests.TestObjects;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerWithSectionExtensions
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerWithSectionExtensions : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithSectionExtensions()
            : base("SectionExtensions", String.Empty)
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
