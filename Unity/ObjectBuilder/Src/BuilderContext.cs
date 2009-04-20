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

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    public class BuilderContext : IBuilderContext
    {
        readonly IStrategyChain chain;
        readonly ILifetimeContainer lifetime;
        readonly IReadWriteLocator locator;
        private readonly IRecoveryStack recoveryStack = new RecoveryStack();
        readonly object originalBuildKey;
        private readonly IPolicyList persistentPolicies;
        readonly IPolicyList policies;
        private object buildKey;
        private object existing;
        private bool buildComplete;

        /// <summary>
        /// Initialize a new instance of the <see cref="BuilderContext"/> class.
        /// </summary>
        protected BuilderContext() { }

        /// <summary>
        /// Initialize a new instance of the <see cref="BuilderContext"/> class with a <see cref="IStrategyChain"/>, 
        /// <see cref="IReadWriteLocator"/>, <see cref="ILifetimeContainer"/>, <see cref="IPolicyList"/> and the 
        /// build key used to start this build operation. 
        /// </summary>
        /// <param name="chain">The <see cref="IStrategyChain"/> to use for this context.</param>
        /// <param name="locator">The <see cref="IReadWriteLocator"/> to use for this context.</param>
        /// <param name="lifetime">The <see cref="ILifetimeContainer"/> to use for this context.</param>
        /// <param name="policies">The <see cref="IPolicyList"/> to use for this context.</param>
        /// <param name="originalBuildKey">Build key to start building.</param>
        /// <param name="existing">The existing object to build up.</param>
        public BuilderContext(IStrategyChain chain,
            IReadWriteLocator locator,
            ILifetimeContainer lifetime,
            IPolicyList policies,
            object originalBuildKey,
            object existing)
        {
            this.chain = chain;
            this.locator = locator;
            this.lifetime = lifetime;
            this.originalBuildKey = originalBuildKey;
            this.buildKey = originalBuildKey;
            this.persistentPolicies = policies;
            this.policies = new PolicyList(persistentPolicies);
            this.existing = existing;
        }

        /// <summary>
        /// Create a new <see cref="BuilderContext"/> using the explicitly provided
        /// values.
        /// </summary>
        /// <param name="chain">The <see cref="IStrategyChain"/> to use for this context.</param>
        /// <param name="locator">The <see cref="IReadWriteLocator"/> to use for this context.</param>
        /// <param name="lifetime">The <see cref="ILifetimeContainer"/> to use for this context.</param>
        /// <param name="persistentPolicies">The set of persistent policies to use for this context.</param>
        /// <param name="transientPolicies">The set of transient policies to use for this context. It is
        /// the caller's responsibility to ensure that the transient and persistent policies are properly
        /// combined.</param>
        /// <param name="buildKey">Build key for this context.</param>
        /// <param name="existing">Existing object to build up.</param>
        public BuilderContext(IStrategyChain chain, IReadWriteLocator locator, ILifetimeContainer lifetime, IPolicyList persistentPolicies, IPolicyList transientPolicies, object buildKey, object existing)
        {
            this.chain = chain;
            this.lifetime = lifetime;
            this.locator = locator;
            this.persistentPolicies = persistentPolicies;
            this.policies = transientPolicies;
            this.originalBuildKey = buildKey;
            this.buildKey = buildKey;
            this.existing = existing;
        }

        /// <summary>
        /// Gets the head of the strategy chain.
        /// </summary>
        /// <returns>
        /// The strategy that's first in the chain; returns null if there are no
        /// strategies in the chain.
        /// </returns>
        public IStrategyChain Strategies
        {
            get { return chain; }
        }

        /// <summary>
        /// Get the current build key for the current build operation.
        /// </summary>
        public object BuildKey
        {
            get { return buildKey; }
            set { buildKey = value; }
        }

        /// <summary>
        /// The current object being built up or torn down.
        /// </summary>
        /// <value>
        /// The current object being manipulated by the build operation. May
        /// be null if the object hasn't been created yet.</value>
        public object Existing
        {
            get { return existing; }
            set { existing = value; }
        }

        /// <summary>
        /// Gets the <see cref="ILifetimeContainer"/> associated with the build.
        /// </summary>
        /// <value>
        /// The <see cref="ILifetimeContainer"/> associated with the build.
        /// </value>
        public ILifetimeContainer Lifetime
        {
            get { return lifetime; }
        }

        /// <summary>
        /// Gets the locator available to the strategies.
        /// </summary>
        /// <value>
        /// The locator available to the strategies.
        /// </value>
        public IReadWriteLocator Locator
        {
            get { return locator; }
        }

        /// <summary>
        /// Gets the original build key for the build operation.
        /// </summary>
        /// <value>
        /// The original build key for the build operation.
        /// </value>
        public object OriginalBuildKey
        {
            get { return originalBuildKey; }
        }

        /// <summary>
        /// The set of policies that were passed into this context.
        /// </summary>
        /// <remarks>This returns the policies passed into the context.
        /// Policies added here will remain after buildup completes.</remarks>
        /// <value>The persistent policies for the current context.</value>
        public IPolicyList PersistentPolicies
        {
            get { return persistentPolicies; }
        }

        /// <summary>
        /// Gets the policies for the current context. 
        /// </summary>
        /// <remarks>
        /// Any modifications will be transient (meaning, they will be forgotten when 
        /// the outer BuildUp for this context is finished executing).
        /// </remarks>
        /// <value>
        /// The policies for the current context.
        /// </value>
        public IPolicyList Policies
        {
            get { return policies; }
        }


        /// <summary>
        /// Gets the collection of <see cref="IRequiresRecovery"/> objects
        /// that need to execute in event of an exception.
        /// </summary>
        public IRecoveryStack RecoveryStack
        {
            get { return recoveryStack; }
        }

        /// <summary>
        /// Flag indicating if the build operation should continue.
        /// </summary>
        /// <value>true means that building should not call any more
        /// strategies, false means continue to the next strategy.</value>
        public bool BuildComplete
        {
            get { return buildComplete; }
            set { buildComplete = value; }
        }

        /// <summary>
        /// Create a new IBuilderContext which has the same strategies, locator, policies, and lifetime
        /// but a new build key and existing object. Used to execute recursive calls when
        /// building up dependencies.
        /// </summary>
        /// <param name="newBuildKey">Build key for new buildup.</param>
        /// <param name="newExistingObject">New exsting object for buildup.</param>
        /// <returns>The new context.</returns>
        public IBuilderContext CloneForNewBuild(object newBuildKey, object newExistingObject)
        {
            return new BuilderContext(chain, locator, lifetime, persistentPolicies, policies, newBuildKey, newExistingObject);
        }

        /// <summary>
        /// A convenience method to do a new buildup operation on an existing context.
        /// </summary>
        /// <remarks>This helper is specific to NamedTypeBuildKey.</remarks>
        /// <typeparam name="T">Type to return from the buildup.</typeparam>
        /// <param name="context">Existing context.</param>
        /// <returns>The built up object.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BuildUp",
            Justification = "Kept for backward compatibility with ObjectBuilder")]
        public static T NewBuildUp<T>(IBuilderContext context)
        {
            IBuilderContext clone = context.CloneForNewBuild(NamedTypeBuildKey.Make<T>(), null);
            return (T) (clone.Strategies.ExecuteBuildUp(clone));
        }
    }
}
