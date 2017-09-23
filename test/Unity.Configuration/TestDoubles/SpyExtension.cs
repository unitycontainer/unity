// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using ObjectBuilder2;
using Unity.ObjectBuilder;

namespace Unity.Tests
{
    internal class SpyExtension : UnityContainerExtension
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
            Context.Strategies.Add(this.strategy, this.stage);
            Context.Policies.SetDefault(this.policyType, this.policy);
        }
    }
}
