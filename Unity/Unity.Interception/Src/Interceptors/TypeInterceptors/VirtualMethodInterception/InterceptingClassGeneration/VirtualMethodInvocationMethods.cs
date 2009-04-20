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

using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class VirtualMethodInvocationMethods
    {
        internal static ConstructorInfo VirtualMethodInvocation
        {
            get
            {
                return typeof(VirtualMethodInvocation).GetConstructor(Sequence.Collect(typeof(object), typeof(MethodBase), typeof(object[])));
            }
        }
    }
}
