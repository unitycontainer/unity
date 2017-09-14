// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Unity.InterceptionExtension.Tests.MatchingRules.SeparateTopLevel;
using Unity.InterceptionExtension.Tests.MatchingRules.TopLevel;
using Unity.InterceptionExtension.Tests.MatchingRules.TopLevel.SecondLevel;
using Unity.InterceptionExtension.Tests.MatchingRules.TopLevel.SecondLevel.ThirdLevel;
using Unity.InterceptionExtension.Tests.MatchingRules.TopLevelTwo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[module: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1403:FileMayOnlyContainASingleNamespace", Justification = "Test needs multiple namespaces so keep the namespaces and test together")]

namespace Unity.InterceptionExtension.Tests.MatchingRules
{
    /// <summary>
    /// Unit tests for NamespaceMatchingRule
    /// </summary>
    [TestClass]
    public class NamespaceMatchingRuleFixture
    {
        private const string TopLevelNamespace =
            "Unity.InterceptionExtension.Tests.MatchingRules.TopLevel";

        private const string TopLevelNamespaceWildcard =
            "Unity.InterceptionExtension.Tests.MatchingRules.TopLevel.*";

        private const string SecondLevelNamespace =
            "Unity.InterceptionExtension.Tests.MatchingRules.TopLevel.SecondLevel";

        private const string ThirdLevelNamespace =
            "Unity.InterceptionExtension.Tests.MatchingRules.TopLevel.SecondLevel.ThirdLevel";

        private const string TopLevelTwoNamespace =
            "Unity.InterceptionExtension.Tests.MatchingRules.TopLevelTwo";

        [TestMethod]
        public void ShouldMatchWhenInExactNamespace()
        {
            TestMatch(TopLevelNamespace, typeof(TopLevelTarget), true);
        }

        [TestMethod]
        public void ShouldNotMatchWhenInChildNamespace()
        {
            TestMatch(TopLevelNamespace, typeof(SecondLevelTarget), false);
        }

        [TestMethod]
        public void ShouldMatchWithIgnoreCaseTurnedOn()
        {
            TestMatch(TopLevelNamespace, true, typeof(TopLevelTarget), true);
        }

        [TestMethod]
        public void ShouldNotMatchWhenInChildNamespaceWithIgnoreCase()
        {
            TestMatch(TopLevelNamespace, true, typeof(SecondLevelTarget), false);
        }

        [TestMethod]
        public void ShouldMatchInExactNamespaceWithWildcard()
        {
            TestMatch(TopLevelNamespaceWildcard, typeof(TopLevelTarget), true);
        }

        [TestMethod]
        public void ShouldMatchInChildNamespaceWithWildcard()
        {
            TestMatch(TopLevelNamespaceWildcard, typeof(SecondLevelTarget), true);
        }

        [TestMethod]
        public void ShouldNotMatchIfNotInSameOrChildNamespace()
        {
            TestMatch(TopLevelNamespaceWildcard, typeof(SeparateTarget), false);
        }

        [TestMethod]
        public void ShouldNotMatchIfNamespaceOfTargetStartsWithSameStringAsMatchingNamespace()
        {
            TestMatch(TopLevelNamespaceWildcard, typeof(TopLevelTwoTarget), false);
        }

        [TestMethod]
        public void ShouldNotMatchIfTargetIsInParentNamespace()
        {
            TestMatch(SecondLevelNamespace, typeof(TopLevelTarget), false);
        }

        [TestMethod]
        public void ShouldMatchWithWildcardMultipleLevelsDeep()
        {
            TestMatch(TopLevelNamespaceWildcard, typeof(ThirdLevelTarget), true);
        }

        [TestMethod]
        public void ShouldMatchWithMultipleMatchOptions()
        {
            IMatchingRule rule =
                new NamespaceMatchingRule(new MatchingInfo[]
                                              {
                                                  new MatchingInfo(TopLevelNamespace),
                                                  new MatchingInfo(ThirdLevelNamespace),
                                                  new MatchingInfo(TopLevelTwoNamespace.ToUpperInvariant(), true)
                                              });

            Assert.IsTrue(rule.Matches(GetTargetMethod(typeof(TopLevelTarget))));
            Assert.IsFalse(rule.Matches(GetTargetMethod(typeof(SecondLevelTarget))));
            Assert.IsTrue(rule.Matches(GetTargetMethod(typeof(ThirdLevelTarget))));
            Assert.IsFalse(rule.Matches(GetTargetMethod(typeof(SeparateTarget))));
            Assert.IsTrue(rule.Matches(GetTargetMethod(typeof(TopLevelTwoTarget))));
        }

        private void TestMatch(string namespaceName,
                       bool ignoreCase,
                       Type targetType,
                       bool shouldMatch)
        {
            NamespaceMatchingRule rule = new NamespaceMatchingRule(namespaceName, ignoreCase);
            MethodInfo methodToMatch = targetType.GetMethod("TargetMethod");
            Assert.AreEqual(shouldMatch, rule.Matches(methodToMatch));
        }

        private void TestMatch(string namespaceName,
                       Type targetType,
                       bool shouldMatch)
        {
            TestMatch(namespaceName, false, targetType, shouldMatch);
        }

        private MethodInfo GetTargetMethod(Type t)
        {
            return t.GetMethod("TargetMethod");
        }
    }

    namespace TopLevel
    {
        internal class TopLevelTarget
        {
            public void TargetMethod() { }
        }

        namespace SecondLevel
        {
            internal class SecondLevelTarget
            {
                public void TargetMethod() { }
            }

            namespace ThirdLevel
            {
                internal class ThirdLevelTarget
                {
                    public void TargetMethod() { }
                }
            }
        }
    }

    namespace SeparateTopLevel
    {
        internal class SeparateTarget
        {
            public void TargetMethod() { }
        }
    }

    namespace TopLevelTwo
    {
        internal class TopLevelTwoTarget
        {
            public void TargetMethod() { }
        }
    }
}
