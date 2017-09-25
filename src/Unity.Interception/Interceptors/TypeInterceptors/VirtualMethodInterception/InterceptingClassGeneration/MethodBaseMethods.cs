// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using ObjectBuilder2;
using Unity.Utility;

namespace Unity.InterceptionExtension
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
