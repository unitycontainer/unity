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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_SerializingPolicies
    /// </summary>
    [TestClass]
    public class When_SerializingPolicies : SerializationFixture
    {
        [TestMethod]
        public void Then_EmptyPoliciesAreSerialized()
        {
            var loadedConfig = SerializeAndLoadConfig("EmptyPolicies.config", c =>
            {
                c.ConfiguringElements.Add(new InterceptionElement());
            });

            var policies = loadedConfig.Containers.Default.ConfiguringElements[0] as InterceptionElement;
            Assert.IsNotNull(policies);
            Assert.AreEqual(0, policies.Policies.Count);
        }

        [TestMethod]
        public void Then_PoliciesWithNoContentsAreSerialized()
        {
            var loadedConfig = SerializeAndLoadConfig("EmptyPolicies.config", c =>
            {
                var interceptionElement = new InterceptionElement();
                interceptionElement.Policies.Add(new PolicyElement()
                {
                    Name = "Policy1"
                });
                interceptionElement.Policies.Add(new PolicyElement()
                {
                    Name = "Policy2"
                });
                c.ConfiguringElements.Add(interceptionElement);
            });

            var policies = (InterceptionElement)loadedConfig.Containers.Default.ConfiguringElements[0];

            policies.Policies.Select(p => p.Name)
                .AssertContainsExactly("Policy1", "Policy2");
        }

        [TestMethod]
        public void Then_MatchingRuleNamesAreSerialized()
        {
            var loadedConfig = SerializeAndLoadConfig("MatchingRules.config", CreateConfigWithMatchingRules);

            var interception = (InterceptionElement) loadedConfig.Containers.Default.ConfiguringElements[0];
            interception.Policies[0].MatchingRules.Select(mr => mr.Name)
                .AssertContainsExactly("NameOnly", "NameAndType", "RuleWithLifetime", "RuleWithElements");
        }

        [TestMethod]
        public void Then_MatchingRuleTypeNamesAreSerialized()
        {
            var loadedConfig = SerializeAndLoadConfig("MatchingRules.config", CreateConfigWithMatchingRules);
            var interception = (InterceptionElement)loadedConfig.Containers.Default.ConfiguringElements[0];
            interception.Policies[0].MatchingRules.Select(mr => mr.TypeName)
                .AssertContainsExactly("", "AlwaysMatchingRule", "AlwaysMatchingRule", "AlwaysMatchingRule");
        }

        [TestMethod]
        public void Then_MatchingRuleLifetimesAreSerialized()
        {
            var loadedConfig = SerializeAndLoadConfig("MatchingRules.config", CreateConfigWithMatchingRules);
            var interception = (InterceptionElement)loadedConfig.Containers.Default.ConfiguringElements[0];

            var lifetime = interception.Policies[0].MatchingRules
                .Where(mr => mr.Name == "RuleWithLifetime")
                .Select(mr => mr.Lifetime)
                .First();

            Assert.AreEqual("singleton", lifetime.TypeName);
        }

        [TestMethod]
        public void Then_MatchingRuleInjectionMembersAreSerialized()
        {
            var loadedConfig = SerializeAndLoadConfig("MatchingRules.config", CreateConfigWithMatchingRules);
            var interception = (InterceptionElement)loadedConfig.Containers.Default.ConfiguringElements[0];

            var injectionMembers = interception.Policies[0].MatchingRules
                .Where(mr => mr.Name == "RuleWithElements")
                .First().Injection;

            injectionMembers.Select(m => m.GetType())
                .AssertContainsExactly(typeof(ConstructorElement), typeof(PropertyElement));

            Assert.AreEqual("MyProp", injectionMembers.OfType<PropertyElement>().First().Name);
        }

        [TestMethod]
        public void Then_CallHandlerNamesAreSerialized()
        {
            var loadedConfig = SerializeAndLoadConfig("CallHandlers.config", CreateConfigWithCallHandlers);

            var interception = (InterceptionElement)loadedConfig.Containers.Default.ConfiguringElements[0];
            interception.Policies[0].CallHandlers.Select(ch => ch.Name)
                .AssertContainsExactly("NamedRule", "NameAndType", "HandlerWithLifetime", "HandlerWithElements");
        }

        [TestMethod]
        public void Then_CallHandlerTypeNamesAreSerialized()
        {
            var loadedConfig = SerializeAndLoadConfig("CallHandlers.config", CreateConfigWithCallHandlers);
            var interception = (InterceptionElement)loadedConfig.Containers.Default.ConfiguringElements[0];

            interception.Policies[0].CallHandlers.Select(ch => ch.TypeName)
                .AssertContainsExactly("", "DoMoreRule", "DoSomethingRule", "CallCountHandler");
        }

        [TestMethod]
        public void Then_CallHandlerLifetimesAreSerialized()
        {
            var loadedConfig = SerializeAndLoadConfig("CallHandlers.config", CreateConfigWithCallHandlers);
            var interception = (InterceptionElement)loadedConfig.Containers.Default.ConfiguringElements[0];

            var lifetime = interception.Policies[0].CallHandlers
                .Where(ch => ch.Name == "HandlerWithLifetime")
                .Select(ch => ch.Lifetime)
                .First();

            Assert.AreEqual("singleton", lifetime.TypeName);
        }

        [TestMethod]
        public void Then_CallHandlerInjectionMembersAreSerialized()
        {
            var loadedConfig = SerializeAndLoadConfig("CallHandlers.config", CreateConfigWithCallHandlers);
            var interception = (InterceptionElement)loadedConfig.Containers.Default.ConfiguringElements[0];

            var injectionMembers = interception.Policies[0].CallHandlers
                .Where(ch => ch.Name == "HandlerWithElements")
                .First().Injection;

            injectionMembers.Select(m => m.GetType())
                .AssertContainsExactly(typeof(ConstructorElement), typeof(PropertyElement));

            Assert.AreEqual("MyProp", injectionMembers.OfType<PropertyElement>().First().Name);
        }
    
        private static void CreateConfigWithMatchingRules(ContainerElement c)
        {
            var interceptionElement = new InterceptionElement();
            var policy = new PolicyElement() {Name = "PolicyOne"};
            policy.MatchingRules.Add(new MatchingRuleElement()
            {
                Name = "NameOnly"
            });

            policy.MatchingRules.Add(new MatchingRuleElement()
            {
                Name = "NameAndType",
                TypeName = "AlwaysMatchingRule"
            });

            policy.MatchingRules.Add(new MatchingRuleElement
            {
                Name = "RuleWithLifetime",
                TypeName = "AlwaysMatchingRule",
                Lifetime = new LifetimeElement()
                {
                    TypeName = "singleton"
                }
            });

            var ruleWithMembers = new MatchingRuleElement
            {
                Name = "RuleWithElements",
                TypeName = "AlwaysMatchingRule"
            };
            ruleWithMembers.Injection.Add(new ConstructorElement());
            ruleWithMembers.Injection.Add(new PropertyElement() { Name = "MyProp" } );

            policy.MatchingRules.Add(ruleWithMembers);

            interceptionElement.Policies.Add(policy);

            c.ConfiguringElements.Add(interceptionElement);
        }

        private static void CreateConfigWithCallHandlers(ContainerElement c)
        {
            var interceptionElement = new InterceptionElement();
            var policy = new PolicyElement() { Name = "PolicyOne" };
            policy.MatchingRules.Add(new MatchingRuleElement() { Name =  "All", TypeName = "AlwaysMatchingRule" });

            policy.CallHandlers.Add(new CallHandlerElement()
            {
                Name = "NamedRule"
            });

            policy.CallHandlers.Add(new CallHandlerElement()
            {
                Name = "NameAndType",
                TypeName = "DoMoreRule"
            });

            policy.CallHandlers.Add(new CallHandlerElement()
            {
                Name = "HandlerWithLifetime",
                TypeName = "DoSomethingRule",
                Lifetime = new LifetimeElement()
                {
                    TypeName = "singleton"
                }
            });

            var handlerWithMembers = new CallHandlerElement()
            {
                Name = "HandlerWithElements",
                TypeName = "CallCountHandler"
            };
            handlerWithMembers.Injection.Add(new ConstructorElement());
            handlerWithMembers.Injection.Add(new PropertyElement() { Name = "MyProp" });

            policy.CallHandlers.Add(handlerWithMembers);

            interceptionElement.Policies.Add(policy);

            c.ConfiguringElements.Add(interceptionElement);
        }
    }
}
