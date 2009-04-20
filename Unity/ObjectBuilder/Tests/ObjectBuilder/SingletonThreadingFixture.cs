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

using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public class SingletonThreadingFixture
    {
        [TestMethod]
        public void SingletonReturnsSameValueWhenCalledInMultipleThreads()
        {
            StrategyChain strategies = GetStrategies();
            IPolicyList policies = GetPolicies();

            BuilderOnThread threadResults1 = new BuilderOnThread(strategies, policies);
            BuilderOnThread threadResults2 = new BuilderOnThread(strategies, policies);

            Thread thread1 = new Thread(new ThreadStart(threadResults1.Build));
            Thread thread2 = new Thread(new ThreadStart(threadResults2.Build));

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Assert.AreSame(threadResults1.Result, threadResults2.Result);
        }

        private StrategyChain GetStrategies()
        {
            StrategyChain chain = new StrategyChain();
            chain.Add(new LifetimeStrategy());
            chain.Add(new SleepingStrategy());
            chain.Add(new NewObjectStrategy());
            return chain;
        }

        private IPolicyList GetPolicies()
        {
            PolicyList policies = new PolicyList();
            policies.Set<ILifetimePolicy>(new SingletonLifetimePolicy(), typeof (object));
            return policies;
        }
    }

    // A test class that runs a buildup - designed to be used in a thread
    class BuilderOnThread
    {
        public object Result;
        private StrategyChain strategies;
        private IPolicyList policies;

        public BuilderOnThread(StrategyChain strategies, IPolicyList policies)
        {
            this.strategies = strategies;
            this.policies = policies;
        }

        public void Build()
        {
            Result = new Builder().BuildUp(
                null, null, policies, strategies, typeof (object), null);
        }
    }

    // A test strategy that puts a variable sleep time into
    // the strategy chain
    class SleepingStrategy : BuilderStrategy
    {
        private int sleepTimeMS = 500;

        public override void PreBuildUp(IBuilderContext context)
        {
            Thread.Sleep(sleepTimeMS);
            sleepTimeMS = sleepTimeMS == 0 ? 500 : 0;
        }
    }

    // A test strategy that just creates an object.
    class NewObjectStrategy : BuilderStrategy
    {
        public override void PreBuildUp(IBuilderContext context)
        {
            context.Existing = new object();
        }
    }
}
