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

using System.Configuration;
using Microsoft.Practices.Unity.Configuration;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class InterceptionPolicyConfigurationElement : TypeResolvingConfigurationElement, IContainerConfigurationCommand
    {
        /// <summary>
        /// Returns name of the element.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("callHandlers")]
        [ConfigurationCollection(typeof(CallHandlerConfigurationElementCollection), AddItemName = "callHandler")]
        public CallHandlerConfigurationElementCollection CallHandlers
        {
            get
            {
                CallHandlerConfigurationElementCollection policies =
                    (CallHandlerConfigurationElementCollection)this["callHandlers"];
                policies.TypeResolver = TypeResolver;
                return policies;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("matchingRules")]
        [ConfigurationCollection(typeof(MatchingRuleConfigurationElementCollection), AddItemName = "matchingRule")]
        public MatchingRuleConfigurationElementCollection MatchingRules
        {
            get
            {
                MatchingRuleConfigurationElementCollection policies =
                    (MatchingRuleConfigurationElementCollection)this["matchingRules"];
                policies.TypeResolver = TypeResolver;
                return policies;
            }
        }

        #region IContainerConfigurationCommand Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public void Configure(IUnityContainer container)
        {
            PolicyDefinition policyDefinition = container.Configure<Interception>().AddPolicy(this.Name);

            foreach (MatchingRuleConfigurationElement ruleElement in MatchingRules)
            {
                ruleElement.Configure(policyDefinition);
            }

            foreach (CallHandlerConfigurationElement handlerElement in CallHandlers)
            {
                handlerElement.Configure(policyDefinition);
            }
        }

        #endregion
    }
}
