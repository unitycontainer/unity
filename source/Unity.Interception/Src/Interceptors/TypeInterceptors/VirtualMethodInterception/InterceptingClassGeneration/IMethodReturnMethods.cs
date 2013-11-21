// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Reflection;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class IMethodReturnMethods
    {
        internal static MethodInfo GetException
        {
            get { return StaticReflection.GetPropertyGetMethodInfo((IMethodReturn imr) => imr.Exception); }
        }

        internal static MethodInfo GetReturnValue
        {
            get { return StaticReflection.GetPropertyGetMethodInfo((IMethodReturn imr) => imr.ReturnValue); }
        }

        internal static MethodInfo GetOutputs
        {
            get { return StaticReflection.GetPropertyGetMethodInfo((IMethodReturn imr) => imr.Outputs); }
        }
    }
}
