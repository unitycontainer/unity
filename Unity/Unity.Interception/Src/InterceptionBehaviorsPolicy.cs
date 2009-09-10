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
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// An <see cref="IInterceptionBehaviorsPolicy"/> that accumulates a sequence of 
    /// <see cref="IInterceptionBehaviorDescriptor"/> instances representing the behaviors for an intercepted object.
    /// </summary>
    public class InterceptionBehaviorsPolicy : IInterceptionBehaviorsPolicy
    {
        private readonly List<IInterceptionBehaviorDescriptor> interceptionBehaviorDescriptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehaviorsPolicy"/> class.
        /// </summary>
        public InterceptionBehaviorsPolicy()
        {
            this.interceptionBehaviorDescriptors = new List<IInterceptionBehaviorDescriptor>();
        }

        /// <summary>
        /// Gets the <see cref="IInterceptionBehaviorDescriptor"/> instances accumulated by this policy.
        /// </summary>
        public IEnumerable<IInterceptionBehaviorDescriptor> InterceptionBehaviorDescriptors
        {
            get { return this.interceptionBehaviorDescriptors; }
        }

        internal void AddInterceptorDescriptor(IInterceptionBehaviorDescriptor interceptionBehaviorDescriptor)
        {
            this.interceptionBehaviorDescriptors.Add(interceptionBehaviorDescriptor);
        }

        internal static InterceptionBehaviorsPolicy GetOrCreate(
            IPolicyList policies,
            Type typeToCreate,
            string name)
        {
            NamedTypeBuildKey key = new NamedTypeBuildKey(typeToCreate, name);
            IInterceptionBehaviorsPolicy policy =
                policies.GetNoDefault<IInterceptionBehaviorsPolicy>(key, false);
            if ((policy == null) || !(policy is InterceptionBehaviorsPolicy))
            {
                policy = new InterceptionBehaviorsPolicy();
                policies.Set<IInterceptionBehaviorsPolicy>(policy, key);
            }
            return (InterceptionBehaviorsPolicy)policy;
        }
    }
}
