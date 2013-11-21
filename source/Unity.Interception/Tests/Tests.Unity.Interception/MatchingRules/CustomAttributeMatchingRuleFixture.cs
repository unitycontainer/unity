// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules
{
    [TestClass]
    public class CustomAttributeMatchingRuleFixture
    {
        [TestMethod]
        public void CustomAttributeRuleMatchesWhenAttributeFuond()
        {
            MethodInfo method = typeof(TestObject).GetMethod("MethodWithAttribute");
            CustomAttributeMatchingRule rule = new CustomAttributeMatchingRule(typeof(RandomAttribute), false);
            Assert.IsTrue(rule.Matches(method));
        }

        [TestMethod]
        public void CustomAttributeRuleDeniesMatchWhenNoAttributeFound()
        {
            MethodInfo method = typeof(TestObject).GetMethod("MethodWithoutAttribute");
            CustomAttributeMatchingRule rule = new CustomAttributeMatchingRule(typeof(RandomAttribute), false);
            Assert.IsFalse(rule.Matches(method));
        }

        [TestMethod]
        public void CustomAttributeRuleSearchesInheritanceChainWhenInheritedIsTrue()
        {
            MethodInfo method = typeof(DerivedObject).GetMethod("MethodWithAttribute");
            CustomAttributeMatchingRule rule = new CustomAttributeMatchingRule(typeof(RandomAttribute), true);
            Assert.IsTrue(rule.Matches(method));
        }

        [TestMethod]
        public void CustomAttributeRuleDoesNotSearchInheritanceChainWhenInheritedIsFalse()
        {
            MethodInfo method = typeof(DerivedObject).GetMethod("MethodWithAttribute");
            CustomAttributeMatchingRule rule = new CustomAttributeMatchingRule(typeof(RandomAttribute), false);
            Assert.IsFalse(rule.Matches(method));
        }

        class DerivedObject : TestObject
        {
            public override void MethodWithAttribute() {}
        }

        class TestObject
        {
            [RandomAttribute]
            public virtual void MethodWithAttribute() {}

            public void MethodWithoutAttribute() {}
        }

        class RandomAttribute : Attribute {}
    }
}
