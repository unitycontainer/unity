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
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// An <see cref="IBuilderStrategy"/> implementation that uses
    /// a <see cref="ILifetimePolicy"/> to figure out if an object
    /// has already been created and to update or remove that
    /// object from some backing store.
    /// </summary>
    public class LifetimeStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        // FxCop suppression: Validation is done by Guard class
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public override void PreBuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            if(context.Existing == null)
            {
                ILifetimePolicy lifetimePolicy = GetLifetimePolicy(context);
                IRequiresRecovery recovery = lifetimePolicy as IRequiresRecovery;
                if(recovery != null)
                {
                    context.RecoveryStack.Add(recovery);
                }

                object existing = lifetimePolicy.GetValue();
                if (existing != null)
                {
                    context.Existing = existing;
                    context.BuildComplete = true;
                }
            }
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        // FxCop suppression: Validation is done by Guard class
        public override void PostBuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            // If we got to this method, then we know the lifetime policy didn't
            // find the object. So we go ahead and store it.
            ILifetimePolicy lifetimePolicy = GetLifetimePolicy(context);
            lifetimePolicy.SetValue(context.Existing);
        }

        private static ILifetimePolicy GetLifetimePolicy(IBuilderContext context)
        {
            ILifetimePolicy policy = context.Policies.GetNoDefault<ILifetimePolicy>(context.BuildKey, false);
            if (policy == null && context.BuildKey.Type.GetTypeInfo().IsGenericType)
            {
                policy = GetLifetimePolicyForGenericType(context);
            }

            if(policy == null)
            {
                policy = new TransientLifetimeManager();
                context.PersistentPolicies.Set<ILifetimePolicy>(policy, context.BuildKey);
            }
            return policy;
        }

        private static ILifetimePolicy GetLifetimePolicyForGenericType(IBuilderContext context)
        {
            Type typeToBuild = context.BuildKey.Type;
            object openGenericBuildKey = new NamedTypeBuildKey(typeToBuild.GetGenericTypeDefinition(),
                                                               context.BuildKey.Name);

            IPolicyList factorySource;
            ILifetimeFactoryPolicy factoryPolicy =
                context.Policies.Get<ILifetimeFactoryPolicy>(openGenericBuildKey, out factorySource);

            if(factoryPolicy != null)
            {
                ILifetimePolicy lifetime = factoryPolicy.CreateLifetimePolicy();
                factorySource.Set<ILifetimePolicy>(lifetime, context.BuildKey);
                return lifetime;
            }

            return null;
        }
    }
}
