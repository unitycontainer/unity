// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
        private static readonly Dictionary<GeneratedTypeKey, Type> InterceptorClasses =
            new Dictionary<GeneratedTypeKey, Type>(new GeneratedTypeKey.GeneratedTypeKeyComparer());

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
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Interceptable",
            Justification = "Spelling is fine")]
        public IEnumerable<MethodImplementationInfo> GetInterceptableMethods(
            Type interceptedType,
            Type implementationType)
        {
            Guard.ArgumentNotNull(interceptedType, "interceptedType");
            Guard.ArgumentNotNull(implementationType, "implementationType");

            return DoGetInterceptableMethods(interceptedType, implementationType);
        }

        private IEnumerable<MethodImplementationInfo> DoGetInterceptableMethods(
            Type interceptedType,
            Type implementationType)
        {
            if (interceptedType.IsInterface && implementationType.IsInterface
                && interceptedType.IsAssignableFrom(implementationType))
            {
                var methods = interceptedType.GetMethods();
                for (int i = 0; i < methods.Length; ++i)
                {
                    yield return new MethodImplementationInfo(methods[i], methods[i]);
                }
            }
            else
            {
                InterfaceMapping mapping = implementationType.GetInterfaceMap(interceptedType);
                for (int i = 0; i < mapping.InterfaceMethods.Length; ++i)
                {
                    yield return new MethodImplementationInfo(mapping.InterfaceMethods[i], mapping.TargetMethods[i]);
                }
            }

            foreach (var type in interceptedType.GetInterfaces())
            {
                foreach (var info in DoGetInterceptableMethods(type, implementationType))
                {
                    yield return info;
                }
            }
        }

        /// <summary>
        /// Create a proxy object that provides interception for <paramref name="target"/>.
        /// </summary>
        /// <param name="t">Type to generate the proxy of.</param>
        /// <param name="target">Object to create the proxy for.</param>
        /// <param name="additionalInterfaces">Additional interfaces the proxy must implement.</param>
        /// <returns>The proxy object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public IInterceptingProxy CreateProxy(Type t, object target, params Type[] additionalInterfaces)
        {
            Guard.ArgumentNotNull(t, "t");
            Guard.ArgumentNotNull(additionalInterfaces, "additionalInterfaces");

            Type interceptorType;
            Type typeToProxy = t;
            bool genericType = false;

            if (t.IsGenericType)
            {
                typeToProxy = t.GetGenericTypeDefinition();
                genericType = true;
            }

            GeneratedTypeKey key = new GeneratedTypeKey(typeToProxy, additionalInterfaces);
            lock (InterceptorClasses)
            {
                if (!InterceptorClasses.TryGetValue(key, out interceptorType))
                {
                    InterfaceInterceptorClassGenerator generator =
                        new InterfaceInterceptorClassGenerator(typeToProxy, additionalInterfaces);
                    interceptorType = generator.CreateProxyType();
                    InterceptorClasses[key] = interceptorType;
                }
            }

            if (genericType)
            {
                interceptorType = interceptorType.MakeGenericType(t.GetGenericArguments());
            }
            return (IInterceptingProxy)interceptorType.GetConstructors()[0].Invoke(new object[] { target, t });
        }

        #endregion
    }
}
