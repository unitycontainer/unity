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
            EventBroker broker = context.Locator.Get<EventBroker>(typeof(EventBroker));
            if(broker == null)
            {
                throw new InvalidOperationException("No event broker available");
            }
            return broker;
        }
    }
}
