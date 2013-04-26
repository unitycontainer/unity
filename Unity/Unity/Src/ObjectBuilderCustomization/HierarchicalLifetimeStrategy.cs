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

using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity
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
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(context, "context");

            IPolicyList lifetimePolicySource;

            var activeLifetime = context.PersistentPolicies.Get<ILifetimePolicy>(context.BuildKey, out lifetimePolicySource);
            if (activeLifetime is HierarchicalLifetimeManager && !ReferenceEquals(lifetimePolicySource, context.PersistentPolicies))         
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
