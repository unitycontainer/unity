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

namespace Microsoft.Practices.Unity.Tests.TestDoubles
{
    class TestingBuilderContext : IBuilderContext
    {
		ILifetimeContainer lifetime = new LifetimeContainer();
		IReadWriteLocator locator;
		object originalBuildKey = null;
        private IPolicyList persistentPolicies;
		IPolicyList policies;
		StrategyChain strategies = new StrategyChain();

	    private object buildKey = null;
	    private object existing = null;
	    private bool buildComplete;
        private IRecoveryStack recoveryStack = new RecoveryStack();

        public TestingBuilderContext()
			: this(new Locator()) {}

		public TestingBuilderContext(IReadWriteLocator locator)
		{
			this.locator = locator;
		    this.persistentPolicies = new PolicyList();
		    this.policies = new PolicyList(persistentPolicies);
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

	    public IBuilderContext CloneForNewBuild(object newBuildKey, object newExistingObject)
	    {
	        TestingBuilderContext newContext = new TestingBuilderContext(this.Locator);
	        newContext.strategies = strategies;
	        newContext.persistentPolicies = persistentPolicies;
	        newContext.policies = policies;
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
