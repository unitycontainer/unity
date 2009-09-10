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
using System.Globalization;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A type based interceptor that works by generated a new class
    /// on the fly that derives from the target class.
    /// </summary>
    public class VirtualMethodInterceptor : ITypeInterceptor
    {
        private readonly Dictionary<GeneratedTypeKey, Type> derivedClasses =
            new Dictionary<GeneratedTypeKey, Type>(new GeneratedTypeKey.GeneratedTypeKeyComparer());


        /// <summary>
        /// Can this interceptor generate a proxy for the given type?
        /// </summary>
        /// <param name="t">Type to check.</param>
        /// <returns>True if interception is possible, false if not.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done via Guard class")]
        public bool CanIntercept(Type t)
        {
            Guard.ArgumentNotNull(t, "t");
            return t.IsClass &&
                (t.IsPublic || t.IsNestedPublic) &&
                t.IsVisible &&
                !t.IsSealed;
        }

        /// <summary>
        /// Returns a sequence of methods on the given type that can be
        /// intercepted.
        /// </summary>
        /// <param name="interceptedType">The intercepted type.</param>
        /// <param name="implementationType">The concrete type of the implementing object.</param>
        /// <returns>Sequence of <see cref="MethodInfo"/> objects.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Interceptable",
            Justification = "Spelling is fine")]
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done via Guard class")]
        public IEnumerable<MethodImplementationInfo> GetInterceptableMethods(Type interceptedType, Type implementationType)
        {
            Guard.ArgumentNotNull(implementationType, "implementationType");

            return DoGetInterceptableMethods(interceptedType, implementationType);
        }

        private IEnumerable<MethodImplementationInfo> DoGetInterceptableMethods(Type interceptedType, Type implementationType)
        {
            foreach (MethodInfo method in
                implementationType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (MethodOverride.MethodCanBeIntercepted(method))
                {
                    yield return new MethodImplementationInfo(null, method);
                }
            }
        }

        /// <summary>
        /// Create a type to proxy for the given type <paramref name="t"/>.
        /// </summary>
        /// <param name="t">Type to proxy.</param>
        /// <param name="additionalInterfaces">Additional interfaces the proxy must implement.</param>
        /// <returns>New type that can be instantiated instead of the
        /// original type t, and supports interception.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done via Guard class")]
        public Type CreateProxyType(Type t, params Type[] additionalInterfaces)
        {
            Guard.ArgumentNotNull(t, "t");
            Guard.ArgumentNotNull(additionalInterfaces, "additionalInterfaces");

            if (!CanIntercept(t))
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Resources.InterceptionNotSupported, t.Name));
            }

            Type interceptorType;
            Type typeToDerive = t;
            bool genericType = false;

            if (t.IsGenericType)
            {
                typeToDerive = t.GetGenericTypeDefinition();
                genericType = true;
            }

            GeneratedTypeKey key = new GeneratedTypeKey(typeToDerive, additionalInterfaces);
            lock (derivedClasses)
            {
                if (!derivedClasses.TryGetValue(key, out interceptorType))
                {
                    InterceptingClassGenerator generator =
                        new InterceptingClassGenerator(typeToDerive, additionalInterfaces);
                    interceptorType = generator.GenerateType();
                    derivedClasses[key] = interceptorType;
                }
            }

            if (genericType)
            {
                interceptorType = interceptorType.MakeGenericType(t.GetGenericArguments());
            }

            return interceptorType;
        }
    }
}
