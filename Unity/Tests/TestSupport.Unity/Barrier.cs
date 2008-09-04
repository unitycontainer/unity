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
