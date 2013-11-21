// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
