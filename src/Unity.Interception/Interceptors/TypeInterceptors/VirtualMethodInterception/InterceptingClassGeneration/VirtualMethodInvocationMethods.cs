// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Reflection;
using Unity.Utility;

namespace Unity.InterceptionExtension
{
    internal static class VirtualMethodInvocationMethods
    {
        internal static ConstructorInfo VirtualMethodInvocation
        {
            get
            {
                return StaticReflection.GetConstructorInfo(
                    () => new VirtualMethodInvocation(default(object), default(MethodBase)));
            }
        }
    }
}
