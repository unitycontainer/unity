using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// MethodInfo objects for the methods we need to generate
    /// calls to on IMethodInvocation.
    /// </summary>
    internal static class IMethodInvocationMethods
    {
        internal static MethodInfo CreateExceptionMethodReturn
        {
            get { return typeof(IMethodInvocation).GetMethod("CreateExceptionMethodReturn"); }
        }

        internal static MethodInfo CreateReturn
        {
            get { return typeof(IMethodInvocation).GetMethod("CreateMethodReturn"); }
        }

        internal static MethodInfo GetArguments
        {
            get { return typeof(IMethodInvocation).GetProperty("Arguments").GetGetMethod(); } 
        }
    }
}