// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Unity.Utility;

namespace Unity.InterceptionExtension
{
    internal static class InterceptionBehaviorPipelineMethods
    {
        internal static ConstructorInfo Constructor
        {
            get { return typeof(IMethodReturn).GetConstructor(new Type[0]); }
        }

        internal static MethodInfo Add
        {
            get { return typeof(InterceptionBehaviorPipeline).GetMethod(nameof(InterceptionBehaviorPipeline.Add)); }
        }

        internal static MethodInfo Invoke
        {
            get { return typeof(InterceptionBehaviorPipeline).GetMethod(nameof(InterceptionBehaviorPipeline.Invoke)); }
        }
    }
}
