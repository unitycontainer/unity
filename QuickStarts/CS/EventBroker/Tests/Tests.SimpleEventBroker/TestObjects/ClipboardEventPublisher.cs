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
