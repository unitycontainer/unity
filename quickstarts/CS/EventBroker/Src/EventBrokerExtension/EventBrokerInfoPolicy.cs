// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace EventBrokerExtension
{
    public class EventBrokerInfoPolicy : IEventBrokerInfoPolicy
    {
        private List<PublicationInfo> publications = new List<PublicationInfo>();
        private List<SubscriptionInfo> subscriptions = new List<SubscriptionInfo>();

        public void AddPublication(string publishedEventName, string eventName)
        {
            publications.Add(new PublicationInfo(publishedEventName, eventName));
        }

        public void AddSubscription(string publishedEventName, MethodInfo subscriber)
        {
            subscriptions.Add(new SubscriptionInfo(publishedEventName, subscriber));
        }

        public IEnumerable<PublicationInfo> Publications
        {
            get { return publications; }
        }

        public IEnumerable<SubscriptionInfo> Subscriptions
        {
            get { return subscriptions; }
        }
    }
}
