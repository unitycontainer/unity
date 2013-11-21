// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Reflection;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class IInterceptingProxyMethods
    {
        internal static MethodInfo AddInterceptionBehavior
        {
            get { return StaticReflection.GetMethodInfo<IInterceptingProxy>(ip => ip.AddInterceptionBehavior(null)); }
        }
    }
}
