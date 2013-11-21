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
    class ClipboardMonitor
    {
        private int numberOfCopyOperations;
        private int numberOfPasteOperations;

        public ClipboardMonitor()
        {
            numberOfPasteOperations = 0;
            numberOfCopyOperations = 0;
        }

        public int NumberOfCopyOperations
        {
            get { return numberOfCopyOperations; }
        }

        public int NumberOfPasteOperations
        {
            get { return numberOfPasteOperations; }
        }

        public void OnClipboardCopy(object sender, EventArgs e)
        {
            ++numberOfCopyOperations;
        }

        public void OnClipboardPaste(object sender, EventArgs e)
        {
            ++numberOfPasteOperations;
        }
    }
}
