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

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_LoadingSectionWithAliases
    /// </summary>
    [TestClass]
    public class When_LoadingSectionWithAliases : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingSectionWithAliases() : base("TwoContainersAndAliases")
        {
        }

        [TestMethod]
        public void Then_AliasesAreAvailableInTheSection()
        {
            Assert.IsNotNull(Section.TypeAliases);
        }

        [TestMethod]
        public void Then_ExpectedNumberOfAliasesArePresent()
        {
            Assert.AreEqual(2, Section.TypeAliases.Count);
        }

        [TestMethod]
        public void Then_IntIsMappedToSystemInt32()
        {
            Assert.AreEqual("System.Int32, mscorlib", Section.TypeAliases["int"]);
        }

        [TestMethod]
        public void Then_StringIsMappedToSystemString()
        {
            Assert.AreEqual("System.String, mscorlib", Section.TypeAliases["string"]);
        }

        [TestMethod]
        public void Then_EnumerationReturnsAliasesInOrderAsGivenInFile()
        {
            CollectionAssert.AreEqual(new [] {"int", "string"},
                Section.TypeAliases.Select(alias => alias.Alias).ToList());
        }

        [TestMethod]
        public void Then_ContainersInTheFileAreAlsoLoaded()
        {
            Assert.AreEqual(2, Section.Containers.Count);
        }
    }
}
