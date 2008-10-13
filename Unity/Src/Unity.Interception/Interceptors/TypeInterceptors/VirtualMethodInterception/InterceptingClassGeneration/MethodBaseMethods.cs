using System;
using System.Reflection;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class MethodBaseMethods
    {
        internal static MethodInfo GetMethodFromHandle
        {
            get
            {
                return typeof(MethodBase).GetMethod("GetMethodFromHandle",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    Sequence.Collect(typeof(RuntimeMethodHandle)), null);
            }
        }

        internal static MethodInfo GetMethodForGenericFromHandle
        {
            get
            {
                return typeof(MethodBase).GetMethod("GetMethodFromHandle", 
                    BindingFlags.Public | BindingFlags.Static,
                    null, 
                    Sequence.Collect(typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle)), 
                    null);
            }
        }


        internal static MethodInfo GetMetadataToken
        {
            get
            {
                return typeof(MethodBase).GetProperty("MetadataToken").GetGetMethod();
            }
        }
    }
}
