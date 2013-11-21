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
using Microsoft.Practices.Unity.Utility;

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
            get { return StaticReflection.GetMethodInfo((IMethodInvocation mi) => mi.CreateExceptionMethodReturn(default(Exception))); }
        }

        internal static MethodInfo CreateReturn
        {
            // Using static reflection causes an FxCop rule to throw an exception here, using plain old reflection instead
            // logged as https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=485609
            get { return typeof(IMethodInvocation).GetMethod("CreateMethodReturn"); }
        }

        internal static MethodInfo GetArguments
        {
            get { return StaticReflection.GetPropertyGetMethodInfo((IMethodInvocation mi) => mi.Arguments); }
        }
    }
}
