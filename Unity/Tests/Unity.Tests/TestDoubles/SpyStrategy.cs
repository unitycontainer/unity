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
    /// <summary>
    /// A small noop strategy that lets us check afterwards to
    /// see if it ran in the strategy chain.
    /// </summary>
    class SpyStrategy : BuilderStrategy
    {
        private IBuilderContext context = null;
        private object buildKey = null;
        private object existing = null;
        private bool buildUpWasCalled = false;

        public override void PreBuildUp(IBuilderContext context)
        {
            this.buildUpWasCalled = true;
            this.context = context;
            this.buildKey = context.BuildKey;
            this.existing = context.Existing;

            UpdateSpyPolicy(context);
        }

        public IBuilderContext Context
        {
            get { return context; }
        }

        public object BuildKey
        {
            get { return buildKey; }
        }

        public object Existing
        {
            get { return existing; }
        }

        public bool BuildUpWasCalled
        {
            get { return buildUpWasCalled; }
        }

        private void UpdateSpyPolicy(IBuilderContext context)
        {
            SpyPolicy policy = context.Policies.Get<SpyPolicy>(context.BuildKey);
            if(policy != null)
            {
                policy.WasSpiedOn = true;
            }
        }
    }
}
