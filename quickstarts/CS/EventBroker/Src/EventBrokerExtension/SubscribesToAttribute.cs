// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace SimpleEventBroker
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class SubscribesToAttribute : PublishSubscribeAttribute
    {
        public SubscribesToAttribute(string eventName) : base(eventName)
        {
        }
    }
}
