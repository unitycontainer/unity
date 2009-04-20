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

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules
{
    [TestClass]
    public class AssemblyMatchingRuleFixture
    {
        MethodBase objectToStringMethod;

        [TestInitialize]
        public void TestInitialize()
        {
            objectToStringMethod = typeof(object).GetMethod("ToString");
        }

        [TestMethod]
        public void CanMatchAssemblyNameByNameOnly()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchAssemblyNameByNameAndVersion()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=2.0.0.0");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        //[TestMethod]
        //public void CanMatchAssemblyNameByNameAndPartialVersion()
        //{
        //    AssemblyNameMatchingRule matchingRule = new AssemblyNameMatchingRule("mscorlib, Version=2.0");
        //    Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        //}

        [TestMethod]
        public void CanMatchAssemblyNameByNameVersionAndKey()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=2.0.0.0, PublicKeyToken=b77a5c561934e089");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchAssemblyNameByNameVersionAndCulture()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=2.0.0.0, Culture=neutral");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchAssemblyByFullyQualifiedName()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanExplicitlyDenyMatchOnNoKey()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=(null)");
            Assert.IsFalse(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanExplicitlyDenyMatchOnSpecificKey()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            Assert.IsFalse(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanExplicitlyDenyMatchOnSpecificCulture()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=2.0.0.0, Culture=nl-NL, PublicKeyToken=b77a5c561934e089");
            Assert.IsFalse(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanExplicitlyDenyMatchOnVersion()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=1.2.3.4, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            Assert.IsFalse(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchAssemblyNameUsingArbitraryAmountOfSpaces()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib,Version=2.0.0.0,    Culture=neutral, PublicKeyToken=b77a5c561934e089");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }
    }
}
