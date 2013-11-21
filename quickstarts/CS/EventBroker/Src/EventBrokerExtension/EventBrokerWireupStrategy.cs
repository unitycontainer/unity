// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.ObjectBuilder2;
using SimpleEventBroker;

namespace EventBrokerExtension
{
    public class EventBrokerWireupStrategy : BuilderStrategy
    {
        public override void PreBuildUp(IBuilderContext context)
        {
            if (context.Existing != null)
            {
                IEventBrokerInfoPolicy policy =
                    context.Policies.Get<IEventBrokerInfoPolicy>(context.BuildKey);
                if(policy != null)
                {
                    EventBroker broker = GetBroker(context);
                    foreach(PublicationInfo pub in policy.Publications)
                    {
                        broker.RegisterPublisher(pub.PublishedEventName, context.Existing, pub.EventName);
                    }
                    foreach(SubscriptionInfo sub in policy.Subscriptions)
                    {
                        broker.RegisterSubscriber(sub.PublishedEventName,
                            (EventHandler)Delegate.CreateDelegate(
                                typeof(EventHandler),
                                context.Existing,
                                sub.Subscriber));
                    }
                }
            }
        }

        private EventBroker GetBroker(IBuilderContext context)
        {
            var broker = context.NewBuildUp<EventBroker>();
            if(broker == null)
            {
                throw new InvalidOperationException("No event broker available");
            }
            return broker;
        }
    }
}
