using System.Linq;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
{
    [TestClass]
    public class When_LoadingSectionWithPolicies :SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingSectionWithPolicies() : base("Policies")
        {
        }

        private InterceptionElement GetInterceptionElement(string containerName)
        {
            return (InterceptionElement)Section.Containers[containerName].ConfiguringElements[0];
        }

        [TestMethod]
        public void Then_ElementWithEmptyPoliciesLoads()
        {
            Assert.AreEqual(0, GetInterceptionElement("emptyPolicies").Policies.Count);
        }

        [TestMethod]
        public void Then_PoliciesAreLoadedFromExplicitCollection()
        {
            GetInterceptionElement("explicitPolicyCollection").Policies.Select(p => p.Name)
                .AssertContainsExactly("policyOne", "policyTwo");
        }

        [TestMethod]
        public void Then_PoliciesAreLoadedFromImplicitCollection()
        {
            
            GetInterceptionElement("implicitPolicyCollection").Policies.Select(p => p.Name)
                .AssertContainsExactly("policyA", "policyB");
        }

        [TestMethod]
        public void Then_PoliciesLoadNameOnlyMatchingRules()
        {
            var interceptionElement = GetInterceptionElement("policyWithNamedMatchingRules");
            var policyOne = interceptionElement.Policies["policyOne"];

            policyOne.MatchingRules.Select(mr => mr.Name).AssertContainsExactly("ruleOne", "ruleTwo");

        }

        [TestMethod]
        public void Then_CanDefinePolicyWithMatchingRuleAndCallHandler()
        {
            var interceptionElement = GetInterceptionElement("policyWithGivenRulesAndHandlersTypes");
            var policyOne = interceptionElement.Policies["policyOne"];

            policyOne.MatchingRules.Select(mr=>mr.Name).AssertContainsExactly("rule1");
            policyOne.MatchingRules.Select(mr => mr.TypeName).AssertContainsExactly("AlwaysMatchingRule");

            policyOne.CallHandlers.Select(ch => ch.Name).AssertContainsExactly("handler1");
            policyOne.CallHandlers.Select(ch => ch.TypeName).AssertContainsExactly("GlobalCountCallHandler");
        }

        [TestMethod]
        public void Then_CanLoadPolicyWithMultipleHandlers()
        {
            var interceptionElement = GetInterceptionElement("policyWithExternallyConfiguredRulesAndHandlers");
            var policyOne = interceptionElement.Policies["policyOne"];

            policyOne.MatchingRules.Select(mr => mr.Name).AssertContainsExactly("rule1");
            policyOne.MatchingRules.Select(mr => mr.TypeName).AssertContainsExactly("");

            policyOne.CallHandlers.Select(ch => ch.Name).AssertContainsExactly("handler1", "handler2");
            policyOne.CallHandlers.Select(ch => ch.TypeName).AssertContainsExactly("", "");
        }

    }
}
