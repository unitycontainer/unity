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

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// A <see cref="ConfigurationElement"/> class used to manage the contents
    /// of a &lt;matchingRule&gt; node in the configuration file for the interception extension's section.
    /// </summary>
    /// <remarks>
    /// This node contains the information necessary to describe a <see cref="IMatchingRule"/>
    /// and can configure a <see cref="RuleDrivenPolicy"/> to include the it.
    /// </remarks>
    /// <seealso cref="RuleDrivenPolicyElementConfigurationElement"/>
    public class MatchingRuleConfigurationElement : RuleDrivenPolicyElementConfigurationElement
    {
        internal void Configure(PolicyDefinition policyDefinition)
        {
            if (string.IsNullOrEmpty(this.TypeName))
            {
                policyDefinition.AddMatchingRule(this.Name);
            }
            else
            {
                policyDefinition.AddMatchingRule(
                    this.Type,
                    this.Name,
                    this.Lifetime != null ? this.Lifetime.CreateLifetimeManager() : null,
                    this.Injection != null ? this.Injection.GetInjectionMembers() : new InjectionMember[0]);
            }
        }
    }
}
