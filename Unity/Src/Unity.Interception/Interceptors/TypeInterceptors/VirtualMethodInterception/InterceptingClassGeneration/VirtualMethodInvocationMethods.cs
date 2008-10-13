using System.Reflection;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class VirtualMethodInvocationMethods
    {
        internal static ConstructorInfo VirtualMethodInvocation
        {
            get
            {
                return typeof(VirtualMethodInvocation).GetConstructor(Sequence.Collect(typeof(object), typeof(MethodBase), typeof(object[])));
            }
        }
    }
}