// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// An <see cref="IInterceptionBehaviorsPolicy"/> that accumulates a sequence of 
    /// <see cref="IInterceptionBehavior"/> instances for an intercepted object.
    /// </summary>
    public class InterceptionBehaviorsPolicy : IInterceptionBehaviorsPolicy
    {
        private readonly List<NamedTypeBuildKey> behaviorKeys = new List<NamedTypeBuildKey>();

        /// <summary>
        /// Get the set of <see cref="NamedTypeBuildKey"/> that can be used to resolve the
        /// behaviors.
        /// </summary>
        public IEnumerable<NamedTypeBuildKey> BehaviorKeys
        {
            get { return behaviorKeys; }
        }

        /// <summary>
        /// Get the set of <see cref="IInterceptionBehavior"/> object to be used for the given type and
        /// interceptor.
        /// </summary>
        /// <remarks>
        /// This method will return a sequence of <see cref="IInterceptionBehavior"/>s. These behaviors will
        /// only be included if their <see cref="IInterceptionBehavior.WillExecute"/> properties are true.
        /// </remarks>
        /// <param name="context">Context for the current build operation.</param>
        /// <param name="interceptor">Interceptor that will be used to invoke the behavior.</param>
        /// <param name="typeToIntercept">Type that interception was requested on.</param>
        /// <param name="implementationType">Type that implements the interception.</param>
        /// <returns></returns>
        public IEnumerable<IInterceptionBehavior> GetEffectiveBehaviors(
            IBuilderContext context, IInterceptor interceptor, Type typeToIntercept, Type implementationType)
        {
            var interceptionRequest = new CurrentInterceptionRequest(interceptor, typeToIntercept, implementationType);

            foreach (var key in BehaviorKeys)
            {
                var behavior = (IInterceptionBehavior)context.NewBuildUp(key,
                    childContext => childContext.AddResolverOverrides(
                        new DependencyOverride<CurrentInterceptionRequest>(interceptionRequest)));
                yield return behavior;
            }
        }

        internal void AddBehaviorKey(NamedTypeBuildKey key)
        {
            behaviorKeys.Add(key);
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
