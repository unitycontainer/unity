// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class MethodBaseMethods
    {
        internal static MethodInfo GetMethodFromHandle
        {
            get
            {
                return StaticReflection.GetMethodInfo(
                    () => MethodBase.GetMethodFromHandle(default(RuntimeMethodHandle)));
            }
        }

        internal static MethodInfo GetMethodForGenericFromHandle
        {
            get
            {
                return StaticReflection.GetMethodInfo(
                    () => MethodBase.GetMethodFromHandle(default(RuntimeMethodHandle), default(RuntimeTypeHandle)));
            }
        }
    }
}
