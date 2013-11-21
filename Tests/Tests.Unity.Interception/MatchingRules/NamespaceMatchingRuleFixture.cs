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
using Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules.SeparateTopLevel;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules.TopLevel;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules.TopLevel.SecondLevel;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules.TopLevel.SecondLevel.ThirdLevel;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules.TopLevelTwo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules
{
    /// <summary>
    /// Unit tests for NamespaceMatchingRule
    /// </summary>
    [TestClass]
    public class NamespaceMatchingRuleFixture
    {
        const string topLevelNamespace =
            "Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules.TopLevel";

        const string topLevelNamespaceWildcard =
            "Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules.TopLevel.*";

        const string secondLevelNamespace =
            "Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules.TopLevel.SecondLevel";

        const string thirdLevelNamespace =
            "Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules.TopLevel.SecondLevel.ThirdLevel";

        const string topLevelTwoNamespace =
            "Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules.TopLevelTwo";

        [TestMethod]
        public void ShouldMatchWhenInExactNamespace()
        {
            TestMatch(topLevelNamespace, typeof(TopLevelTarget), true);
        }

        [TestMethod]
        public void ShouldNotMatchWhenInChildNamespace()
        {
            TestMatch(topLevelNamespace, typeof(SecondLevelTarget), false);
        }

        [TestMethod]
        public void ShouldMatchWithIgnoreCaseTurnedOn()
        {
            TestMatch(topLevelNamespace, true, typeof(TopLevelTarget), true);
        }

        [TestMethod]
        public void ShouldNotMatchWhenInChildNamespaceWithIgnoreCase()
        {
            TestMatch(topLevelNamespace, true, typeof(SecondLevelTarget), false);
        }

        [TestMethod]
        public void ShouldMatchInExactNamespaceWithWildcard()
        {
            TestMatch(topLevelNamespaceWildcard, typeof(TopLevelTarget), true);
        }

        [TestMethod]
        public void ShouldMatchInChildNamespaceWithWildcard()
        {
            TestMatch(topLevelNamespaceWildcard, typeof(SecondLevelTarget), true);
        }

        [TestMethod]
        public void ShouldNotMatchIfNotInSameOrChildNamespace()
        {
            TestMatch(topLevelNamespaceWildcard, typeof(SeparateTarget), false);
        }

        [TestMethod]
        public void ShouldNotMatchIfNamespaceOfTargetStartsWithSameStringAsMatchingNamespace()
        {
            TestMatch(topLevelNamespaceWildcard, typeof(TopLevelTwoTarget), false);
        }

        [TestMethod]
        public void ShouldNotMatchIfTargetIsInParentNamespace()
        {
            TestMatch(secondLevelNamespace, typeof(TopLevelTarget), false);
        }

        [TestMethod]
        public void ShouldMatchWithWildcardMultipleLevelsDeep()
        {
            TestMatch(topLevelNamespaceWildcard, typeof(ThirdLevelTarget), true);
        }

        [TestMethod]
        public void ShouldMatchWithMultipleMatchOptions()
        {
            IMatchingRule rule =
                new NamespaceMatchingRule(new MatchingInfo[]
                                              {
                                                  new MatchingInfo(topLevelNamespace),
                                                  new MatchingInfo(thirdLevelNamespace),
                                                  new MatchingInfo(topLevelTwoNamespace.ToUpperInvariant(), true)
                                              });

            Assert.IsTrue(rule.Matches(GetTargetMethod(typeof(TopLevelTarget))));
            Assert.IsFalse(rule.Matches(GetTargetMethod(typeof(SecondLevelTarget))));
            Assert.IsTrue(rule.Matches(GetTargetMethod(typeof(ThirdLevelTarget))));
            Assert.IsFalse(rule.Matches(GetTargetMethod(typeof(SeparateTarget))));
            Assert.IsTrue(rule.Matches(GetTargetMethod(typeof(TopLevelTwoTarget))));
        }

        void TestMatch(string namespaceName,
                       bool ignoreCase,
                       Type targetType,
                       bool shouldMatch)
        {
            NamespaceMatchingRule rule = new NamespaceMatchingRule(namespaceName, ignoreCase);
            MethodInfo methodToMatch = targetType.GetMethod("TargetMethod");
            Assert.AreEqual(shouldMatch, rule.Matches(methodToMatch));
        }

        void TestMatch(string namespaceName,
                       Type targetType,
                       bool shouldMatch)
        {
            TestMatch(namespaceName, false, targetType, shouldMatch);
        }

        MethodInfo GetTargetMethod(Type t)
        {
            return t.GetMethod("TargetMethod");
        }
    }

    namespace TopLevel
    {
        class TopLevelTarget
        {
            public void TargetMethod() { }
        }

        namespace SecondLevel
        {
            class SecondLevelTarget
            {
                public void TargetMethod() { }
            }

            namespace ThirdLevel
            {
                class ThirdLevelTarget
                {
                    public void TargetMethod() { }
                }
            }
        }
    }

    namespace SeparateTopLevel
    {
        class SeparateTarget
        {
            public void TargetMethod() { }
        }
    }

    namespace TopLevelTwo
    {
        class TopLevelTwoTarget
        {
            public void TargetMethod() { }
        }
    }
}
