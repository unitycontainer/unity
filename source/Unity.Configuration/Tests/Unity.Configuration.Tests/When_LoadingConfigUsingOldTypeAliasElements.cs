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
    /// Summary description for When_LoadingConfigUsingOldTypeAliasElements
    /// </summary>
    [TestClass]
    public class When_LoadingConfigUsingOldTypeAliasElements : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingConfigUsingOldTypeAliasElements() : base("OldAliasesSyntax")
        {
            
        }

        [TestMethod]
        public void Then_ExpectedNumberOfAliasesArePresent()
        {
            Assert.AreEqual(8, Section.TypeAliases.Count);
        }

        [TestMethod]
        public void Then_AliasesAreAvailableInExpectedOrder()
        {
            CollectionAssert.AreEqual(
                new [] { "string", "int", "ILogger", "MockLogger", "SpecialLogger", "DependentConstructor", "TwoConstructorArgs", "MockDatabase" },
                Section.TypeAliases.Select(a => a.Alias).ToList());
        }
    }
}
