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
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules
{
    [TestClass]
    public class PropertyMatchingRuleFixture
    {
        MethodInfo getMyProperty;
        MethodInfo setMyProperty;
        MethodInfo getMyOtherProperty;
        MethodInfo setNotAProperty;
        MethodInfo getACompletelyDifferentProperty;
        MethodInfo setACompletelyDifferentProperty;

        [TestInitialize]
        public void Setup()
        {
            Type propTarget = typeof(PropertyTarget);
            getMyProperty = propTarget.GetProperty("MyProperty").GetGetMethod();
            setMyProperty = propTarget.GetProperty("MyProperty").GetSetMethod();
            getMyOtherProperty = propTarget.GetProperty("MyOtherProperty").GetGetMethod();
            setNotAProperty = propTarget.GetMethod("set_NotAProperty");
            getACompletelyDifferentProperty =
                propTarget.GetProperty("ACompletelyDifferentProperty").GetGetMethod();
            setACompletelyDifferentProperty =
                propTarget.GetProperty("ACompletelyDifferentProperty").GetSetMethod();
        }

        [TestMethod]
        public void ShouldMatchPropertyName()
        {
            IMatchingRule rule =
                new PropertyMatchingRule("MyProperty");
            Assert.IsTrue(rule.Matches(getMyProperty));
            Assert.IsTrue(rule.Matches(setMyProperty));
        }

        [TestMethod]
        public void ShouldNotMatchSetWithGetOption()
        {
            IMatchingRule rule =
                new PropertyMatchingRule("MyProperty", PropertyMatchingOption.Get);
            Assert.IsTrue(rule.Matches(getMyProperty));
            Assert.IsFalse(rule.Matches(setMyProperty));
        }

        [TestMethod]
        public void ShouldNotMatchGetWithSetOption()
        {
            IMatchingRule rule =
                new PropertyMatchingRule("MyProperty", PropertyMatchingOption.Set);
            Assert.IsFalse(rule.Matches(getMyProperty));
            Assert.IsTrue(rule.Matches(setMyProperty));
        }

        [TestMethod]
        public void ShouldMatchWithWildcard()
        {
            IMatchingRule rule =
                new PropertyMatchingRule("My*");
            Assert.IsTrue(rule.Matches(getMyProperty));
            Assert.IsTrue(rule.Matches(setMyProperty));
            Assert.IsTrue(rule.Matches(getMyOtherProperty));
        }

        [TestMethod]
        public void ShouldNotMatchPathologiciallyNamedMethod()
        {
            IMatchingRule rule = new PropertyMatchingRule("NotAProperty");
            Assert.IsFalse(rule.Matches(setNotAProperty));
        }

        [TestMethod]
        public void ShouldMatchWithMultipleMatchTargets()
        {
            IMatchingRule rule = new PropertyMatchingRule(new PropertyMatchingInfo[]
                                                              {
                                                                  new PropertyMatchingInfo("MyProperty"),
                                                                  new PropertyMatchingInfo("ACompletelyDifferentProperty", PropertyMatchingOption.Set)
                                                              });
            Assert.IsTrue(rule.Matches(getMyProperty));
            Assert.IsTrue(rule.Matches(setMyProperty));
            Assert.IsFalse(rule.Matches(getMyOtherProperty));
            Assert.IsFalse(rule.Matches(getACompletelyDifferentProperty));
            Assert.IsTrue(rule.Matches(setACompletelyDifferentProperty));
        }
    }

    class PropertyTarget
    {
        public double ACompletelyDifferentProperty
        {
            get { return -23.4; }
            set { }
        }

        public string MyOtherProperty
        {
            get { return "abc"; }
        }

        public int MyProperty
        {
            get { return 1; }
            set { }
        }

        public void NotAProperty() {}

        public void set_NotAProperty(string value) {}
    }
}
