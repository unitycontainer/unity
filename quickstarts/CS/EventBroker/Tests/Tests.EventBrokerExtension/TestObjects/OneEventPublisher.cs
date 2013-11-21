// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using SimpleEventBroker;

namespace Tests.EventBrokerExtension.TestObjects
{
    class OneEventPublisher
    {
#pragma warning disable 67 // Disable "Event is never used" warning
        [Publishes("paste")]
        public event EventHandler Pasting;
    }
}
