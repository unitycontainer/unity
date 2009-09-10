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

using System.Collections.Generic;

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
            attributeMatchRule = new AttributeDrivenPolicyMatchingRule();
        }

        /// <summary>
        /// Derived classes implement this method to calculate if the policy
        /// will provide any handler to the specified member.
        /// </summary>
        /// <param name="member">Member to check.</param>
        /// <returns>true if policy applies to this member, false if not.</returns>
        protected override bool DoesMatch(MethodImplementationInfo member)
        {
            bool matchesInterface = member.InterfaceMethodInfo != null ? attributeMatchRule.Matches(member.InterfaceMethodInfo) : false;
            bool matchesImplementation = attributeMatchRule.Matches(member.ImplementationMethodInfo);

            return matchesInterface | matchesImplementation;
        }

        /// <summary>
        /// Derived classes implement this method to supply the list of handlers for
        /// this specific member.
        /// </summary>
        /// <param name="member">Member to get handlers for.</param>
        /// <param name="container">The <see cref="IUnityContainer"/> to use when creating handlers,
        /// if necessary.</param>
        /// <returns>Enumerable collection of handlers for this method.</returns>
        protected override IEnumerable<ICallHandler> DoGetHandlersFor(MethodImplementationInfo member, IUnityContainer container)
        {
            if (member.InterfaceMethodInfo != null)
            {
                foreach (HandlerAttribute attr in ReflectionHelper.GetAllAttributes<HandlerAttribute>(member.InterfaceMethodInfo, true))
                {
                    yield return attr.CreateHandler(container);
                }
            }
            foreach (HandlerAttribute attr in ReflectionHelper.GetAllAttributes<HandlerAttribute>(member.ImplementationMethodInfo, true))
            {
                yield return attr.CreateHandler(container);
            }
        }
    }
}
