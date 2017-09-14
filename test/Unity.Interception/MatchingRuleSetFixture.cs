// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Tests
{
    /// <summary>
    /// Tests for the MatchingRuleSet class
    /// </summary>
    [TestClass]
    public class MatchingRuleSetFixture
    {
        [TestMethod]
        public void ShouldNotMatchWithNoContainedRules()
        {
            MatchingRuleSet ruleSet = new MatchingRuleSet();

            MethodBase member = GetType().GetMethod("ShouldNotMatchWithNoContainedRules");
            Assert.IsFalse(ruleSet.Matches(member));
        }

        [TestMethod]
        public void ShouldMatchWithMatchingTypeRule()
        {
            MatchingRuleSet ruleSet = new MatchingRuleSet();
            ruleSet.Add(new TypeMatchingRule(typeof(MatchingRuleSetFixture)));
            MethodBase member = GetType().GetMethod("ShouldMatchWithMatchingTypeRule");
            Assert.IsTrue(ruleSet.Matches(member));
        }

        [TestMethod]
        public void ShouldNotMatchWhenOneRuleDoesntMatch()
        {
            MethodBase member = GetType().GetMethod("ShouldNotMatchWhenOneRuleDoesntMatch");
            MatchingRuleSet ruleSet = new MatchingRuleSet();

            ruleSet.Add(new TypeMatchingRule(typeof(MatchingRuleSetFixture)));
            Assert.IsTrue(ruleSet.Matches(member));

            ruleSet.Add(new MemberNameMatchingRule("ThisMethodDoesntExist"));
            Assert.IsFalse(ruleSet.Matches(member));
        }
    }
}
