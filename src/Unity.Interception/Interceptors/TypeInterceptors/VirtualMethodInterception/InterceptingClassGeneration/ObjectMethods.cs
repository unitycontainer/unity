// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Unity.Utility;

namespace Unity.InterceptionExtension
{
    internal static class ObjectMethods
    {
        // Zero argument constructor
        internal static ConstructorInfo Constructor { get { return StaticReflection.GetConstructorInfo(() => new object()); } }
    }
}
