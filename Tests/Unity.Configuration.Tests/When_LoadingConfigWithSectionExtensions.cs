// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.Configuration.Tests.TestObjects;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    [TestClass]
    public class When_LoadingConfigWithSectionExtensions : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingConfigWithSectionExtensions() : base("SectionExtensions")
        {
        }

        [TestMethod]
        public void Then_ExpectedNumberOfSectionExtensionArePresent()
        {
            Assert.AreEqual(2, Section.SectionExtensions.Count);
        }

        [TestMethod]
        public void Then_FirstSectionExtensionIsPresent()
        {
            Assert.AreEqual("TestSectionExtension", Section.SectionExtensions[0].TypeName);
            Assert.AreEqual("", Section.SectionExtensions[0].Prefix);
        }

        [TestMethod]
        public void Then_SecondSectionExtensionIsPresent()
        {
            Assert.AreEqual("TestSectionExtension", Section.SectionExtensions[1].TypeName);
            Assert.AreEqual("ext2", Section.SectionExtensions[1].Prefix);
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
            string typeName = Section.TypeAliases["scalarObject"];
            Assert.IsNotNull(typeName);
            Assert.AreEqual(typeof (ObjectTakingScalars).AssemblyQualifiedName, typeName);
        }

        [TestMethod]
        public void Then_PrefixedAliasWasAdded()
        {
            string typeName = Section.TypeAliases["ext2.scalarObject"];
            Assert.IsNotNull(typeName);
            Assert.AreEqual(typeof (ObjectTakingScalars).AssemblyQualifiedName, typeName);
            
        }



    }
}
