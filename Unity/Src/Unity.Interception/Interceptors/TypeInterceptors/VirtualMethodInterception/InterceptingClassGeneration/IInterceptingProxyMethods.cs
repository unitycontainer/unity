using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class IInterceptingProxyMethods
    {
        internal static MethodInfo GetPipeline
        {
            get { return typeof (IInterceptingProxy).GetMethod("GetPipeline"); }
        }

        internal static MethodInfo SetPipeline
        {
            get { return typeof (IInterceptingProxy).GetMethod("SetPipeline"); }
        }
    }
}