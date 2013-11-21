// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class ObjectMethods
    {
        // Zero argument constructor
        internal static ConstructorInfo Constructor { get { return StaticReflection.GetConstructorInfo(() => new object()); } }
    }
}
