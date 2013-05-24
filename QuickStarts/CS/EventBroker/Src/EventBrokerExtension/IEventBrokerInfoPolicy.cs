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
