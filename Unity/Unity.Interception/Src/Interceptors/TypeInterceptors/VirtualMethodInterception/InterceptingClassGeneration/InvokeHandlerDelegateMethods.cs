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
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class InvokeHandlerDelegateMethods
    {
        internal static ConstructorInfo InvokeHandlerDelegate
        {
            get
            {
                return typeof(InvokeHandlerDelegate).GetConstructor(
              Sequence.Collect(typeof(object), typeof(IntPtr)));
            }
        }
    }
}
