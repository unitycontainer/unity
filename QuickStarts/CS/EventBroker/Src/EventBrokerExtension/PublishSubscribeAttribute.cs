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
