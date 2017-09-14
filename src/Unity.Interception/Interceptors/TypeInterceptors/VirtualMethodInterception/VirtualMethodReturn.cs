// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Unity.InterceptionExtension
{
    /// <summary>
    /// An implementation of <see cref="IMethodReturn"/> used by
    /// the virtual method interception mechanism.
    /// </summary>
    public class VirtualMethodReturn : IMethodReturn
    {
        private ParameterCollection outputs;
        private Exception exception;
        private IDictionary<string, object> invocationContext;
        private object returnValue;

        /// <summary>
        /// Construct a <see cref="VirtualMethodReturn"/> instance that returns
        /// a value.
        /// </summary>
        /// <param name="originalInvocation">The method invocation.</param>
        /// <param name="returnValue">Return value (should be null if method returns void).</param>
        /// <param name="arguments">All arguments (including current values) passed to the method.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public VirtualMethodReturn(IMethodInvocation originalInvocation, object returnValue, object[] arguments)
        {
            Unity.Utility.Guard.ArgumentNotNull(originalInvocation, "originalInvocation");

            invocationContext = originalInvocation.InvocationContext;
            this.returnValue = returnValue;
            outputs = new ParameterCollection(arguments, originalInvocation.MethodBase.GetParameters(),
                delegate(ParameterInfo pi) { return pi.ParameterType.IsByRef; });
        }

        /// <summary>
        /// Construct a <see cref="VirtualMethodReturn"/> instance for when the target method throws an exception.
        /// </summary>
        /// <param name="originalInvocation">The method invocation.</param>
        /// <param name="exception">Exception that was thrown.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public VirtualMethodReturn(IMethodInvocation originalInvocation, Exception exception)
        {
            Unity.Utility.Guard.ArgumentNotNull(originalInvocation, "originalInvocation");
            
            invocationContext = originalInvocation.InvocationContext;
            this.exception = exception;
            outputs = new ParameterCollection(new object[0], new ParameterInfo[0], delegate { return false; });
        }

        /// <summary>
        /// The collection of output parameters. If the method has no output
        /// parameters, this is a zero-length list (never null).
        /// </summary>
        public IParameterCollection Outputs
        {
            get { return outputs; }
        }

        /// <summary>
        /// Returns value from the method call.
        /// </summary>
        /// <remarks>This value is null if the method has no return value.</remarks>
        public object ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }

        /// <summary>
        /// If the method threw an exception, the exception object is here.
        /// </summary>
        public Exception Exception
        {
            get { return exception; }
            set { exception = value; }
        }

        /// <summary>
        /// Retrieves a dictionary that can be used to store arbitrary additional
        /// values. This allows the user to pass values between call handlers.
        /// </summary>
        /// <remarks>This is guaranteed to be the same dictionary that was used
        /// in the IMethodInvocation object, so handlers can set context
        /// properties in the pre-call phase and retrieve them in the after-call phase.
        /// </remarks>
        public IDictionary<string, object> InvocationContext
        {
            get { return invocationContext; }
        }
    }
}
