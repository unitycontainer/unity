// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using ObjectBuilder2;

namespace Unity
{
    /// <summary>
    /// A strategy that handles Hierarchical lifetimes across a set of parent/child
    /// containers.
    /// </summary>
    public class HierarchicalLifetimeStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope",
            Justification = "Lifetime manager aligns with lifetime of container and is disposed when container is disposed.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public override void PreBuildUp(IBuilderContext context)
        {
            Utility.Guard.ArgumentNotNull(context, "context");

            IPolicyList lifetimePolicySource;

            var activeLifetime = context.PersistentPolicies.Get<ILifetimePolicy>(context.BuildKey, out lifetimePolicySource);
            if (activeLifetime is HierarchicalLifetimeManager && !object.ReferenceEquals(lifetimePolicySource, context.PersistentPolicies))
            {
                // came from parent, add a new Hierarchical lifetime manager locally   
                var newLifetime = new HierarchicalLifetimeManager { InUse = true };
                context.PersistentPolicies.Set<ILifetimePolicy>(newLifetime, context.BuildKey);
                // Add to the lifetime container - we know this one is disposable
                context.Lifetime.Add(newLifetime);
            }
        }
    }
}
