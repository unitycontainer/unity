// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleEventBroker
{
    [AttributeUsage(AttributeTargets.Event, Inherited = true)]
    public class PublishesAttribute : PublishSubscribeAttribute
    {
        public PublishesAttribute(string publishedEventName) : base(publishedEventName)
        {
        }
    }
}
