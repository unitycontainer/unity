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
                return typeof(MethodBase).GetMethod(nameof(MethodBase.GetMethodFromHandle));
            }
        }

        internal static MethodInfo GetMethodForGenericFromHandle
        {
            get
            {
                return typeof(MethodBase).GetMethod(nameof(MethodBase.GetMethodFromHandle));
            }
        }
    }
}
