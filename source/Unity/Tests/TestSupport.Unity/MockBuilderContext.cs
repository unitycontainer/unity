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
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.TestSupport
{
    public class MockBuilderContext : IBuilderContext
    {
        ILifetimeContainer lifetime = new LifetimeContainer();
        NamedTypeBuildKey originalBuildKey = null;
        private IPolicyList persistentPolicies;
        IPolicyList policies;
        StrategyChain strategies = new StrategyChain();
        private CompositeResolverOverride resolverOverrides = new CompositeResolverOverride();

        private NamedTypeBuildKey buildKey = null;
        private object existing = null;
        private IRecoveryStack recoveryStack = new RecoveryStack();

        public MockBuilderContext()
        {
            this.persistentPolicies = new PolicyList();
            this.policies = new PolicyList(persistentPolicies);
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
            get { return persistentPolicies; }
        }

        public IPolicyList Policies
        {
            get { return policies; }
        }

        public IRecoveryStack RecoveryStack
        {
            get { return recoveryStack; }
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

        public bool BuildComplete { get; set; }

        public object CurrentOperation { get; set; }

        public IBuilderContext ChildContext { get; set; }

        public void AddResolverOverrides(IEnumerable<ResolverOverride> newOverrides)
        {
            resolverOverrides.AddRange(newOverrides);
        }

        public IDependencyResolverPolicy GetOverriddenResolver(Type dependencyType)
        {
            return resolverOverrides.GetResolver(this, dependencyType);
        }

        public IBuilderContext CloneForNewBuild(NamedTypeBuildKey newBuildKey, object newExistingObject)
        {
            var newContext = new MockBuilderContext
                                 {
                                     strategies = strategies,
                                     persistentPolicies = persistentPolicies,
                                     policies = policies,
                                     lifetime = lifetime,
                                     originalBuildKey = buildKey,
                                     buildKey = newBuildKey,
                                     existing = newExistingObject
                                 };
            newContext.resolverOverrides.Add(resolverOverrides);

            return newContext;
        }

        /// <summary>
        /// A convenience method to do a new buildup operation on an existing context.
        /// </summary>
        /// <param name="newBuildKey">Key to use to build up.</param>
        /// <returns>Created object.</returns>
        public object NewBuildUp(NamedTypeBuildKey newBuildKey)
        {
            var clone = CloneForNewBuild(newBuildKey, null);
            return clone.Strategies.ExecuteBuildUp(clone);
        }

        /// <summary>
        /// A convenience method to do a new buildup operation on an existing context. This
        /// overload allows you to specify extra policies which will be in effect for the duration
        /// of the build.
        /// </summary>
        /// <param name="newBuildKey">Key defining what to build up.</param>
        /// <param name="childCustomizationBlock">A delegate that takes a <see cref="IBuilderContext"/>. This
        /// is invoked with the new child context before the build up process starts. This gives callers
        /// the opportunity to customize the context for the build process.</param>
        /// <returns>Created object.</returns>
        public object NewBuildUp(NamedTypeBuildKey newBuildKey, Action<IBuilderContext> childCustomizationBlock)
        {
            var newContext = new MockBuilderContext
            {
                strategies = strategies,
                persistentPolicies = persistentPolicies,
                policies = new PolicyList(persistentPolicies),
                lifetime = lifetime,
                originalBuildKey = buildKey,
                buildKey = newBuildKey,
                existing = null
            };
            newContext.resolverOverrides.Add(resolverOverrides);

            childCustomizationBlock(newContext);
            
            return strategies.ExecuteBuildUp(newContext);
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
    }
}
