// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Configuration.Tests.ConfigFiles;
using Unity.Configuration.Tests.TestObjects;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Configuration.Tests
{
    [TestClass]
    public class When_LoadingConfigWithSectionExtensions : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingConfigWithSectionExtensions()
            : base("SectionExtensions")
        {
        }

        [TestMethod]
        public void Then_ExpectedNumberOfSectionExtensionArePresent()
        {
            Assert.AreEqual(2, section.SectionExtensions.Count);
        }

        [TestMethod]
        public void Then_FirstSectionExtensionIsPresent()
        {
            Assert.AreEqual("TestSectionExtension", section.SectionExtensions[0].TypeName);
            Assert.AreEqual(String.Empty, section.SectionExtensions[0].Prefix);
        }

        [TestMethod]
        public void Then_SecondSectionExtensionIsPresent()
        {
            Assert.AreEqual("TestSectionExtension", section.SectionExtensions[1].TypeName);
            Assert.AreEqual("ext2", section.SectionExtensions[1].Prefix);
        }

        [TestMethod]
        public void Then_TestSectionExtensionWasInvokedOnce()
        {
            Assert.AreEqual(1, TestSectionExtension.NumberOfCalls);
        }

        [TestMethod]
        public void Then_ContainerConfiguringExtensionElementsWereAdded()
        {
            Assert.AreEqual(typeof(ContainerConfigElementOne),
                ExtensionElementMap.GetContainerConfiguringElementType("configOne"));
            Assert.AreEqual(typeof(ContainerConfigElementTwo),
                ExtensionElementMap.GetContainerConfiguringElementType("configTwo"));
        }

        [TestMethod]
        public void Then_PrefixedContainerConfiguringExtensionsWereAdded()
        {
            Assert.AreEqual(typeof(ContainerConfigElementOne),
                ExtensionElementMap.GetContainerConfiguringElementType("ext2.configOne"));
            Assert.AreEqual(typeof(ContainerConfigElementTwo),
                ExtensionElementMap.GetContainerConfiguringElementType("ext2.configTwo"));
        }

        [TestMethod]
        public void Then_ValueElementWasAdded()
        {
            Assert.AreEqual(typeof(SeventeenValueElement),
                ExtensionElementMap.GetParameterValueElementType("seventeen"));
        }

        [TestMethod]
        public void Then_UnprefixedAliasWasAdded()
        {
            string typeName = section.TypeAliases["scalarObject"];
            Assert.IsNotNull(typeName);
            Assert.AreEqual(typeof(ObjectTakingScalars).AssemblyQualifiedName, typeName);
        }

        [TestMethod]
        public void Then_PrefixedAliasWasAdded()
        {
            string typeName = section.TypeAliases["ext2.scalarObject"];
            Assert.IsNotNull(typeName);
            Assert.AreEqual(typeof(ObjectTakingScalars).AssemblyQualifiedName, typeName);
        }
    }
}
