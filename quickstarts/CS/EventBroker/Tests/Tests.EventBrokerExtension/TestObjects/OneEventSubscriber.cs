// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using SimpleEventBroker;

namespace Tests.EventBrokerExtension.TestObjects
{
    class OneEventSubscriber
    {
        [SubscribesTo("copy")]
        public void OnCopy(object sender, EventArgs e)
        {
            // Nothing needed
        }
    }
}
