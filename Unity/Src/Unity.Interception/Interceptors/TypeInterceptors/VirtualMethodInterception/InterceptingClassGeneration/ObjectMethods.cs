//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.Practices.Unity.InterceptionExtension.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration
{
    internal static class ObjectMethods
    {
        // Zero argument constructor
        internal static ConstructorInfo Constructor { get { return typeof (object).GetConstructor(new Type[0]);  } }
    }
}
