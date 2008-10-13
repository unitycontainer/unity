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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A collection of Policy objects. The policies within a PolicySet combine using
    /// an "or" operation.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710", Justification = "Using alternative suffix 'Set'.")]
    public class PolicySet : List<InjectionPolicy>
    {
        /// <summary>
        /// Creates a new <see cref="PolicySet"/> containing the given policies.
        /// </summary>
        /// <param name="policies">Policies to put into the policy set.</param>
        public PolicySet(params InjectionPolicy[] policies)
        {
            AddRange(policies);
        }

        /// <summary>
        /// Gets the policies that apply to the given member.
        /// </summary>
        /// <param name="member">Member to get policies for.</param>
        /// <returns>Collection of policies that apply to this member.</returns>
        public IEnumerable<InjectionPolicy> GetPoliciesFor(MethodImplementationInfo member)
        {
            foreach (InjectionPolicy policy in this)
            {
                if (policy.Matches(member))
                {
                    yield return policy;
                }
            }
        }

        /// <summary>
        /// Gets the policies in the <see cref="PolicySet"/> that do not
        /// apply to the given member.
        /// </summary>
        /// <param name="member">Member to check.</param>
        /// <returns>Collection of policies that do not apply to <paramref name="member"/>.</returns>
        public IEnumerable<InjectionPolicy> GetPoliciesNotFor(MethodImplementationInfo member)
        {
            foreach (InjectionPolicy policy in this)
            {
                if (!policy.Matches(member))
                {
                    yield return policy;
                }
            }
        }

        /// <summary>
        /// Gets the handlers that apply to the given member based on all policies in the <see cref="PolicySet"/>.
        /// </summary>
        /// <param name="member">Member to get handlers for.</param>
        /// <param name="container">The <see cref="IUnityContainer"/> to use when creating handlers,
        /// if necessary.</param>
        /// <returns>Collection of call handlers for <paramref name="member"/>.</returns>
        public IEnumerable<ICallHandler> GetHandlersFor(MethodImplementationInfo member, IUnityContainer container)
        {
            return new List<ICallHandler>(CalculateHandlersFor(this, member, container));
        }

        internal static IEnumerable<ICallHandler> CalculateHandlersFor(
            IEnumerable<InjectionPolicy> policies, 
            MethodImplementationInfo member, 
            IUnityContainer container)
        {
            List<ICallHandler> ordered = new List<ICallHandler>();
            List<ICallHandler> nonOrdered = new List<ICallHandler>();

            foreach (InjectionPolicy p in policies)
            {
                foreach (ICallHandler handler in p.GetHandlersFor(member, container))
                {
                    if (handler.Order != 0)
                    {
                        bool inserted = false;
                        // add in order to ordered
                        for (int i = ordered.Count - 1; i >= 0; i--)
                        {
                            if (ordered[i].Order <= handler.Order)
                            {
                                ordered.Insert(i + 1, handler);
                                inserted = true;
                                break;
                            }
                        }
                        if (!inserted)
                        {
                            ordered.Insert(0, handler);
                        }
                    }
                    else
                    {
                        nonOrdered.Add(handler);
                    }
                }
            }
            ordered.AddRange(nonOrdered);
            return ordered;
        }
    }
}
