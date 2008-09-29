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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Security.Permissions;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// This class provides the remoting-based interception mechanism. It is
    /// invoked by a call on the corresponding TransparentProxy
    /// object. The InterceptingRealProxy reads the <see cref="PolicySet"/> at
    /// construction time, sets up the collection of <see cref="HandlerPipeline"/>s,
    /// and routes calls through the handlers at invocation time.
    /// </summary>
    public class InterceptingRealProxy : RealProxy, IRemotingTypeInfo
    {
        private readonly object target;
        private string typeName;

        private Dictionary<MethodBase, HandlerPipeline> memberHandlers;

        /// <summary>
        /// Creates a new <see cref="InterceptingRealProxy"/> instance that applies
        /// the given policies to the given target object.
        /// </summary>
        /// <param name="target">Target object to intercept calls to.</param>
        /// <param name="classToProxy">Type to return as the type being proxied.</param>
        /// <param name="policies"><see cref="PolicySet"/> that determines which
        /// handlers are installed.</param>
        /// <param name="container">The <see cref="IUnityContainer"/> to use when creating handlers,
        /// if necessary.</param>
        public InterceptingRealProxy(
            object target,
            Type classToProxy,
            PolicySet policies,
            IUnityContainer container)
            : base(classToProxy)
        {
            this.target = target;
            this.typeName = target.GetType().FullName;
            Type targetType = target.GetType();

            memberHandlers = new Dictionary<MethodBase, HandlerPipeline>();

            AddHandlersForType(targetType, policies, container);

            Type baseTypeIter = targetType.BaseType;
            while (baseTypeIter != null && baseTypeIter != typeof(object))
            {
                AddHandlersForType(baseTypeIter, policies, container);
                baseTypeIter = baseTypeIter.BaseType;
            }

            foreach (Type inter in targetType.GetInterfaces())
            {
                AddHandlersForInterface(targetType, inter);
            }
        }

        private void AddHandlersForType(Type classToProxy, PolicySet policies, IUnityContainer container)
        {
            foreach (MethodInfo member in classToProxy.GetMethods())
            {
                IEnumerable<ICallHandler> handlers = policies.GetHandlersFor(member, container);
                HandlerPipeline pipeline = new HandlerPipeline(handlers);
                memberHandlers[member] = pipeline;
            }
        }

        private void AddHandlersForInterface(Type targetType, Type itf)
        {
            InterfaceMapping itfMapping = targetType.GetInterfaceMap(itf);
            int numMethods = itfMapping.InterfaceMethods.Length;
            for (int i = 0; i < numMethods; ++i)
            {
                if (memberHandlers.ContainsKey(itfMapping.TargetMethods[i]))
                {
                    memberHandlers[itfMapping.InterfaceMethods[i]] =
                        memberHandlers[itfMapping.TargetMethods[i]];
                }
            }
        }

        /// <summary>
        /// Returns the target of this intercepted call.
        /// </summary>
        /// <value>The target object.</value>
        public object Target
        {
            get { return target; }
        }

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
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage callMessage = (IMethodCallMessage)msg;

            HandlerPipeline pipeline;
            if (memberHandlers.ContainsKey(callMessage.MethodBase))
            {
                pipeline = memberHandlers[callMessage.MethodBase];
            }
            else
            {
                pipeline = new HandlerPipeline();
            }

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
            return ((TransparentProxyMethodReturn)result).ToMethodReturnMessage();
        }

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

            if (fromType.IsAssignableFrom(o.GetType()))
            {
                return true;
            }
            return false;
        }

        #region IRemotingTypeInfo Members

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
            [method: SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
            get { return typeName; }
            [method: SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
            set { typeName = value; }
        }

        #endregion
    }
}
