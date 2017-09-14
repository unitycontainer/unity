// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using ObjectBuilder2;

namespace Unity.InterceptionExtension
{
    /// <summary>
    /// Stores information about a single <see cref="IInterceptionBehavior"/> to be used on an intercepted object and
    /// configures a container accordingly.
    /// </summary>
    /// <seealso cref="IInterceptionBehavior"/>
    public class InterceptionBehavior : InterceptionBehaviorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// <see cref="IInterceptionBehavior"/>.
        /// </summary>
        /// <param name="interceptionBehavior">The interception behavior to use.</param>
        public InterceptionBehavior(IInterceptionBehavior interceptionBehavior)
            : base(interceptionBehavior)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// given type/name pair.
        /// </summary>
        /// <param name="behaviorType">Type of behavior to </param>
        /// <param name="name"></param>
        public InterceptionBehavior(Type behaviorType, string name)
            : base(behaviorType, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// given behavior type.
        /// </summary>
        /// <param name="behaviorType">Type of behavior to </param>
        public InterceptionBehavior(Type behaviorType)
            : base(behaviorType)
        {
        }

        /// <summary>
        /// Get the list of behaviors for the current type so that it can be added to.
        /// </summary>
        /// <param name="policies">Policy list.</param>
        /// <param name="implementationType">Implementation type to set behaviors for.</param>
        /// <param name="name">Name type is registered under.</param>
        /// <returns>An instance of <see cref="InterceptionBehaviorsPolicy"/>.</returns>
        protected override InterceptionBehaviorsPolicy GetBehaviorsPolicy(IPolicyList policies, Type implementationType, string name)
        {
            return InterceptionBehaviorsPolicy.GetOrCreate(policies, implementationType, name);
        }
    }

    /// <summary>
    /// A generic version of <see cref="InterceptionBehavior"/> that lets you
    /// specify behavior types using generic syntax.
    /// </summary>
    /// <typeparam name="TBehavior">Type of behavior to register.</typeparam>
    public class InterceptionBehavior<TBehavior> : InterceptionBehavior
        where TBehavior : IInterceptionBehavior
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// given behavior type.
        /// </summary>
        public InterceptionBehavior() : base(typeof(TBehavior)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionBehavior"/> with a 
        /// given type/name pair.
        /// </summary>
        /// <param name="name">Name to use to resolve the behavior.</param>
        public InterceptionBehavior(string name) : base(typeof(TBehavior), name) { }
    }
}
