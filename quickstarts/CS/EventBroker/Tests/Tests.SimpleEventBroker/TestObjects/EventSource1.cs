// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace EventBrokerTests
{
    class EventSource1
    {
        public event EventHandler Event1;

        public void FireEvent1()
        {
            OnEvent1(this, EventArgs.Empty);
        }
        protected virtual void OnEvent1(object sender, EventArgs e)
        {
            if (Event1 != null)
            {
                Event1(sender, e);
            }
        }

        public int NumberOfEvent1Delegates
        {
            get
            {
                if( Event1 == null )
                {
                    return 0;
                }
                return Event1.GetInvocationList().Length;
            }
        }
    }
}
