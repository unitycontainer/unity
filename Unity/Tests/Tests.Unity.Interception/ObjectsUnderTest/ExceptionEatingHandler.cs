using System;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest
{
    internal class ExceptionEatingHandler : ICallHandler
    {
        private int order;
        private Exception thrownException;

        public Exception ThrownException
        {
            get { return thrownException; }
        }

        #region ICallHandler Members

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            IMethodReturn result = getNext()(input, getNext);

            if (result.Exception != null)
            {
                thrownException = result.Exception;
                result.Exception = null;
            }

            return result;
        }

        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        #endregion
    }
}