// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using ObjectBuilder2;

namespace Unity.Tests.TestDoubles
{
    /// <summary>
    /// A small noop strategy that lets us check afterwards to
    /// see if it ran in the strategy chain.
    /// </summary>
    internal class SpyStrategy : BuilderStrategy
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

            this.UpdateSpyPolicy(context);
        }

        public IBuilderContext Context
        {
            get { return this.context; }
        }

        public object BuildKey
        {
            get { return this.buildKey; }
        }

        public object Existing
        {
            get { return this.existing; }
        }

        public bool BuildUpWasCalled
        {
            get { return this.buildUpWasCalled; }
        }

        private void UpdateSpyPolicy(IBuilderContext context)
        {
            SpyPolicy policy = context.Policies.Get<SpyPolicy>(context.BuildKey);
            if (policy != null)
            {
                policy.WasSpiedOn = true;
            }
        }
    }
}
