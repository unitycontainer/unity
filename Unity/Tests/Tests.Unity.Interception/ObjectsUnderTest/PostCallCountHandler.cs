using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest
{
    class PostCallCountHandler : ICallHandler
    {
        private int order;
        private int callsCompleted = 0;

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            IMethodReturn result = getNext()(input, getNext);
            callsCompleted++;
            return result;
        }

        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public int CallsCompleted
        {
            get { return callsCompleted; }
        }
    }
}
