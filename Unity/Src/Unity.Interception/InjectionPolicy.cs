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
using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Base class for Policies that specifies which handlers apply to which methods of an object.
    /// </summary>
    /// <remarks>
    /// <para>This base class always enforces the 
    /// <see cref="ApplyNoPoliciesMatchingRule"/> before
    /// passing the checks onto derived classes. This way, derived classes do not need to
    /// worry about implementing this check.</para>
    /// <para>It also means that derived classes cannot override this rule. This is considered a feature.</para></remarks>
    public abstract class InjectionPolicy
    {
        private readonly string name;
        private readonly IMatchingRule doesNotHaveNoPoliciesAttributeRule;

        /// <summary>
        /// Creates a new empty Policy.
        /// </summary>
        protected InjectionPolicy()
            : this("Unnamed policy")
        {
        }

        /// <summary>
        /// Creates a new empty policy with the given name.
        /// </summary>
        /// <param name="name">Name of the policy.</param>
        protected InjectionPolicy(string name)
        {
            this.name = name;
            doesNotHaveNoPoliciesAttributeRule = new ApplyNoPoliciesMatchingRule();
        }

        /// <summary>
        /// Gets the name of this policy.
        /// </summary>
        /// <value>The name of the policy.</value>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Checks to see if this policy contains rules that match members
        /// of the given type.
        /// </summary>
        /// <param name="t">Type to check.</param>
        /// <returns>true if any of the members of type t match the ruleset for this policy, false if not.</returns>
        public bool AppliesTo(Type t)
        {
            if (Array.Exists(t.GetMethods(),
                delegate(MethodInfo method) { return doesNotHaveNoPoliciesAttributeRule.Matches(method); }))
            {
                return DoesApplyTo(t);
            }
            return false;
        }

        /// <summary>
        /// Checks if the rules in this policy match the given member info.
        /// </summary>
        /// <param name="member">MemberInfo to check against.</param>
        /// <returns>true if ruleset matches, false if it does not.</returns>
        public bool Matches(MethodBase member)
        {
            return doesNotHaveNoPoliciesAttributeRule.Matches(member) && DoesMatch(member);
        }

        /// <summary>
        /// Returns ordered collection of handlers in order that apply to the given member.
        /// </summary>
        /// <param name="member">Member that may or may not be assigned handlers by this policy.</param>
        /// <param name="container">The <see cref="IUnityContainer"/> to use when creating handlers,
        /// if necessary.</param>
        /// <returns>Collection of handlers (possibly empty) that apply to this member.</returns>
        public virtual IEnumerable<ICallHandler> GetHandlersFor(MethodBase member, IUnityContainer container)
        {
            if (doesNotHaveNoPoliciesAttributeRule.Matches(member))
            {
                foreach (MethodBase method in GetMethodSet(member))
                {
                    List<ICallHandler> handlers = new List<ICallHandler>(DoGetHandlersFor(method, container));
                    if (handlers.Count > 0)
                    {
                        foreach (ICallHandler handler in handlers)
                        {
                            yield return handler;
                        }
                        yield break;
                    }
                }
            }
        }

        /// <summary>
        /// Given a method on an object, return the set of MethodBases for that method,
        /// plus any inteface methods that the member implements.
        /// </summary>
        /// <param name="member">Member to get Method Set for.</param>
        /// <returns>The set of methods</returns>
        protected static IEnumerable<MethodBase> GetMethodSet(MethodBase member)
        {
            List<MethodBase> methodSet = new List<MethodBase>(new MethodBase[] { member });
            if (member.DeclaringType != null)
            {
                foreach (Type itf in member.DeclaringType.GetInterfaces())
                {
                    InterfaceMapping mapping = member.DeclaringType.GetInterfaceMap(itf);
                    for (int i = 0; i < mapping.TargetMethods.Length; ++i)
                    {
                        if (mapping.TargetMethods[i] == member)
                        {
                            methodSet.Add(mapping.InterfaceMethods[i]);
                        }
                    }
                }
            }

            return methodSet;
        }

        /// <summary>
        /// Derived classes implement this method to calculate if the policy
        /// provides any handlers for any methods on the given type.
        /// </summary>
        /// <param name="t">Type to check.</param>
        /// <returns>true if the policy applies to this type, false if it does not.</returns>
        protected abstract bool DoesApplyTo(Type t);

        /// <summary>
        /// Derived classes implement this method to calculate if the policy
        /// will provide any handler to the specified member.
        /// </summary>
        /// <param name="member">Member to check.</param>
        /// <returns>true if policy applies to this member, false if not.</returns>
        protected abstract bool DoesMatch(MethodBase member);

        /// <summary>
        /// Derived classes implement this method to supply the list of handlers for
        /// this specific member.
        /// </summary>
        /// <param name="member">Member to get handlers for.</param>
        /// <param name="container">The <see cref="IUnityContainer"/> to use when creating handlers,
        /// if necessary.</param>
        /// <returns>Enumerable collection of handlers for this method.</returns>
        protected abstract IEnumerable<ICallHandler> DoGetHandlersFor(
            MethodBase member,
            IUnityContainer container);
    }
}
