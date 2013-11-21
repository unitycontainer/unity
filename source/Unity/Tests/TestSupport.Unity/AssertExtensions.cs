using System;
using System.Reflection;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.TestSupport
{
    public static class AssertExtensions
    {
        public static void AssertException<TException>(Action action)
            where TException : Exception
        {
            AssertException<TException>(action, (e) => { });
        }

        public static void AssertException<TException>(Action action, Action<TException> callback)
           where TException : Exception
        {
            try
            {
                action();
                Assert.Fail("Expected exception of type {0}", typeof(TException).GetTypeInfo().Name);
            }
            catch (TException e)
            {
                callback(e);
            }
        }


    }
}
