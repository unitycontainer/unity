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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleEventBroker;

namespace EventBrokerTests
{
    /// <summary>
    /// Summary description for EventBrokerFixture
    /// </summary>
    [TestClass]
    public class EventBrokerFixture
    {
        [TestMethod]
        public void ShouldRegisterOnePublisher()
        {
            EventBroker broker = new EventBroker();
            EventSource1 publisher = new EventSource1();

            broker.RegisterPublisher("PublishedEvent1", publisher, "Event1");

            List<string> published = new List<string>(broker.RegisteredEvents);
            List<object> publishers = new List<object>(broker.GetPublishersFor("PublishedEvent1"));

            Assert.IsTrue(published.Contains("PublishedEvent1"));
            Assert.AreEqual(1, publishers.Count);
            Assert.AreSame(publisher, publishers[0]);
        }

        [TestMethod]
        public void ShouldRegisterOneSubscriber()
        {
            EventBroker broker = new EventBroker();

            EventHandler subscriber = delegate { };

            broker.RegisterSubscriber("SubscribedEvent1", subscriber);

            List<string> published = new List<string>(broker.RegisteredEvents);
            List<object> publishers = new List<object>(broker.GetPublishersFor("SubscribedEvent1"));
            List<EventHandler> subscribers = new List<EventHandler>(broker.GetSubscribersFor("SubscribedEvent1"));

            Assert.AreEqual(1, published.Count);
            Assert.AreEqual("SubscribedEvent1", published[0]);
            Assert.AreEqual(0, publishers.Count);
            Assert.AreEqual(1, subscribers.Count);
            Assert.AreSame(subscriber, subscribers[0]);
        }

        [TestMethod]
        public void ShouldRegisterOnePublisherAndOneSubscriber()
        {
            EventBroker broker = new EventBroker();
            EventSource1 publisher = new EventSource1();
            string publishedEventName = "MyEvent";
            EventHandler subscriber = delegate { };

            broker.RegisterPublisher(publishedEventName, publisher, "Event1");
            broker.RegisterSubscriber(publishedEventName, subscriber);
            
            List<string> published = new List<string>(broker.RegisteredEvents);
            List<object> publishers = new List<object>(broker.GetPublishersFor(publishedEventName));
            List<EventHandler> subscribers = new List<EventHandler>(broker.GetSubscribersFor(publishedEventName));

            Assert.AreEqual(1, published.Count);
            Assert.AreEqual(publishedEventName, published[0]);

            Assert.AreEqual(1, publishers.Count);
            Assert.AreSame(publisher, publishers[0]);

            Assert.AreEqual(1, subscribers.Count);
            Assert.AreSame(subscriber, subscribers[0]);
        }

        [TestMethod]
        public void ShouldCallSubscriberWhenPublisherFiresEvent()
        {
            EventBroker broker = new EventBroker();
            EventSource1 publisher = new EventSource1();
            string publishedEventName = "MyEvent";
            bool subscriberFired = false;
            EventHandler subscriber = delegate { subscriberFired = true;  };

            broker.RegisterPublisher(publishedEventName, publisher, "Event1");
            broker.RegisterSubscriber(publishedEventName, subscriber);

            publisher.FireEvent1();

            Assert.IsTrue(subscriberFired);
        }

        [TestMethod]
        public void ShouldRemovePublisherFromListOnUnregistration()
        {
            EventBroker broker = new EventBroker();
            EventSource1 publisher = new EventSource1();
            string publishedEventName = "MyEvent";
            broker.RegisterPublisher(publishedEventName, publisher, "Event1");

            broker.UnregisterPublisher(publishedEventName, publisher, "Event1");

            Assert.AreEqual(0, new List<object>(broker.GetPublishersFor(publishedEventName)).Count);
        }

        [TestMethod]
        public void ShouldRemoveSubscriberFromListOnUnregistration()
        {
            EventBroker broker = new EventBroker();
            string publishedEventName = "SomeEvent";
            EventHandler subscriber = delegate { };
            broker.RegisterSubscriber(publishedEventName, subscriber);

            broker.UnregisterSubscriber(publishedEventName, subscriber);

            Assert.AreEqual(0, new List<EventHandler>(broker.GetSubscribersFor(publishedEventName)).Count);
        }

    }
}
