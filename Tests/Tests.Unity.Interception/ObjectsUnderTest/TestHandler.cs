namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest
{
    public class TestHandler : ICallHandler
    {
        #region ICallHandler Members

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            var methodName = input.MethodBase.Name;
            var target = input.Target;

            return getNext()(input, getNext);
        }

        public int Order { get; set; }

        #endregion
    }
}