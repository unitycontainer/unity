using System;
using System.Reflection;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class InvokeHandlerDelegateMethods
    {
        internal static ConstructorInfo InvokeHandlerDelegate
        {
            get
            {
                return typeof(InvokeHandlerDelegate).GetConstructor(
              Sequence.Collect(typeof(object), typeof(IntPtr)));
            }
        }
    }
}