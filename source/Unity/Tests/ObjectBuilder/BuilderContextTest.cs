// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#else
using Xunit;
#endif

namespace ObjectBuilder2.Tests
{
     
    public class BuilderContextTest : IBuilderStrategy
    {
        private IBuilderContext parentContext, childContext, receivedContext;
        private bool throwOnBuildUp;
        
        public BuilderContextTest()
        {
            this.throwOnBuildUp = false;
        }

        [Fact]
        public void NewBuildSetsChildContextWhileBuilding()
        {
            this.parentContext = new BuilderContext(GetNonThrowingStrategyChain(), null, null, null, null, null);

            this.parentContext.NewBuildUp(null);

            Assert.Same(this.childContext, this.receivedContext);
        }

        [Fact]
        public void NewBuildClearsTheChildContextOnSuccess()
        {
            this.parentContext = new BuilderContext(GetNonThrowingStrategyChain(), null, null, null, null, null);

            this.parentContext.NewBuildUp(null);

            Assert.Null(this.parentContext.ChildContext);
        }

        [Fact]
        public void NewBuildDoesNotClearTheChildContextOnFailure()
        {
            this.parentContext = new BuilderContext(GetThrowingStrategyChain(), null, null, null, null, null);

            try
            {
                this.parentContext.NewBuildUp(null);
                Assert.True(false, string.Format("an exception should have been thrown here"));
            }
            catch (Exception)
            {
                Assert.NotNull(this.parentContext.ChildContext);
                Assert.Same(this.parentContext.ChildContext, this.receivedContext);
            }
        }

        private StrategyChain GetNonThrowingStrategyChain()
        {
            this.throwOnBuildUp = false;
            return new StrategyChain(new[] { this });
        }

        private StrategyChain GetThrowingStrategyChain()
        {
            this.throwOnBuildUp = true;
            return new StrategyChain(new[] { this });
        }

        public void PreBuildUp(IBuilderContext context)
        {
            this.childContext = this.parentContext.ChildContext;
            this.receivedContext = context;

            if (this.throwOnBuildUp)
            {
                throw new Exception();
            }
        }

        public void PostBuildUp(IBuilderContext context)
        {
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
