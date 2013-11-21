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

namespace SimpleEventBroker
{
    public class PublishedEvent
    {
        private List<object> publishers;
        private List<EventHandler> subscribers;

        public PublishedEvent()
        {
            publishers = new List<object>();
            subscribers = new List<EventHandler>();
        }

        public IEnumerable<object> Publishers
        {
            get
            {
                foreach(object publisher in publishers)
                {
                    yield return publisher;
                }
            }
        }

        public IEnumerable<EventHandler> Subscribers
        {
            get
            {
                foreach(EventHandler subscriber in subscribers)
                {
                    yield return subscriber;
                }
            }
        }

        public bool HasPublishers
        {
            get { return publishers.Count > 0; }
        }

        public bool HasSubscribers
        {
            get { return subscribers.Count > 0; }
        }
        
        public void AddPublisher(object publisher, string eventName)
        {
            publishers.Add(publisher);
            EventInfo targetEvent = publisher.GetType().GetEvent(eventName);
            GuardEventExists(eventName, publisher, targetEvent);

            MethodInfo addEventMethod = targetEvent.GetAddMethod();
            GuardAddMethodExists(targetEvent);

            EventHandler newSubscriber = OnPublisherFiring;
            addEventMethod.Invoke(publisher, new object[] { newSubscriber });
        }

        public void RemovePublisher(object publisher, string eventName)
        {
            publishers.Remove(publisher);
            EventInfo targetEvent = publisher.GetType().GetEvent(eventName);
            GuardEventExists(eventName, publisher, targetEvent);

            MethodInfo removeEventMethod = targetEvent.GetRemoveMethod();
            GuardRemoveMethodExists(targetEvent);

            EventHandler subscriber = OnPublisherFiring;
            removeEventMethod.Invoke(publisher, new object[] {subscriber});
        }

        public void AddSubscriber(EventHandler subscriber)
        {
            subscribers.Add(subscriber);
        }

        public void RemoveSubscriber(EventHandler subscriber)
        {
            subscribers.Remove(subscriber);
        }

        private void OnPublisherFiring(object sender, EventArgs e)
        {
            foreach(EventHandler subscriber in subscribers)
            {
                subscriber(sender, e);
            }
        }

        private static void GuardEventExists(string eventName, object publisher, EventInfo targetEvent)
        {
            if(targetEvent == null)
            {
                throw new ArgumentException(string.Format("The event '{0}' is not implemented on type '{1}'",
                                                          eventName, publisher.GetType().Name));
            }
        } 

        private static void GuardAddMethodExists(EventInfo targetEvent)
        {
            if(targetEvent.GetAddMethod() == null)
            {
                throw new ArgumentException(string.Format("The event '{0}' does not have a public Add method",
                                                          targetEvent.Name));
            }
        }

        private static void GuardRemoveMethodExists(EventInfo targetEvent)
        {
            if (targetEvent.GetRemoveMethod() == null)
            {
                throw new ArgumentException(string.Format("The event '{0}' does not have a public Remove method",
                                                          targetEvent.Name));
            }
        }
    }
}
