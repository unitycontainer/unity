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

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class IMethodReturnMethods
    {
        internal static MethodInfo GetException
        {
            get { return typeof (IMethodReturn).GetProperty("Exception").GetGetMethod(); } 
        }
        internal static MethodInfo GetReturnValue
        {
            get { return typeof(IMethodReturn).GetProperty("ReturnValue").GetGetMethod(); }
        }

        internal static MethodInfo GetOutputs
        {
            get { return typeof (IMethodReturn).GetProperty("Outputs").GetGetMethod(); }
        }
    }
}
