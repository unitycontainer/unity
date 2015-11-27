// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using ObjectBuilder2;

namespace Unity.InterceptionExtension
{
    internal static class InvokeInterceptionBehaviorDelegateMethods
    {
        internal static ConstructorInfo InvokeInterceptionBehaviorDelegate
        {
            get
            {
                // cannot use static reflection with delegate types
                return typeof(InvokeInterceptionBehaviorDelegate)
                    .GetConstructor(Sequence.Collect(typeof(object), typeof(IntPtr)));
            }
        }
    }
}
