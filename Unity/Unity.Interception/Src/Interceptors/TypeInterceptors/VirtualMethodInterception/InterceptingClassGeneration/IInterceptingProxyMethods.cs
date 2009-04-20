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
    internal static class IInterceptingProxyMethods
    {
        internal static MethodInfo GetPipeline
        {
            get { return typeof (IInterceptingProxy).GetMethod("GetPipeline"); }
        }

        internal static MethodInfo SetPipeline
        {
            get { return typeof (IInterceptingProxy).GetMethod("SetPipeline"); }
        }
    }
}
