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
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Implementation of <see cref="IMethodInvocation"/> used
    /// by the virtual method interceptor.
    /// </summary>
    public class VirtualMethodInvocation : IMethodInvocation
    {
        private readonly ParameterCollection inputs;
        private readonly ParameterCollection arguments;
        private readonly Hashtable context;
        private readonly object target;
        private readonly MethodBase targetMethod;

        /// <summary>
        /// Construct a new <see cref="VirtualMethodInvocation"/> instance for the
        /// given target object and method, passing the <paramref name="parameterValues"/>
        /// to the target method.
        /// </summary>
        /// <param name="target">Object that is target of this invocation.</param>
        /// <param name="targetMethod">Method on <paramref name="target"/> to call.</param>
        /// <param name="parameterValues">Values for the parameters.</param>
        public VirtualMethodInvocation(object target, MethodBase targetMethod, params object[] parameterValues)
        {
            this.target = target;
            this.targetMethod = targetMethod;
            this.context = new Hashtable();

            ParameterInfo[] targetParameters = targetMethod.GetParameters();
            this.arguments = new ParameterCollection(parameterValues, targetParameters, param => true );
            this.inputs = new ParameterCollection(parameterValues, targetParameters, param => !param.IsOut);
        }

        /// <summary>
        /// Gets the inputs for this call.
        /// </summary>
        public IParameterCollection Inputs
        {
            get { return inputs; }
        }

        /// <summary>
        /// Collection of all parameters to the call: in, out and byref.
        /// </summary>
        public IParameterCollection Arguments
        {
            get { return arguments; }
        }

        /// <summary>
        /// Retrieves a dictionary that can be used to store arbitrary additional
        /// values. This allows the user to pass values between call handlers.
        /// </summary>
        public IDictionary InvocationContext
        {
            get { return context; }
        }

        /// <summary>
        /// The object that the call is made on.
        /// </summary>
        public object Target
        {
            get { return target; }
        }

        /// <summary>
        /// The method on Target that we're aiming at.
        /// </summary>
        public MethodBase MethodBase
        {
            get { return targetMethod; }
        }

        /// <summary>
        /// Factory method that creates the correct implementation of
        /// IMethodReturn.
        /// </summary>
        /// <param name="returnValue">Return value to be placed in the IMethodReturn object.</param>
        /// <param name="outputs">All arguments passed or returned as out/byref to the method. 
        /// Note that this is the entire argument list, including in parameters.</param>
        /// <returns>New IMethodReturn object.</returns>
        public IMethodReturn CreateMethodReturn(object returnValue, params object[] outputs)
        {
            return new VirtualMethodReturn(this, returnValue, outputs);
        }

        /// <summary>
        /// Factory method that creates the correct implementation of
        /// IMethodReturn in the presence of an exception.
        /// </summary>
        /// <param name="ex">Exception to be set into the returned object.</param>
        /// <returns>New IMethodReturn object</returns>
        public IMethodReturn CreateExceptionMethodReturn(Exception ex)
        {
            return new VirtualMethodReturn(this, ex);
        }
    }
}
