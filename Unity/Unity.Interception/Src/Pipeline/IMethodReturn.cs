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
using System.Collections;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// This interface is used to represent the return value from a method.
    /// An implementation of IMethodReturn is returned by call handlers, and
    /// each handler can manipulate the parameters, return value, or add an
    /// exception on the way out.
    /// </summary>
    public interface IMethodReturn
    {
        /// <summary>
        /// The collection of output parameters. If the method has no output
        /// parameters, this is a zero-length list (never null).
        /// </summary>
        IParameterCollection Outputs { get; }

        /// <summary>
        /// Returns value from the method call.
        /// </summary>
        /// <remarks>This value is null if the method has no return value.</remarks>
        object ReturnValue { get; set; }

        /// <summary>
        /// If the method threw an exception, the exception object is here.
        /// </summary>
        Exception Exception { get; set; }

        /// <summary>
        /// Retrieves a dictionary that can be used to store arbitrary additional
        /// values. This allows the user to pass values between call handlers.
        /// </summary>
        /// <remarks>This is guaranteed to be the same dictionary that was used
        /// in the IMethodInvocation object, so handlers can set context
        /// properties in the pre-call phase and retrieve them in the after-call phase.
        /// </remarks>
        IDictionary InvocationContext { get; }
    }
}
