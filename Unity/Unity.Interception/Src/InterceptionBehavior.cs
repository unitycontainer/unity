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
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Stores information about a single <see cref="IInterceptionBehavior"/> to be used on an intercepted object and
    /// configures a container accordingly.
    /// </summary>
    /// <seealso cref="IInterceptionBehavior"/>
    /// <seealso cref="IInterceptionBehaviorDescriptor"/>
    public class InterceptionBehavior : InterceptionMember
    {
        private readonly IInterceptionBehaviorDescriptor interceptionBehaviorDescriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// <see cref="IInterceptionBehaviorDescriptor"/>.
        /// </summary>
        /// <param name="interceptionBehaviorDescriptor">A descriptor representing the interception behavior to use.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptionBehaviorDescriptor"/> is
        /// <see langword="null"/>.</exception>
        public InterceptionBehavior(IInterceptionBehaviorDescriptor interceptionBehaviorDescriptor)
        {
            Guard.ArgumentNotNull(interceptionBehaviorDescriptor, "interceptionBehaviorDescriptor");

            this.interceptionBehaviorDescriptor = interceptionBehaviorDescriptor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// <see cref="IInterceptionBehavior"/>.
        /// </summary>
        /// <param name="interceptionBehavior">The interception behavior to use.</param>
        public InterceptionBehavior(IInterceptionBehavior interceptionBehavior)
            : this(new ExistingInterceptionBehaviorDescriptor(interceptionBehavior))
        { }

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the container to use the represented 
        /// <see cref="IInterceptionBehavior"/> for the supplied parameters.
        /// </summary>
        /// <param name="serviceType">Interface being registered.</param>
        /// <param name="implementationType">Type to register.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            InterceptionBehaviorsPolicy policy =
                InterceptionBehaviorsPolicy.GetOrCreate(policies, implementationType, name);
            policy.AddInterceptorDescriptor(this.interceptionBehaviorDescriptor);
        }
    }
}
