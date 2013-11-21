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
    public partial class AssemblyMatchingRuleFixture
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
        public void CanExplicitlyDenyMatchOnVersion()
        {
            AssemblyMatchingRule matchingRule = new AssemblyMatchingRule("mscorlib, Version=1.2.3.4, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            Assert.IsFalse(matchingRule.Matches(objectToStringMethod));
        }

    }
}
