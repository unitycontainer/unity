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
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// An instance interceptor that works by generating a
    /// proxy class on the fly for a single interface.
    /// </summary>
    public class InterfaceInterceptor : IInstanceInterceptor
    {
        private static readonly Dictionary<Type, Type> interceptorClasses = new Dictionary<Type, Type>();
        #region IInstanceInterceptor Members

        /// <summary>
        /// Can this interceptor generate a proxy for the given type?
        /// </summary>
        /// <param name="t">Type to check.</param>
        /// <returns>True if interception is possible, false if not.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done via Guard class.")]
        public bool CanIntercept(Type t)
        {
            Guard.ArgumentNotNull(t, "t");
            return t.IsInterface;
        }

        /// <summary>
        /// Returns a sequence of methods on the given type that can be
        /// intercepted.
        /// </summary>
        /// <param name="interceptedType">Type that was specified when this interceptor
        /// was created (typically an interface).</param>
        /// <param name="implementationType">The concrete type of the implementing object.</param>
        /// <returns>Sequence of <see cref="MethodInfo"/> objects.</returns>
        public IEnumerable<MethodImplementationInfo> GetInterceptableMethods(Type interceptedType, Type implementationType)
        {
            Guard.ArgumentNotNull(interceptedType, "interceptedType");
            Guard.ArgumentNotNull(implementationType, "implementationType");

            InterfaceMapping mapping = implementationType.GetInterfaceMap(interceptedType);
            for (int i = 0; i < mapping.InterfaceMethods.Length; ++i)
            {
                yield return new MethodImplementationInfo(mapping.InterfaceMethods[i], mapping.TargetMethods[i]);
            }
        }

        /// <summary>
        /// Create a proxy object that provides interception for <paramref name="target"/>.
        /// </summary>
        /// <param name="t">Type to generate the proxy of.</param>
        /// <param name="target">Object to create the proxy for.</param>
        /// <returns>The proxy object.</returns>
        public IInterceptingProxy CreateProxy(Type t, object target)
        {
            Type interceptorType;
            Type typeToProxy = t;

            if(t.IsGenericType)
            {
                typeToProxy = t.GetGenericTypeDefinition();
            }
            lock(interceptorClasses)
            {
                if(!interceptorClasses.ContainsKey(typeToProxy))
                {
                    InterfaceInterceptorClassGenerator generator = new InterfaceInterceptorClassGenerator(typeToProxy);
                    interceptorClasses[typeToProxy] = generator.CreateProxyType();
                }
                interceptorType = interceptorClasses[typeToProxy];
            }
            if(interceptorType.IsGenericTypeDefinition)
            {
                interceptorType = interceptorType.MakeGenericType(t.GetGenericArguments());
            }
            return (IInterceptingProxy)Activator.CreateInstance(interceptorType, target);
        }

        /// <summary>
        /// Given a <see cref="MethodImplementationInfo"/>, return the appropriate
        /// <see cref="MethodInfo"/> object to use to attach a <see cref="HandlerPipeline"/>
        /// to so that the handlers will get called when the method gets called.
        /// </summary>
        /// <param name="methodInfo">Original <see cref="MethodImplementationInfo"/> object that
        /// combines the <see cref="MethodInfo"/>s for an interface method and the corresponding implementation.</param>
        /// <returns>The <see cref="MethodInfo"/> object to pass to the <see cref="IInterceptingProxy.SetPipeline"/> method.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
           Justification = "Validation done via Guard class.")]
        public MethodInfo MethodInfoForPipeline(MethodImplementationInfo methodInfo)
        {
            Guard.ArgumentNotNull(methodInfo, "methodInfo");
            return methodInfo.InterfaceMethodInfo;
        }

        #endregion
    }
}
