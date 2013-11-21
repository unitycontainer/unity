// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
