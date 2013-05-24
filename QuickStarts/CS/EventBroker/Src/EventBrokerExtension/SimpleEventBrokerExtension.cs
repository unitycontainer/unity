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

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using SimpleEventBroker;

namespace EventBrokerExtension
{
    public class SimpleEventBrokerExtension : UnityContainerExtension, ISimpleEventBrokerConfiguration
    {
        private readonly EventBroker broker = new EventBroker();

        protected override void Initialize()
        {
            Context.Container.RegisterInstance(broker, new ExternallyControlledLifetimeManager());

            Context.Strategies.AddNew<EventBrokerReflectionStrategy>(UnityBuildStage.PreCreation);
            Context.Strategies.AddNew<EventBrokerWireupStrategy>(UnityBuildStage.Initialization);
        }

        public EventBroker Broker
        {
            get { return broker; }
        }
    }
}
