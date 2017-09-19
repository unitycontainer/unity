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

            if (context.Existing != null) return;

            IPolicyList containingPolicyList;
            ILifetimePolicy lifetimePolicy = GetLifetimePolicy(context, out containingPolicyList);

            if (lifetimePolicy is HierarchicalLifetimeManager && 
                !ReferenceEquals(containingPolicyList, context.PersistentPolicies))
            {
                var newLifetime = new HierarchicalLifetimeManager { InUse = true };
                context.PersistentPolicies.Set<ILifetimePolicy>(newLifetime, context.BuildKey);
                context.Lifetime.Add(newLifetime);
            }

            if (lifetimePolicy is PerResolveLifetimeManager &&
                ReferenceEquals(containingPolicyList, context.PersistentPolicies))
            {
                var newLifetime = new PerResolveLifetimeManager();
                context.Policies.Set<ILifetimePolicy>(newLifetime, context.BuildKey);
            }

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
            
            IPolicyList containingPolicyList;

            ILifetimePolicy lifetimePolicy = this.GetLifetimePolicy(context, out containingPolicyList);
            if (lifetimePolicy is ContainerControlledLifetimeManager)
            {
                lifetimePolicy.SetValue(context.Existing);
            }
        }

        private ILifetimePolicy GetLifetimePolicy(IBuilderContext context, out IPolicyList containingPolicyList)
        {
            var policy = context.Policies.Get<ILifetimePolicy>(context.BuildKey, out containingPolicyList);
            if (policy == null && context.BuildKey.Type.GetTypeInfo().IsGenericType)
            {
                policy = GetLifetimePolicyForGenericType(context, out containingPolicyList);
            }

            if (policy == null)
            {
                policy = new TransientLifetimeManager();
                context.PersistentPolicies.Set<ILifetimePolicy>(policy, context.BuildKey);
            }

            return policy;
        }

        private ILifetimePolicy GetLifetimePolicyForGenericType(IBuilderContext context, out IPolicyList containingPolicyList)
        {
            Type typeToBuild = context.BuildKey.Type;
            object openGenericBuildKey = new NamedTypeBuildKey(typeToBuild.GetGenericTypeDefinition(),
                                                               context.BuildKey.Name);

            ILifetimeFactoryPolicy factoryPolicy =
                context.Policies.Get<ILifetimeFactoryPolicy>(openGenericBuildKey, out containingPolicyList);

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
                    ILifetimePolicy lifetime = containingPolicyList.GetNoDefault<ILifetimePolicy>(context.BuildKey, false);
                    if (lifetime == null)
                    {
                        containingPolicyList.Set(newLifetime, context.BuildKey);
                        lifetime = newLifetime;
                    }

                    return lifetime;
                }
            }

            return null;
        }
    }
}
