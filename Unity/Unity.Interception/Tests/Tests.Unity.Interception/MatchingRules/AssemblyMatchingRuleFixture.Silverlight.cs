using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules
{
    public partial class AssemblyMatchingRuleFixture
    {
        [TestMethod]
        public void CanMatchAssemblyNameByNameAndVersion()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=2.0.5.0");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchAssemblyNameByNameVersionAndKey()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=2.0.5.0, PublicKeyToken=7cec85d7bea7798e");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchAssemblyNameByNameVersionAndCulture()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=2.0.5.0, Culture=neutral");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchAssemblyByFullyQualifiedName()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanExplicitlyDenyMatchOnNoKey()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=null");
            Assert.IsFalse(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanExplicitlyDenyMatchOnSpecificKey()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            Assert.IsFalse(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanExplicitlyDenyMatchOnSpecificCulture()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=2.0.5.0, Culture=nl-NL, PublicKeyToken=7cec85d7bea7798e");
            Assert.IsFalse(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchAssemblyNameUsingArbitraryAmountOfSpaces()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib,Version=2.0.5.0,    Culture=neutral, PublicKeyToken=7cec85d7bea7798e");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }
    }
}
