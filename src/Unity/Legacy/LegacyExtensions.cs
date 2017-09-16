using System;
using System.Reflection;

namespace Unity
{
    internal static class LegacyExtensions
    {
        public static Delegate CreateDelegate(this MethodInfo method, Type delegateType)
        {
            return Delegate.CreateDelegate(delegateType, method);
        }

        public static Delegate CreateDelegate(this MethodInfo method, Type delegateType, object target)
        {
            return Delegate.CreateDelegate(delegateType, target, method);
        }
    }
}
