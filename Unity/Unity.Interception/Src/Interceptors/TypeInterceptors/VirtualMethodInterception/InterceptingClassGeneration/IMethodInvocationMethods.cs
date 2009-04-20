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
    /// <summary>
    /// MethodInfo objects for the methods we need to generate
    /// calls to on IMethodInvocation.
    /// </summary>
    internal static class IMethodInvocationMethods
    {
        internal static MethodInfo CreateExceptionMethodReturn
        {
            get { return typeof(IMethodInvocation).GetMethod("CreateExceptionMethodReturn"); }
        }

        internal static MethodInfo CreateReturn
        {
            get { return typeof(IMethodInvocation).GetMethod("CreateMethodReturn"); }
        }

        internal static MethodInfo GetArguments
        {
            get { return typeof(IMethodInvocation).GetProperty("Arguments").GetGetMethod(); } 
        }
    }
}
