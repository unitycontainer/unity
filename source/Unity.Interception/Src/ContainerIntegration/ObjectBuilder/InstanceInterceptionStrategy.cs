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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public override void PostBuildUp(IBuilderContext context)
        {
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(context, "context");

            // If it's already been intercepted, don't do it again.
            if (context.Existing is IInterceptingProxy) return;

            IInstanceInterceptionPolicy interceptionPolicy =
                FindInterceptionPolicy<IInstanceInterceptionPolicy>(context, true);
            if (interceptionPolicy == null) return;
            var interceptor = interceptionPolicy.GetInterceptor(context);


            IInterceptionBehaviorsPolicy interceptionBehaviorsPolicy =
                FindInterceptionPolicy<IInterceptionBehaviorsPolicy>(context, true);
            if (interceptionBehaviorsPolicy == null) return;

            IAdditionalInterfacesPolicy additionalInterfacesPolicy =
                FindInterceptionPolicy<IAdditionalInterfacesPolicy>(context, false);
            IEnumerable<Type> additionalInterfaces =
                additionalInterfacesPolicy != null ? additionalInterfacesPolicy.AdditionalInterfaces : Type.EmptyTypes;

            Type typeToIntercept = context.OriginalBuildKey.Type;
            Type implementationType = context.Existing.GetType();

            IInterceptionBehavior[] interceptionBehaviors =
                interceptionBehaviorsPolicy.GetEffectiveBehaviors(
                    context, interceptor, typeToIntercept, implementationType)
                .ToArray();

            if (interceptionBehaviors.Length > 0)
            {
                context.Existing =
                    Intercept.ThroughProxyWithAdditionalInterfaces(
                        typeToIntercept,
                        context.Existing,
                        interceptor,
                        interceptionBehaviors,
                        additionalInterfaces);
            }
        }

        private static T FindInterceptionPolicy<T>(IBuilderContext context, bool probeOriginalKey)
            where T : class, IBuilderPolicy
        {
            T policy;

            // First, try for a match against the current build key
            Type currentType = context.BuildKey.Type;
            policy = context.Policies.Get<T>(context.BuildKey, false) ??
                context.Policies.Get<T>(currentType, false);
            if (policy != null)
            {
                return policy;
            }

            if (!probeOriginalKey)
                return null;

            // Next, try the original build key
            Type originalType = context.OriginalBuildKey.Type;
            policy = context.Policies.Get<T>(context.OriginalBuildKey, false) ??
                context.Policies.Get<T>(originalType, false);

            return policy;
        }
    }
}
