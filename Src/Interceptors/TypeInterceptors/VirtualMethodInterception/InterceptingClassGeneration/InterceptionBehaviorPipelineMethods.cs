// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class InterceptionBehaviorPipelineMethods
    {
        internal static ConstructorInfo Constructor
        {
            get { return StaticReflection.GetConstructorInfo(() => new InterceptionBehaviorPipeline()); }
        }

        internal static MethodInfo Add
        {
            get { return StaticReflection.GetMethodInfo((InterceptionBehaviorPipeline pip) => pip.Add(null)); }
        }

        internal static MethodInfo Invoke
        {
            get { return StaticReflection.GetMethodInfo((InterceptionBehaviorPipeline pip) => pip.Invoke(null, null)); }
        }
    }
}
