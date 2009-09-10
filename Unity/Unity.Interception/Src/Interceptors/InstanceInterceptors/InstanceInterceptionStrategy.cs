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
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A <see cref="IBuilderStrategy"/> that intercepts objects
    /// in the build chain by creating a proxy object.
    /// </summary>
    public class InstanceInterceptionStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PostBuildUp(IBuilderContext context)
        {
            // If it's already been intercepted, don't do it again.
            if (context.Existing is IInterceptingProxy) return;

            IInstanceInterceptionPolicy interceptionPolicy =
                FindInterceptionPolicy<IInstanceInterceptionPolicy>(context, true);
            if (interceptionPolicy == null) return;

            IInterceptionBehaviorsPolicy interceptionBehaviorsPolicy =
                FindInterceptionPolicy<IInterceptionBehaviorsPolicy>(context, true);
            if (interceptionBehaviorsPolicy == null) return;

            IAdditionalInterfacesPolicy additionalInterfacesPolicy =
                FindInterceptionPolicy<IAdditionalInterfacesPolicy>(context, false);
            IEnumerable<Type> additionalInterfaces =
                additionalInterfacesPolicy != null ? additionalInterfacesPolicy.AdditionalInterfaces : Type.EmptyTypes;

            Type typeToIntercept = BuildKey.GetType(context.OriginalBuildKey);
            Type implementationType = context.Existing.GetType();

            IUnityContainer container = context.NewBuildUp<IUnityContainer>();
            IInterceptionBehavior[] interceptionBehaviors =
                interceptionBehaviorsPolicy.InterceptionBehaviorDescriptors
                    .Select(pid =>
                        pid.GetInterceptionBehavior(
                            interceptionPolicy.Interceptor,
                            typeToIntercept,
                            implementationType,
                            container))
                    .Where(pi => pi != null)
                    .ToArray();

            if (interceptionBehaviors.Length > 0)
            {
                context.Existing =
                    Intercept.ThroughProxy(
                        typeToIntercept,
                        context.Existing,
                        interceptionPolicy.Interceptor,
                        interceptionBehaviors,
                        additionalInterfaces);
            }
        }

        private static T FindInterceptionPolicy<T>(IBuilderContext context, bool probeOriginalKey)
            where T : class, IBuilderPolicy
        {
            T policy;

            // First, try for a match against the current build key
            Type currentType = BuildKey.GetType(context.BuildKey);
            policy = context.Policies.Get<T>(context.BuildKey, false) ??
                context.Policies.Get<T>(currentType, false);
            if (policy != null)
            {
                return policy;
            }

            if (!probeOriginalKey)
                return null;

            // Next, try the original build key
            Type originalType = BuildKey.GetType(context.OriginalBuildKey);
            policy = context.Policies.Get<T>(context.OriginalBuildKey, false) ??
                context.Policies.Get<T>(originalType, false);

            return policy;
        }
    }
}
