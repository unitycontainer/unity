using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class HandlerPipelineMethods
    {
        internal static MethodInfo Invoke
        {
            get { return typeof(HandlerPipeline).GetMethod("Invoke"); }
        }
    }
}