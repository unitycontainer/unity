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
    public class TypeMatchingRuleFixture
    {
        [TestMethod]
        public void ShouldMatchExactClassName()
        {
            TestMatch("MyType1", typeof(MyType1), true);
        }

        [TestMethod]
        public void ShouldNotMatchWithDifferentClassName()
        {
            TestMatch("MyType1", typeof(MyType2), false);
        }

        [TestMethod]
        public void ShouldMatchInDifferentNamespaces()
        {
            TestMatch("MyType1", typeof(ANestedNamespace.MyType1), true);
        }

        [TestMethod]
        public void ShouldMatchCaseInsensitive()
        {
            TestMatch("mytype2", typeof(MyType2), true, true);
        }

        [TestMethod]
        public void ShouldMatchWithFullTypeName()
        {
            TestMatch(
                typeof(MyType1).FullName,
                typeof(MyType1), false, true);
        }

        [TestMethod]
        public void ShouldNotMatchFullNameWithDifferentNamespace()
        {
            TestMatch(typeof(MyType1).FullName, typeof(ANestedNamespace.MyType1), false);
        }

        [TestMethod]
        public void ShouldMatchOneOfMultipleMatchOptions()
        {
            IMatchingRule rule = new TypeMatchingRule(new MatchingInfo[]
                                                          {
                                                              new MatchingInfo(typeof(MyType1).FullName),
                                                              new MatchingInfo("MYTYPE2", true)
                                                          });
            Assert.IsTrue(rule.Matches(typeof(MyType1).GetMethod("TargetMethod")));
            Assert.IsFalse(rule.Matches(typeof(ANestedNamespace.MyType1).GetMethod("TargetMethod")));
            Assert.IsTrue(rule.Matches(typeof(MyType2).GetMethod("TargetMethod")));
        }

        [TestMethod]
        public void ShouldNotMatchTypeIfItImplementsMatchingInterface()
        {
            TestMatch(typeof(IInterfaceOne).FullName, typeof(ANestedNamespace.MyType1), false);
            TestMatch(typeof(IInterfaceOne).FullName, typeof(MyType1), false);
        }

        public void TestMatch(string typeName,
                              Type typeToMatch,
                              bool ignoreCase,
                              bool shouldMatch)
        {
            TypeMatchingRule rule = new TypeMatchingRule(typeName, ignoreCase);
            MethodInfo methodToMatch = typeToMatch.GetMethod("TargetMethod");
            Assert.AreEqual(shouldMatch, rule.Matches(methodToMatch));
        }

        public void TestMatch(string typeName,
                              Type typeToMatch,
                              bool shouldMatch)
        {
            TestMatch(typeName, typeToMatch, false, shouldMatch);
        }
    }

    class MyType1
    {
        public void TargetMethod() { }
    }

    namespace ANestedNamespace
    {
        class MyType1 : IInterfaceOne
        {
            public void TargetMethod() { }
        }
    }

    class MyType2 : IAnotherInterface
    {
        public void ADifferentTargetMethod() { }

        public void TargetMethod() { }
    }

    interface IInterfaceOne
    {
        void TargetMethod();
    }

    interface IAnotherInterface
    {
        void ADifferentTargetMethod();
    }
}