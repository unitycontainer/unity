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

using System.Threading;

namespace TestSupport.Unity
{
    public class Barrier
    {
        private readonly object lockObj = new object();
        private readonly int originalCount;
        private int currentCount;

        public Barrier(int count)
        {
            originalCount = count;
            currentCount = count;
        }

        public void Await()
        {
            lock (lockObj)
            {
                if (currentCount > 0)
                {
                    --currentCount;

                    if (currentCount == 0)
                    {
                        Monitor.PulseAll(lockObj);
                        currentCount = originalCount;
                    }
                    else
                    {
                        Monitor.Wait(lockObj);
                    }
                }
            }
        }
    }
}
