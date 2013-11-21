// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;

namespace EventBrokerExtension
{
    /// <summary>
    /// This policy interface allows access to saved publication and
    /// subscription information.
    /// </summary>
    public interface IEventBrokerInfoPolicy : IBuilderPolicy
    {
        IEnumerable<PublicationInfo> Publications { get;}
        IEnumerable<SubscriptionInfo> Subscriptions { get; }
    }

    public struct PublicationInfo
    {
        public string PublishedEventName;
        public string EventName;

        public PublicationInfo(string publishedEventName, string eventName)
        {
            PublishedEventName = publishedEventName;
            EventName = eventName;
        }
    }

    public struct SubscriptionInfo
    {
        public string PublishedEventName;
        public MethodInfo Subscriber;


        public SubscriptionInfo(string publishedEventName, MethodInfo subscriber)
        {
            PublishedEventName = publishedEventName;
            Subscriber = subscriber;
        }
    }
}
