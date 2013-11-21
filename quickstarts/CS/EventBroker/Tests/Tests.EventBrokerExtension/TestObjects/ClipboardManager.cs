// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using SimpleEventBroker;

namespace Tests.EventBrokerExtension.TestObjects
{
    class ClipboardManager
    {
        // turn off "event is never used" warning
        #pragma warning disable 67

        [Publishes("cut")]
        public event EventHandler Cut;

        [Publishes("copy")]
        public event EventHandler Copy;

        [Publishes("paste")]
        public event EventHandler Paste;

        #pragma warning restore 67

        [SubscribesTo("copy")]
        public void OnCopy(object sender, EventArgs e)
        {
        }

        [SubscribesTo("clipboard data available")]
        public void OnClipboardDataAvailable(object sender, EventArgs e)
        {
            
        }
    }
}
