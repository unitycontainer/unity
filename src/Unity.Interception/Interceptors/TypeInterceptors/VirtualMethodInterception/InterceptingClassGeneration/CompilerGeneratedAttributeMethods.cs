// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Utility;

namespace Unity.InterceptionExtension
{
    internal static class CompilerGeneratedAttributeMethods
    {
        public static ConstructorInfo CompilerGeneratedAttribute
        {
            get
            {
                return typeof(CompilerGeneratedAttribute).GetConstructor(new Type[0]);
            }
        }
    }
}
