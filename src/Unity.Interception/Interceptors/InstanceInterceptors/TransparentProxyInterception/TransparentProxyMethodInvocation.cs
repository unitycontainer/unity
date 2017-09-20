// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Permissions;

namespace Unity.InterceptionExtension
{
    /// <summary>
    /// An implementation of <see cref="IMethodInvocation"/> that wraps the
    /// remoting based <see cref="IMethodCallMessage"/> in the PIAB call
    /// interface.
    /// </summary>
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
    public sealed class TransparentProxyMethodInvocation : IMethodInvocation
    {
        private IMethodCallMessage callMessage;
        private TransparentProxyInputParameterCollection inputParams;
        private ParameterCollection allParams;
        private Dictionary<string, object> invocationContext;
        private object target;
        private object[] arguments;

        /// <summary>
        /// Creates a new <see cref="IMethodInvocation"/> implementation that wraps
        /// the given <paramref name="callMessage"/>, with the given ultimate
        /// target object.
        /// </summary>
        /// <param name="callMessage">Remoting call message object.</param>
        /// <param name="target">Ultimate target of the method call.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public TransparentProxyMethodInvocation(IMethodCallMessage callMessage, object target)
        {
            Unity.Utility.Guard.ArgumentNotNull(callMessage, "callMessage");

            this.callMessage = callMessage;
            this.invocationContext = new Dictionary<string, object>();
            this.target = target;
            this.arguments = callMessage.Args;
            this.inputParams = new TransparentProxyInputParameterCollection(callMessage, this.arguments);
            this.allParams =
                new ParameterCollection(arguments, callMessage.MethodBase.GetParameters(), info => true);
        }

        /// <summary>
        /// Gets the inputs for this call.
        /// </summary>
        /// <value>The input collection.</value>
        public IParameterCollection Inputs
        {
            [SecuritySafeCritical]
            get { return inputParams; }
        }

        /// <summary>
        /// Collection of all parameters to the call: in, out and byref.
        /// </summary>
        /// <value>The arguments collection.</value>
        IParameterCollection IMethodInvocation.Arguments
        {
            [SecuritySafeCritical]
            get { return allParams; }
        }

        /// <summary>
        /// Retrieves a dictionary that can be used to store arbitrary additional
        /// values. This allows the user to pass values between call handlers.
        /// </summary>
        /// <value>The invocation context dictionary.</value>
        public IDictionary<string, object> InvocationContext
        {
            [SecuritySafeCritical]
            get { return invocationContext; }
        }

        /// <summary>
        /// The object that the call is made on.
        /// </summary>
        /// <value>The target object.</value>
        public object Target
        {
            [SecuritySafeCritical]
            get { return target; }
        }

        /// <summary>
        /// The method on Target that we're aiming at.
        /// </summary>
        /// <value>The target method base.</value>
        public MethodBase MethodBase
        {
            [SecuritySafeCritical]
            get { return callMessage.MethodBase; }
        }

        /// <summary>
        /// Factory method that creates the correct implementation of
        /// IMethodReturn.
        /// </summary>
        /// <remarks>In this implementation we create an instance of <see cref="TransparentProxyMethodReturn"/>.</remarks>
        /// <param name="returnValue">Return value to be placed in the IMethodReturn object.</param>
        /// <param name="outputs">All arguments passed or returned as out/byref to the method. 
        /// Note that this is the entire argument list, including in parameters.</param>
        /// <returns>New IMethodReturn object.</returns>
        [SecuritySafeCritical]
        public IMethodReturn CreateMethodReturn(object returnValue, params object[] outputs)
        {
            return new TransparentProxyMethodReturn(callMessage, returnValue, outputs, invocationContext);
        }

        /// <summary>
        /// Factory method that creates the correct implementation of
        /// IMethodReturn in the presence of an exception.
        /// </summary>
        /// <param name="ex">Exception to be set into the returned object.</param>
        /// <returns>New IMethodReturn object</returns>
        [SecuritySafeCritical]
        public IMethodReturn CreateExceptionMethodReturn(Exception ex)
        {
            return new TransparentProxyMethodReturn(ex, callMessage, invocationContext);
        }

        /// <summary>
        /// Gets the collection of arguments being passed to the target.
        /// </summary>
        /// <remarks>This method exists because the underlying remoting call message
        /// does not let handlers change the arguments.</remarks>
        /// <value>Array containing the arguments to the target.</value>
        internal object[] Arguments
        {
            get { return arguments; }
        }
    }
}
