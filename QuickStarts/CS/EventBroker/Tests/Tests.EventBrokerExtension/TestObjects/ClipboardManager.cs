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
