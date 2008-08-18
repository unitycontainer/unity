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
using Microsoft.Practices.ObjectBuilder2;

namespace Tests.EventBrokerExtension.Utility
{
    public class MockBuilderContext : IBuilderContext
    {
        private ILifetimeContainer lifetime = new LifetimeContainer();
        private IReadWriteLocator locator;
        private object originalBuildKey = null;
        private IPolicyList policies = new PolicyList();
        private IPolicyList transientPolicies;
        private StrategyChain strategies = new StrategyChain();
        private RecoveryStack recoveryStack = new RecoveryStack();

        private object buildKey = null;
        private object existing = null;
        private bool buildComplete;

        public MockBuilderContext()
            : this(new Locator()) { }

        public MockBuilderContext(IReadWriteLocator locator)
        {
            transientPolicies = new PolicyList(policies);
            this.locator = locator;
        }

        public ILifetimeContainer Lifetime
        {
            get { return lifetime; }
        }

        public IReadWriteLocator Locator
        {
            get { return locator; }
        }

        public object OriginalBuildKey
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


        public object BuildKey
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

        public IBuilderContext CloneForNewBuild(object newBuildKey, object newExistingObject)
        {
            MockBuilderContext newContext = new MockBuilderContext(this.Locator);
            newContext.strategies = strategies;
            newContext.policies = policies;
            newContext.transientPolicies = transientPolicies;
            newContext.lifetime = lifetime;
            newContext.originalBuildKey = newBuildKey;
            newContext.buildKey = newBuildKey;
            newContext.existing = newExistingObject;
            return newContext;
        }

        public object ExecuteBuildUp(object buildKey, object existing)
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
