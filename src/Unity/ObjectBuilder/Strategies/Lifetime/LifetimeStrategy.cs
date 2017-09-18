// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Unity;
using Unity.Utility;

namespace ObjectBuilder2
{
    /// <summary>
    /// An <see cref="IBuilderStrategy"/> implementation that uses
    /// a <see cref="ILifetimePolicy"/> to figure out if an object
    /// has already been created and to update or remove that
    /// object from some backing store.
    /// </summary>
    public class LifetimeStrategy : BuilderStrategy
    {
        private readonly object genericLifetimeManagerLock = new object();

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        // FxCop suppression: Validation is done by Guard class
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation is done by Guard class.")]
        public override void PreBuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            if (context.Existing == null)
            {
                ILifetimePolicy lifetimePolicy = this.GetLifetimePolicy(context);
                IRequiresRecovery recovery = lifetimePolicy as IRequiresRecovery;
                if (recovery != null)
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
            ILifetimePolicy lifetimePolicy = this.GetLifetimePolicy(context);
            lifetimePolicy.SetValue(context.Existing);
        }

        private ILifetimePolicy GetLifetimePolicy(IBuilderContext context)
        {
            IPolicyList lifetimePolicySource;
            // TODO: Verify if call is optimal
            var policy = context.PersistentPolicies.Get<ILifetimePolicy>(context.BuildKey, out lifetimePolicySource);
            if (policy == null && context.BuildKey.Type.GetTypeInfo().IsGenericType)
            {
                policy = this.GetLifetimePolicyForGenericType(context);
            }

            if (policy == null)
            {
                policy = new TransientLifetimeManager();
                context.PersistentPolicies.Set<ILifetimePolicy>(policy, context.BuildKey);
            }

            return policy;
        }

        private ILifetimePolicy GetLifetimePolicyForGenericType(IBuilderContext context)
        {
            Type typeToBuild = context.BuildKey.Type;
            object openGenericBuildKey = new NamedTypeBuildKey(typeToBuild.GetGenericTypeDefinition(),
                                                               context.BuildKey.Name);

            IPolicyList factorySource;
            ILifetimeFactoryPolicy factoryPolicy =
                context.Policies.Get<ILifetimeFactoryPolicy>(openGenericBuildKey, out factorySource);

            if (factoryPolicy != null)
            {
                // creating the lifetime policy can result in arbitrary code execution
                // in particular it will likely result in a Resolve call, which could result in locking
                // to avoid deadlocks the new lifetime policy is created outside the lock
                // multiple instances might be created, but only one instance will be used
                ILifetimePolicy newLifetime = factoryPolicy.CreateLifetimePolicy();

                lock (this.genericLifetimeManagerLock)
                {
                    // check whether the policy for closed-generic has been added since first checked
                    ILifetimePolicy lifetime = factorySource.GetNoDefault<ILifetimePolicy>(context.BuildKey, false);
                    if (lifetime == null)
                    {
                        factorySource.Set<ILifetimePolicy>(newLifetime, context.BuildKey);
                        lifetime = newLifetime;
                    }

                    return lifetime;
                }
            }

            return null;
        }
    }
}
