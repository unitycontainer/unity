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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public partial class BuildFailedExceptionTest
    {
        [TestMethod]
        public void ExceptionsThrownDuringBuildGetTranslated()
        {
            StrategyChain chain = new StrategyChain();
            chain.Add(new ThrowingStrategy("a"));

            PolicyList policies = new PolicyList();
            policies.Set<IShouldThrowPolicy>(new ShouldThrowPolicy(true, false, false, false), "a");

            Builder builder = new Builder();
            try
            {
                builder.BuildUp(null, null, policies, chain, typeof(object), null);
                Assert.Fail("Expected exception did not occur");
            }
            catch(BuildFailedException ex)
            {
                StringAssert.Contains(ex.BuildKey, typeof(object).Name);
                Assert.AreEqual(typeof(ThrowingStrategy).Name, ex.ExecutingStrategyTypeName);
                Assert.AreEqual(0, ex.ExecutingStrategyIndex);
                Assert.IsInstanceOfType(ex.InnerException, typeof(Exception));
            }
        }

        [TestMethod]
        public void ExceptionsThrownInPostBuildupGetTranslated()
        {
            StrategyChain chain = new StrategyChain();
            chain.Add(new ThrowingStrategy("a"));
            chain.Add(new ThrowingStrategy("b"));

            PolicyList policies = new PolicyList();
            policies.Set<IShouldThrowPolicy>(new ShouldThrowPolicy(false, false, false, false), "a");
            policies.Set<IShouldThrowPolicy>(new ShouldThrowPolicy(false, true, false, false), "b");

            Builder builder = new Builder();
            try
            {
                builder.BuildUp(null, null, policies, chain, typeof(object), null);
                Assert.Fail("Expected exception did not occur");
            }
            catch (BuildFailedException ex)
            {
                StringAssert.Contains(ex.BuildKey, typeof(object).Name);
                Assert.AreEqual(typeof(ThrowingStrategy).Name, ex.ExecutingStrategyTypeName);
                Assert.AreEqual(1, ex.ExecutingStrategyIndex);
                Assert.IsInstanceOfType(ex.InnerException, typeof(Exception));
            }
        }

        [TestMethod]
        public void ExceptionsThrownInPreTeardownGetTranslated()
        {
            StrategyChain chain = new StrategyChain();
            chain.Add(new ThrowingStrategy("a"));
            chain.Add(new ThrowingStrategy("b"));

            PolicyList policies = new PolicyList();
            policies.Set<IShouldThrowPolicy>(new ShouldThrowPolicy(false, false, true, false), "a");
            policies.Set<IShouldThrowPolicy>(new ShouldThrowPolicy(false, true, false, false), "b");

            Builder builder = new Builder();
            try
            {
                builder.TearDown(null, null, policies, chain, new object());
                Assert.Fail("Expected exception did not occur");
            }
            catch (BuildFailedException ex)
            {
                Assert.AreEqual(typeof(ThrowingStrategy).Name, ex.ExecutingStrategyTypeName);
                Assert.AreEqual(1, ex.ExecutingStrategyIndex);
                Assert.IsInstanceOfType(ex.InnerException, typeof(Exception));
            }            
        }

        [TestMethod]
        public void ExceptionsThrownInPostTeardownGetTranslated()
        {
            StrategyChain chain = new StrategyChain();
            chain.Add(new ThrowingStrategy("a"));
            chain.Add(new ThrowingStrategy("b"));

            PolicyList policies = new PolicyList();
            policies.Set<IShouldThrowPolicy>(new ShouldThrowPolicy(false, false, false, false), "a");
            policies.Set<IShouldThrowPolicy>(new ShouldThrowPolicy(false, false, false, true), "b");

            Builder builder = new Builder();
            try
            {
                builder.TearDown(null, null, policies, chain, new object());
                Assert.Fail("Expected exception did not occur");
            }
            catch (BuildFailedException ex)
            {
                Assert.AreEqual(typeof(ThrowingStrategy).Name, ex.ExecutingStrategyTypeName);
                Assert.AreEqual(0, ex.ExecutingStrategyIndex);
                Assert.IsInstanceOfType(ex.InnerException, typeof(Exception));
            }            
        }

        private interface IShouldThrowPolicy : IBuilderPolicy
        {
            bool PreBuildupShouldThrow { get; }
            bool PostBuildupShouldThrow { get; }
            bool PreTeardownShouldThrow { get; }
            bool PostTeardownShouldThrow { get; }
        }

        private class ShouldThrowPolicy : IShouldThrowPolicy
        {
            private readonly bool preBuildupShouldThrow;
            private readonly bool postBuildupShouldThrow;
            private readonly bool preTeardownShouldThrow;
            private readonly bool postTeardownShouldThrow;


            public ShouldThrowPolicy(bool preBuildupShouldThrow, 
                bool postBuildupShouldThrow, 
                bool preTeardownShouldThrow,
                bool postTeardownShouldThrow)
            {
                this.preBuildupShouldThrow = preBuildupShouldThrow;
                this.postBuildupShouldThrow = postBuildupShouldThrow;
                this.preTeardownShouldThrow = preTeardownShouldThrow;
                this.postTeardownShouldThrow = postTeardownShouldThrow;
            }

            public bool PreBuildupShouldThrow
            {
                get { return preBuildupShouldThrow; }
            }

            public bool PostBuildupShouldThrow
            {
                get { return postBuildupShouldThrow; }
            }

            public bool PreTeardownShouldThrow
            {
                get { return preTeardownShouldThrow; }
            }

            public bool PostTeardownShouldThrow
            {
                get { return postTeardownShouldThrow; }
            }
        }

        private class ThrowingStrategy : BuilderStrategy
        {
            private readonly string policyKey;


            public ThrowingStrategy(string policyKey)
            {
                this.policyKey = policyKey;
            }

            public override void PreBuildUp(IBuilderContext context)
            {
                if(GetPolicy(context).PreBuildupShouldThrow)
                {
                    throw new Exception("Pre build is throwing");
                }
            }

            public override void PostBuildUp(IBuilderContext context)
            {
                if(GetPolicy(context).PostBuildupShouldThrow)
                {
                    throw new Exception("Post build is throwing");
                }
            }

            public override void PreTearDown(IBuilderContext context)
            {
                if(GetPolicy(context).PreTeardownShouldThrow)
                {
                    throw new Exception("Pre teardown is throwing");
                }
            }

            public override void PostTearDown(IBuilderContext context)
            {
                if(GetPolicy(context).PostTeardownShouldThrow)
                {
                    throw new Exception("post teardown is throwing");
                }
            }

            private IShouldThrowPolicy GetPolicy(IBuilderContext context)
            {
                return context.Policies.Get<IShouldThrowPolicy>(policyKey);
            }
        }
    }
}
