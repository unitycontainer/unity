// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace SimpleEventBroker
{
    /// <summary>
    /// Base class for the two publish / subscribe attributes. Stores
    /// the event name to be published or subscribed to.
    /// </summary>
    public abstract class PublishSubscribeAttribute : Attribute
    {
        private string eventName;

        protected PublishSubscribeAttribute(string eventName)
        {
            this.eventName = eventName;
        }

        public string EventName
        {
            get { return eventName; }
            set { eventName = value; }
        }
    }
}
