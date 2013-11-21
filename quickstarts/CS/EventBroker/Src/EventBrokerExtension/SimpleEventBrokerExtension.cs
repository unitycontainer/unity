// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
