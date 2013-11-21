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
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public class StagedStrategyChainTest
    {
        static void AssertOrder(IStrategyChain chain,
                                params FakeStrategy[] strategies)
        {
            List<IBuilderStrategy> strategiesInChain = new List<IBuilderStrategy>(chain);
            CollectionAssert.AreEqual(strategies, strategiesInChain);
        }

        [TestMethod]
        public void InnerStrategiesComeBeforeOuterStrategiesInStrategyChain()
        {
            StagedStrategyChain<FakeStage> innerChain = new StagedStrategyChain<FakeStage>();
            StagedStrategyChain<FakeStage> outerChain = new StagedStrategyChain<FakeStage>(innerChain);
            FakeStrategy innerStrategy = new FakeStrategy();
            FakeStrategy outerStrategy = new FakeStrategy();
            innerChain.Add(innerStrategy, FakeStage.Stage1);
            outerChain.Add(outerStrategy, FakeStage.Stage1);

            IStrategyChain chain = outerChain.MakeStrategyChain();

            AssertOrder(chain, innerStrategy, outerStrategy);
        }

        [TestMethod]
        public void OrderingAcrossStagesForStrategyChain()
        {
            StagedStrategyChain<FakeStage> innerChain = new StagedStrategyChain<FakeStage>();
            StagedStrategyChain<FakeStage> outerChain = new StagedStrategyChain<FakeStage>(innerChain);
            FakeStrategy innerStage1 = new FakeStrategy();
            FakeStrategy innerStage2 = new FakeStrategy();
            FakeStrategy outerStage1 = new FakeStrategy();
            FakeStrategy outerStage2 = new FakeStrategy();
            innerChain.Add(innerStage1, FakeStage.Stage1);
            innerChain.Add(innerStage2, FakeStage.Stage2);
            outerChain.Add(outerStage1, FakeStage.Stage1);
            outerChain.Add(outerStage2, FakeStage.Stage2);

            IStrategyChain chain = outerChain.MakeStrategyChain();

            AssertOrder(chain, innerStage1, outerStage1, innerStage2, outerStage2);
        }

        [TestMethod]
        public void MultipleChildContainers()
        {
            StagedStrategyChain<FakeStage> innerChain = new StagedStrategyChain<FakeStage>();
            StagedStrategyChain<FakeStage> outerChain = new StagedStrategyChain<FakeStage>(innerChain);
            StagedStrategyChain<FakeStage> superChain = new StagedStrategyChain<FakeStage>(outerChain);

            FakeStrategy innerStrategy = new FakeStrategy();
            FakeStrategy outerStrategy = new FakeStrategy();
            FakeStrategy superStrategy = new FakeStrategy();
            innerChain.Add(innerStrategy, FakeStage.Stage1);
            outerChain.Add(outerStrategy, FakeStage.Stage1);
            superChain.Add(superStrategy, FakeStage.Stage1);

            IStrategyChain chain = superChain.MakeStrategyChain();

            AssertOrder(chain, innerStrategy, outerStrategy, superStrategy);
        }

        enum FakeStage
        {
            Stage1,
            Stage2,
        }

        class FakeStrategy : IBuilderStrategy
        {
            public void PreBuildUp(IBuilderContext context)
            {
                throw new NotImplementedException();
            }

            public void PostBuildUp(IBuilderContext context)
            {
                throw new NotImplementedException();
            }

            public void PreTearDown(IBuilderContext context)
            {
                throw new NotImplementedException();
            }

            public void PostTearDown(IBuilderContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
