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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Security.Permissions;
using Microsoft.Practices.Unity.InterceptionExtension.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// This class provides the remoting-based interception mechanism. It is
    /// invoked by a call on the corresponding TransparentProxy
    /// object. It routes calls through the handlers as appropriate.
    /// </summary>
    public class InterceptingRealProxy : RealProxy, IRemotingTypeInfo, IInterceptingProxy
    {
        private readonly PipelineManager pipelines = new PipelineManager();
        private readonly object target;
        private string typeName;

        /// <summary>
        /// Creates a new <see cref="InterceptingRealProxy"/> instance that applies
        /// the given policies to the given target object.
        /// </summary>
        /// <param name="target">Target object to intercept calls to.</param>
        /// <param name="classToProxy">Type to return as the type being proxied.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class")]
        public InterceptingRealProxy(
            object target,
            Type classToProxy)
            : base(classToProxy)
        {
            Guard.ArgumentNotNull(target, "target");
            this.target = target;
            typeName = target.GetType().FullName;
        }

        /// <summary>
        /// Returns the target of this intercepted call.
        /// </summary>
        /// <value>The target object.</value>
        public object Target
        {
            get { return target; }
        }

        #region IInterceptingProxy Members

        /// <summary>
        /// Retrieve the pipeline assocated with the requested <paramref name="method"/>.
        /// </summary>
        /// <param name="method">Method for which the pipeline is being requested.</param>
        /// <returns>The handler pipeline for the given method. If no pipeline has
        /// been set, returns a new empty pipeline.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class.")]
        public HandlerPipeline GetPipeline(MethodBase method)
        {
            Guard.ArgumentNotNull(method, "method");
            return pipelines.GetPipeline(method.MetadataToken);
        }

        /// <summary>
        /// Set a new pipeline for a method.
        /// </summary>
        /// <param name="method">Method to apply the pipeline to.</param>
        /// <param name="pipeline">The new pipeline.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class.")]
        public void SetPipeline(MethodBase method, HandlerPipeline pipeline)
        {
            Guard.ArgumentNotNull(method, "method");
            pipelines.SetPipeline(method.MetadataToken, pipeline);
        }

        #endregion

        #region IRemotingTypeInfo Members

        ///<summary>
        ///Checks whether the proxy that represents the specified object type can be cast to the type represented by the <see cref="T:System.Runtime.Remoting.IRemotingTypeInfo"></see> interface.
        ///</summary>
        ///
        ///<returns>
        ///true if cast will succeed; otherwise, false.
        ///</returns>
        ///
        ///<param name="fromType">The type to cast to. </param>
        ///<param name="o">The object for which to check casting. </param>
        ///<exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission. </exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class.")]
        public bool CanCastTo(Type fromType, object o)
        {
            Guard.ArgumentNotNull(fromType, "fromType");
            Guard.ArgumentNotNull(o, "o");

            if (fromType == typeof (IInterceptingProxy))
            {
                return true;
            }

            if (fromType.IsAssignableFrom(o.GetType()))
            {
                return true;
            }
            return false;
        }

        ///<summary>
        ///Gets or sets the fully qualified type name of the server object in a <see cref="T:System.Runtime.Remoting.ObjRef"></see>.
        ///</summary>
        ///
        ///<value>
        ///The fully qualified type name of the server object in a <see cref="T:System.Runtime.Remoting.ObjRef"></see>.
        ///</value>
        ///
        ///<exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="Infrastructure" /></PermissionSet>
        public string TypeName
        {
            [method : SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
            get { return typeName; }
            [method : SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
            set { typeName = value; }
        }

        #endregion

        /// <summary>
        /// Executes a method call represented by the <paramref name="msg"/>
        /// parameter. The CLR will call this method when a method is called
        /// on the TransparentProxy. This method runs the invocation through
        /// the call handler pipeline and finally sends it down to the
        /// target object, and then back through the pipeline. 
        /// </summary>
        /// <param name="msg">An <see cref="IMessage"/> object that contains the information
        /// about the method call.</param>
        /// <returns>An <see cref="TransparentProxyMethodReturn"/> object contains the
        /// information about the target method's return value.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class")]
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override IMessage Invoke(IMessage msg)
        {
            Guard.ArgumentNotNull(msg, "msg");

            IMethodCallMessage callMessage = (IMethodCallMessage) msg;

            if (callMessage.MethodBase.DeclaringType == typeof (IInterceptingProxy))
            {
                return HandleInterceptingProxyMethod(callMessage);
            }

            HandlerPipeline pipeline = GetPipeline(TranslateInterfaceMethod(callMessage.MethodBase));

            TransparentProxyMethodInvocation invocation = new TransparentProxyMethodInvocation(callMessage, target);
            IMethodReturn result =
                pipeline.Invoke(
                    invocation,
                    delegate(IMethodInvocation input, GetNextHandlerDelegate getNext)
                    {
                        try
                        {
                            object returnValue = callMessage.MethodBase.Invoke(target, invocation.Arguments);
                            return input.CreateMethodReturn(returnValue, invocation.Arguments);
                        }
                        catch (TargetInvocationException ex)
                        {
                            // The outer exception will always be a reflection exception; we want the inner, which is
                            // the underlying exception.
                            return input.CreateExceptionMethodReturn(ex.InnerException);
                        }
                    });
            return ((TransparentProxyMethodReturn) result).ToMethodReturnMessage();
        }

        private IMessage HandleInterceptingProxyMethod(IMethodCallMessage callMessage)
        {
            switch (callMessage.MethodName)
            {
            case "GetPipeline":
                return ExecuteGetPipeline(callMessage);
            case "SetPipeline":
                return ExecuteSetPipeline(callMessage);
            }
            throw new InvalidOperationException();
        }

        private IMessage ExecuteGetPipeline(IMethodCallMessage callMessage)
        {
            MethodBase method = (MethodBase) callMessage.InArgs[0];
            method = TranslateInterfaceMethod(method);
            HandlerPipeline pipeline = GetPipeline(method);
            return new ReturnMessage(pipeline, new object[0], 0, callMessage.LogicalCallContext, callMessage);
        }

        private IMessage ExecuteSetPipeline(IMethodCallMessage callMessage)
        {
            MethodBase method = (MethodBase) callMessage.InArgs[0];
            method = TranslateInterfaceMethod(method);
            HandlerPipeline pipeline = (HandlerPipeline) callMessage.InArgs[1];
            SetPipeline(method, pipeline);
            return new ReturnMessage(null, new object[0], 0, callMessage.LogicalCallContext, callMessage);
        }

        /// <summary>
        /// Given a MethodBase, if it's for an interface method, return the MethodBase
        /// for the method that implements the interface method. If it's not an
        /// interface method, do nothing.
        /// </summary>
        /// <param name="method">Original Method</param>
        /// <returns>The implementing method.</returns>
        private MethodBase TranslateInterfaceMethod(MethodBase method)
        {
            // TODO: Figure out what to do with the module!
            if (!method.DeclaringType.IsInterface)
            {
                return method;
            }

            InterfaceMapping map = target.GetType().GetInterfaceMap(method.DeclaringType);
            for (int i = 0; i < map.InterfaceMethods.Length; ++i)
            {
                if (map.InterfaceMethods[i] == method)
                {
                    return map.TargetMethods[i];
                }
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                Resources.InterfaceMethodNotImplemented,
                method.DeclaringType.Name, method.Name, target.GetType().Name));
        }
    }
}
