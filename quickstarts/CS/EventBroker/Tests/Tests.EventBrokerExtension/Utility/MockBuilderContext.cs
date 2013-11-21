// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Tests.EventBrokerExtension.Utility
{
    public class MockBuilderContext : IBuilderContext
    {
        private ILifetimeContainer lifetime = new LifetimeContainer();
        private NamedTypeBuildKey originalBuildKey = null;
        private IPolicyList policies = new PolicyList();
        private IPolicyList transientPolicies;
        private StrategyChain strategies = new StrategyChain();
        private RecoveryStack recoveryStack = new RecoveryStack();

        private NamedTypeBuildKey buildKey = null;
        private object existing = null;
        private bool buildComplete;

        public MockBuilderContext()
        {
            transientPolicies = new PolicyList(policies);
        }

        public ILifetimeContainer Lifetime
        {
            get { return lifetime; }
        }

        public NamedTypeBuildKey OriginalBuildKey
        {
            get { return originalBuildKey; }
        }


        public IPolicyList PersistentPolicies
        {
            get { return policies; }
        }

        public IPolicyList Policies
        {
            get { return transientPolicies; }
        }

        public StrategyChain Strategies
        {
            get { return strategies; }
        }

        IStrategyChain IBuilderContext.Strategies
        {
            get { return strategies; }
        }


        public NamedTypeBuildKey BuildKey
        {
            get { return buildKey; }
            set { buildKey = value; }
        }

        public object Existing
        {
            get { return existing; }
            set { existing = value; }
        }

        public bool BuildComplete
        {
            get { return buildComplete; }
            set { buildComplete = value; }
        }

        public IRecoveryStack RecoveryStack
        {
            get { return recoveryStack; }
        }

        /// <summary>
        /// Add a new set of resolver override objects to the current build operation.
        /// </summary>
        /// <param name="newOverrides"><see cref="T:Microsoft.Practices.Unity.ResolverOverride"/> objects to add.</param>
        public void AddResolverOverrides(IEnumerable<ResolverOverride> newOverrides)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a <see cref="T:Microsoft.Practices.ObjectBuilder2.IDependencyResolverPolicy"/> object for the given <paramref name="dependencyType"/>
        ///             or null if that dependency hasn't been overridden.
        /// </summary>
        /// <param name="dependencyType">Type of the dependency.</param>
        /// <returns>
        /// Resolver to use, or null if no override matches for the current operation.
        /// </returns>
        public IDependencyResolverPolicy GetOverriddenResolver(Type dependencyType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A convenience method to do a new buildup operation on an existing context.
        /// </summary>
        /// <param name="newBuildKey">Key to use to build up.</param>
        /// <returns>
        /// Created object.
        /// </returns>
        public object NewBuildUp(NamedTypeBuildKey newBuildKey)
        {
            MockBuilderContext newContext = new MockBuilderContext();
            newContext.strategies = strategies;
            newContext.policies = policies;
            newContext.transientPolicies = transientPolicies;
            newContext.lifetime = lifetime;
            newContext.originalBuildKey = newBuildKey;
            newContext.buildKey = newBuildKey;
            this.ChildContext = newContext;

            return newContext.Strategies.ExecuteBuildUp(newContext);
        }

        /// <summary>
        /// An object representing what is currently being done in the
        ///             build chain. Used to report back errors if there's a failure.
        /// </summary>
        public object CurrentOperation
        {
            get; set;
        }

        /// <summary>
        /// The build context used to resolve a dependency during the build operation represented by this context.
        /// </summary>
        public IBuilderContext ChildContext
        {
            get; private set;
        }
        public object ExecuteBuildUp(NamedTypeBuildKey buildKey, object existing)
        {
            this.BuildKey = buildKey;
            this.Existing = existing;

            return Strategies.ExecuteBuildUp(this);
        }

        public object ExecuteTearDown(object existing)
        {
            this.BuildKey = null;
            this.Existing = existing;

            Strategies.Reverse().ExecuteTearDown(this);
            return existing;
        }

        /// <summary>
        /// A convenience method to do a new buildup operation on an existing context. This
        ///             overload allows you to specify extra policies which will be in effect for the duration
        ///             of the build.
        /// </summary>
        /// <param name="newBuildKey">Key defining what to build up.</param><param name="childCustomizationBlock">A delegate that takes a <see cref="T:Microsoft.Practices.ObjectBuilder2.IBuilderContext"/>. This
        ///             is invoked with the new child context before the build up process starts. This gives callers
        ///             the opportunity to customize the context for the build process.</param>
        /// <returns>
        /// Created object.
        /// </returns>
        public object NewBuildUp(NamedTypeBuildKey newBuildKey, Action<IBuilderContext> childCustomizationBlock)
        {
            var newContext = new MockBuilderContext
            {
                strategies = strategies,
                policies = policies,
                transientPolicies = transientPolicies,
                lifetime = lifetime,
                originalBuildKey = newBuildKey,
                buildKey = newBuildKey
            };

            childCustomizationBlock(newContext);

            this.ChildContext = newContext;

            return newContext.Strategies.ExecuteBuildUp(newContext);
        }
    }
}
