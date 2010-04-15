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
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// An implementation of <see cref="IMethodReturn"/> that wraps the
    /// remoting call and return messages.
    /// </summary>
    [SecurityCritical(SecurityCriticalScope.Everything)]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
    class TransparentProxyMethodReturn : IMethodReturn
    {
        private readonly IMethodCallMessage callMessage;
        private readonly ParameterCollection outputs;
        private readonly IDictionary invocationContext;
        private readonly object[] arguments;
        private object returnValue;
        private Exception exception;

        /// <summary>
        /// Creates a new <see cref="TransparentProxyMethodReturn"/> object that contains a
        /// return value.
        /// </summary>
        /// <param name="callMessage">The original call message that invoked the method.</param>
        /// <param name="returnValue">Return value from the method.</param>
        /// <param name="arguments">Collections of arguments passed to the method (including the new
        /// values of any out params).</param>
        /// <param name="invocationContext">Invocation context dictionary passed into the call.</param>
        public TransparentProxyMethodReturn(IMethodCallMessage callMessage, object returnValue, object[] arguments, IDictionary invocationContext)
        {
            this.callMessage = callMessage;
            this.invocationContext = invocationContext;
            this.arguments = arguments;
            this.returnValue = returnValue;
            this.outputs = new TransparentProxyOutputParameterCollection(callMessage, arguments);
        }

        /// <summary>
        /// Creates a new <see cref="TransparentProxyMethodReturn"/> object that contains an
        /// exception thrown by the target.
        /// </summary>
        /// <param name="ex">Exception that was thrown.</param>
        /// <param name="callMessage">The original call message that invoked the method.</param>
        /// <param name="invocationContext">Invocation context dictionary passed into the call.</param>
        public TransparentProxyMethodReturn(Exception ex, IMethodCallMessage callMessage, IDictionary invocationContext)
        {
            this.callMessage = callMessage;
            this.invocationContext = invocationContext;
            this.exception = ex;
            this.arguments = new object[0];
            this.outputs = new ParameterCollection(this.arguments, new ParameterInfo[0], pi => false);
        }

        /// <summary>
        /// The collection of output parameters. If the method has no output
        /// parameters, this is a zero-length list (never null).
        /// </summary>
        /// <value>The output parameter collection.</value>
        public IParameterCollection Outputs
        {
            get { return outputs; }
        }

        /// <summary>
        /// Return value from the method call.
        /// </summary>
        /// <remarks>This value is null if the method has no return value.</remarks>
        /// <value>The return value.</value>
        public object ReturnValue
        {
            get { return returnValue; }
            set
            {
                returnValue = value;
            }
        }

        /// <summary>
        /// If the method threw an exception, the exception object is here.
        /// </summary>
        /// <value>The exception, or null if no exception was thrown.</value>
        public Exception Exception
        {
            get { return exception; }
            set
            {
                exception = value;
            }
        }

        /// <summary>
        /// Retrieves a dictionary that can be used to store arbitrary additional
        /// values. This allows the user to pass values between call handlers.
        /// </summary>
        /// <remarks>This is guaranteed to be the same dictionary that was used
        /// in the IMethodInvocation object, so handlers can set context
        /// properties in the pre-call phase and retrieve them in the after-call phase.
        /// </remarks>
        /// <value>The invocation context dictionary.</value>
        public IDictionary InvocationContext
        {
            get { return invocationContext; }
        }

        /// <summary>
        /// Constructs a <see cref="IMethodReturnMessage"/> for the remoting
        /// infrastructure based on the contents of this object.
        /// </summary>
        /// <returns>The <see cref="IMethodReturnMessage"/> instance.</returns>
        public IMethodReturnMessage ToMethodReturnMessage()
        {
            if (exception == null)
            {
                return
                    new ReturnMessage(returnValue, arguments, arguments.Length,
                        callMessage.LogicalCallContext, callMessage);
            }
            else
            {
                return new ReturnMessage(exception, callMessage);
            }
        }
    }
}
