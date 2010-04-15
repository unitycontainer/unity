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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// A configuration element representing a matching rule.
    /// </summary>
    public class MatchingRuleElement : PolicyChildElement
    {
        internal void Configure(IUnityContainer container, PolicyDefinition policyDefinition)
        {
            if (string.IsNullOrEmpty(TypeName))
            {
                policyDefinition.AddMatchingRule(Name);
            }
            else
            {
                Type matchingRuleType = TypeResolver.ResolveType(TypeName);
                LifetimeManager lifetime = Lifetime.CreateLifetimeManager();
                IEnumerable<InjectionMember> injectionMembers =
                    Injection.SelectMany(
                        element => element.GetInjectionMembers(container, typeof(IMatchingRule), matchingRuleType, Name));

                policyDefinition.AddMatchingRule(matchingRuleType, lifetime, injectionMembers.ToArray());
            }
        }
    }
}
