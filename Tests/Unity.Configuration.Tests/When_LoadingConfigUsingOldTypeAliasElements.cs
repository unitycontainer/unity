// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using System.Text;
using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport;
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
        public When_LoadingConfigUsingOldTypeAliasElements()
            : base("OldAliasesSyntax")
        {
        }

        [TestMethod]
        public void Then_ExpectedNumberOfAliasesArePresent()
        {
            Assert.AreEqual(8, section.TypeAliases.Count);
        }

        [TestMethod]
        public void Then_AliasesAreAvailableInExpectedOrder()
        {
            CollectionAssertExtensions.AreEqual(
                new[] { "string", "int", "ILogger", "MockLogger", "SpecialLogger", "DependentConstructor", "TwoConstructorArgs", "MockDatabase" },
                section.TypeAliases.Select(a => a.Alias).ToList());
        }
    }
}
