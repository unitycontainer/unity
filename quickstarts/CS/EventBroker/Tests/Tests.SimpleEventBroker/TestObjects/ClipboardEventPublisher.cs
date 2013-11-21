// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace EventBrokerTests.TestObjects
{
    class ClipboardEventPublisher
    {
        public event EventHandler Copy;
        public event EventHandler Paste;

        public int NumberOfCopyDelegates
        {
            get { return GetInvocationListLength(Copy); }
        }

        public int NumberOfPasteDelegates
        {
            get { return GetInvocationListLength(Paste); }
        }

        public void DoPaste()
        {
            if(Paste != null)
            {
                Paste(this, EventArgs.Empty);
            }
        }

        public void DoCopy()
        {
            if(Copy != null)
            {
                Copy(this, EventArgs.Empty);
            }
        }

        private int GetInvocationListLength(EventHandler @event)
        {
            if( @event == null )
            {
                return 0;
            }
            return @event.GetInvocationList().Length;
        }
    }
}
