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
    /// A <see cref="InjectionPolicy"/> class that reads and constructs handlers
    /// based on <see cref="HandlerAttribute"/> on the target.
    /// </summary>
    public class AttributeDrivenPolicy : InjectionPolicy
    {
        private readonly AttributeDrivenPolicyMatchingRule attributeMatchRule;

        /// <summary>
        /// Constructs a new instance of the <see cref="AttributeDrivenPolicy"/>.
        /// </summary>
        public AttributeDrivenPolicy()
            : base("Attribute Driven Policy")
        {
            this.attributeMatchRule = new AttributeDrivenPolicyMatchingRule();
        }

        /// <summary>
        /// Returns ordered collection of handlers in order that apply to the given member.
        /// </summary>
        /// <param name="member">Member that may or may not be assigned handlers by this policy.</param>
        /// <param name="container">The <see cref="IUnityContainer"/> to use when creating handlers,
        /// if necessary.</param>
        /// <returns>Collection of handlers (possibly empty) that apply to this member.</returns>
        public override IEnumerable<ICallHandler> GetHandlersFor(MethodBase member, IUnityContainer container)
        {
            if (Matches(member))
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
                    }
                }
            }
        }

        /// <summary>
        /// Derived classes implement this method to calculate if the policy
        /// provides any handlers for any methods on the given type.
        /// </summary>
        /// <param name="t">Type to check.</param>
        /// <returns>true if the policy applies to this type, false if it does not.</returns>
        protected override bool DoesApplyTo(Type t)
        {
            return Array.Exists(t.GetMethods(),
                delegate(MethodInfo method) { return Matches(method); });
        }

        /// <summary>
        /// Derived classes implement this method to calculate if the policy
        /// will provide any handler to the specified member.
        /// </summary>
        /// <param name="member">Member to check.</param>
        /// <returns>true if policy applies to this member, false if not.</returns>
        protected override bool DoesMatch(MethodBase member)
        {
            foreach (MethodBase method in GetMethodSet(member))
            {
                if (attributeMatchRule.Matches(method))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Derived classes implement this method to supply the list of handlers for
        /// this specific member.
        /// </summary>
        /// <param name="member">Member to get handlers for.</param>
        /// <param name="container">The <see cref="IUnityContainer"/> to use when creating handlers,
        /// if necessary.</param>
        /// <returns>Enumerable collection of handlers for this method.</returns>
        protected override IEnumerable<ICallHandler> DoGetHandlersFor(
            MethodBase member,
            IUnityContainer container)
        {
            foreach (HandlerAttribute attr in ReflectionHelper.GetAllAttributes<HandlerAttribute>(member, true))
            {
                yield return attr.CreateHandler(container);
            }
        }
    }
}
