using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class IMethodReturnMethods
    {
        internal static MethodInfo GetException
        {
            get { return typeof (IMethodReturn).GetProperty("Exception").GetGetMethod(); } 
        }
        internal static MethodInfo GetReturnValue
        {
            get { return typeof(IMethodReturn).GetProperty("ReturnValue").GetGetMethod(); }
        }
    }
}