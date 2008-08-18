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
using Microsoft.Practices.Unity.ObjectBuilder;

namespace Microsoft.Practices.Unity.Tests.TestDoubles
{
    /// <summary>
    /// A simple extension that puts the supplied strategy into the
    /// chain at the indicated stage.
    /// </summary>
    class SpyExtension : UnityContainerExtension
    {
        private IBuilderStrategy strategy;
        private UnityBuildStage stage;
        private IBuilderPolicy policy;
        private Type policyType;

        public SpyExtension(IBuilderStrategy strategy, UnityBuildStage stage)
        {
            this.strategy = strategy;
            this.stage = stage;
        }

        public SpyExtension(IBuilderStrategy strategy, UnityBuildStage stage, IBuilderPolicy policy, Type policyType)
        {
            this.strategy = strategy;
            this.stage = stage;
            this.policy = policy;
            this.policyType = policyType;
        }

        protected override void Initialize()
        {
            Context.Strategies.Add(strategy, stage);
            Context.Policies.SetDefault(policyType, policy);
        }
    }
}
