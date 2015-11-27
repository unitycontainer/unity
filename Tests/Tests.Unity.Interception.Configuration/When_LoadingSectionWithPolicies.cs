// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using Unity.InterceptionExtension.Configuration.Tests.ConfigFiles;
using Unity.TestSupport;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Configuration.Tests
{
    [TestClass]
    public class When_LoadingSectionWithPolicies : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingSectionWithPolicies()
            : base("Policies")
        {
        }

        private InterceptionElement GetInterceptionElement(string containerName)
        {
            return (InterceptionElement)section.Containers[containerName].ConfiguringElements[0];
        }

        [TestMethod]
        public void Then_ElementWithEmptyPoliciesLoads()
        {
            Assert.AreEqual(0, this.GetInterceptionElement("emptyPolicies").Policies.Count);
        }

        [TestMethod]
        public void Then_PoliciesAreLoadedFromExplicitCollection()
        {
            this.GetInterceptionElement("explicitPolicyCollection").Policies.Select(p => p.Name)
                .AssertContainsExactly("policyOne", "policyTwo");
        }

        [TestMethod]
        public void Then_PoliciesAreLoadedFromImplicitCollection()
        {
            this.GetInterceptionElement("implicitPolicyCollection").Policies.Select(p => p.Name)
                .AssertContainsExactly("policyA", "policyB");
        }

        [TestMethod]
        public void Then_PoliciesLoadNameOnlyMatchingRules()
        {
            var interceptionElement = this.GetInterceptionElement("policyWithNamedMatchingRules");
            var policyOne = interceptionElement.Policies["policyOne"];

            policyOne.MatchingRules.Select(mr => mr.Name).AssertContainsExactly("ruleOne", "ruleTwo");
        }

        [TestMethod]
        public void Then_CanDefinePolicyWithMatchingRuleAndCallHandler()
        {
            var interceptionElement = this.GetInterceptionElement("policyWithGivenRulesAndHandlersTypes");
            var policyOne = interceptionElement.Policies["policyOne"];

            policyOne.MatchingRules.Select(mr => mr.Name).AssertContainsExactly("rule1");
            policyOne.MatchingRules.Select(mr => mr.TypeName).AssertContainsExactly("AlwaysMatchingRule");

            policyOne.CallHandlers.Select(ch => ch.Name).AssertContainsExactly("handler1");
            policyOne.CallHandlers.Select(ch => ch.TypeName).AssertContainsExactly("GlobalCountCallHandler");
        }

        [TestMethod]
        public void Then_CanLoadPolicyWithMultipleHandlers()
        {
            var interceptionElement = this.GetInterceptionElement("policyWithExternallyConfiguredRulesAndHandlers");
            var policyOne = interceptionElement.Policies["policyOne"];

            policyOne.MatchingRules.Select(mr => mr.Name).AssertContainsExactly("rule1");
            policyOne.MatchingRules.Select(mr => mr.TypeName).AssertContainsExactly(String.Empty);

            policyOne.CallHandlers.Select(ch => ch.Name).AssertContainsExactly("handler1", "handler2");
            policyOne.CallHandlers.Select(ch => ch.TypeName).AssertContainsExactly(String.Empty, String.Empty);
        }
    }
}
