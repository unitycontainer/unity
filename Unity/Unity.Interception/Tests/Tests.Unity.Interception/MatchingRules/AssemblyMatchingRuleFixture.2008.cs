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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules
{
    // Tests on assembly matching rules that only work in VS 2008/.NET 3.5
    public partial class AssemblyMatchingRuleFixture
    {
        [TestMethod]
        public void CanMatchAssemblyNameByNameAndVersion()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=2.0.0.0");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

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
        public void CanMatchAssemblyNameUsingArbitraryAmountOfSpaces()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib,Version=2.0.0.0,    Culture=neutral, PublicKeyToken=b77a5c561934e089");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }
    }
}
