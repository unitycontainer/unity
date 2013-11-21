// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
                return StaticReflection.GetConstructorInfo(
                    () => new VirtualMethodInvocation(default(object), default(MethodBase)));
            }
        }
    }
}
