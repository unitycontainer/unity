// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class CompilerGeneratedAttributeMethods
    {
        public static ConstructorInfo CompilerGeneratedAttribute
        {
            get { return StaticReflection.GetConstructorInfo(() => new CompilerGeneratedAttribute()); }
        }
    }
}
