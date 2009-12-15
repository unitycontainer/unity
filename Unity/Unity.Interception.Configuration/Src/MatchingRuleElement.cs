using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity.Configuration;

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
                        element => element.GetInjectionMembers(container, typeof (IMatchingRule), matchingRuleType, Name));

                policyDefinition.AddMatchingRule(matchingRuleType, Name, lifetime, injectionMembers.ToArray());
            }
        }
    }
}