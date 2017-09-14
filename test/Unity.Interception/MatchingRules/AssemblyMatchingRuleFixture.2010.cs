// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Tests.MatchingRules
{
    // Tests on assembly matching rules that only work in VS 2010/.NET 4.0
    public partial class AssemblyMatchingRuleFixture
    {
        [TestMethod]
        public void CanMatchAssemblyNameByNameAndVersion()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=4.0.0.0");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchAssemblyNameByNameVersionAndKey()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=4.0.0.0, PublicKeyToken=b77a5c561934e089");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchAssemblyNameByNameVersionAndCulture()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=4.0.0.0, Culture=neutral");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchAssemblyByFullyQualifiedName()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanExplicitlyDenyMatchOnNoKey()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=(null)");
            Assert.IsFalse(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanExplicitlyDenyMatchOnSpecificKey()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            Assert.IsFalse(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanExplicitlyDenyMatchOnSpecificCulture()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=4.0.0.0, Culture=nl-NL, PublicKeyToken=b77a5c561934e089");
            Assert.IsFalse(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchAssemblyNameUsingArbitraryAmountOfSpaces()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib,Version=4.0.0.0,    Culture=neutral, PublicKeyToken=b77a5c561934e089");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }
    }
}
