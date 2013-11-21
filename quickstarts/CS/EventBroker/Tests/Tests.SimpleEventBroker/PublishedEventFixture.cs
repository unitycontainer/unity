// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleEventBroker;

namespace EventBrokerTests
{
    /// <summary>
    /// Summary description for PublishedEventFixture
    /// </summary>
    [TestClass]
    public class PublishedEventFixture
    {
        [TestMethod]
        public void ShouldAddDelegateToPublishingEventOnAddingPublisher()
        {
            PublishedEvent pe = new PublishedEvent();
            EventSource1 publisher = new EventSource1();

            pe.AddPublisher(publisher, "Event1");

            Assert.AreEqual(1, publisher.NumberOfEvent1Delegates);
        }

        [TestMethod]
        public void ShouldRemoveDelegateWhenRemovingPublisher()
        {
            PublishedEvent pe = new PublishedEvent();
            EventSource1 publisher = new EventSource1();
            pe.AddPublisher(publisher, "Event1");

            pe.RemovePublisher(publisher, "Event1");

            Assert.AreEqual(0, publisher.NumberOfEvent1Delegates);
        }

        [TestMethod]
        public void ShouldCallSubscriberWhenPublisherFiresEvent()
        {
            PublishedEvent pe = new PublishedEvent();
            EventSource1 publisher = new EventSource1();
            pe.AddPublisher(publisher, "Event1");
            bool subscriberCalled = false;
            EventHandler subscriber = delegate { subscriberCalled = true; };
            pe.AddSubscriber(subscriber);

            publisher.FireEvent1();

            Assert.IsTrue(subscriberCalled);
        }

        [TestMethod]
        public void ShouldNotCallSubscriberAfterRemoval()
        {
            PublishedEvent pe = new PublishedEvent();
            EventSource1 publisher = new EventSource1();
            pe.AddPublisher(publisher, "Event1");
            int numberOfSubscriberCalls = 0;
            EventHandler subscriber = delegate { ++numberOfSubscriberCalls; };
            pe.AddSubscriber(subscriber);

            publisher.FireEvent1();

            pe.RemoveSubscriber(subscriber);
            publisher.FireEvent1();

            Assert.AreEqual(1, numberOfSubscriberCalls);
        }

        [TestMethod]
        public void ShouldMulticastEventsToMultipleSubscribers()
        {
            PublishedEvent pe = new PublishedEvent();
            EventSource1 publisher = new EventSource1();
            pe.AddPublisher(publisher, "Event1");
            bool subscriber1Called = false;
            EventHandler subscriber1 = delegate { subscriber1Called = true; };
            bool subscriber2Called = false;
            EventHandler subscriber2 = delegate { subscriber2Called = true; };
            pe.AddSubscriber(subscriber1);
            pe.AddSubscriber(subscriber2);

            publisher.FireEvent1();

            Assert.IsTrue(subscriber1Called);
            Assert.IsTrue(subscriber2Called);
        }

    }
}
