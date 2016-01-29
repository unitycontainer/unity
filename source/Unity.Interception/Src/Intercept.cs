// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Practices.Unity.InterceptionExtension.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// High-level API for performing interception on existing and new objects.
    /// </summary>
    public static class Intercept
    {
        /// <summary>
        /// Returns a <see cref="IInterceptingProxy"/> for type <typeparamref name="T"/> which wraps
        /// the supplied <paramref name="target"/>.
        /// </summary>
        /// <typeparam name="T">The type to intercept.</typeparam>
        /// <param name="target">The instance to intercept.</param>
        /// <param name="interceptor">The <see cref="IInstanceInterceptor"/> to use when creating the proxy.</param>
        /// <param name="interceptionBehaviors">The interception behaviors for the new proxy.</param>
        /// <param name="additionalInterfaces">Any additional interfaces the proxy must implement.</param>
        /// <returns>A proxy for <paramref name="target"/> compatible with <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptor"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptionBehaviors"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="additionalInterfaces"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">when <paramref name="interceptor"/> cannot intercept
        /// <typeparamref name="T"/>.</exception>
        public static T ThroughProxyWithAdditionalInterfaces<T>(
            T target,
            IInstanceInterceptor interceptor,
            IEnumerable<IInterceptionBehavior> interceptionBehaviors,
            IEnumerable<Type> additionalInterfaces)
            where T : class
        {
            return (T)ThroughProxyWithAdditionalInterfaces(typeof(T), target, interceptor, interceptionBehaviors, additionalInterfaces);
        }

        /// <summary>
        /// Returns a <see cref="IInterceptingProxy"/> for type <typeparamref name="T"/> which wraps
        /// the supplied <paramref name="target"/>.
        /// </summary>
        /// <typeparam name="T">Type to intercept.</typeparam>
        /// <param name="target">The instance to intercept.</param>
        /// <param name="interceptor">The <see cref="IInstanceInterceptor"/> to use when creating the proxy.</param>
        /// <param name="interceptionBehaviors">The interception behaviors for the new proxy.</param>
        /// <returns>A proxy for <paramref name="target"/> compatible with <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptor"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptionBehaviors"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">when <paramref name="interceptor"/> cannot intercept
        /// <typeparamref name="T"/>.</exception>
        public static T ThroughProxy<T>(
            T target,
            IInstanceInterceptor interceptor,
            IEnumerable<IInterceptionBehavior> interceptionBehaviors)
            where T : class
        {
            return (T)ThroughProxyWithAdditionalInterfaces(typeof(T), target, interceptor, interceptionBehaviors, Type.EmptyTypes);
        }

        /// <summary>
        /// Returns a <see cref="IInterceptingProxy"/> for type <paramref name="interceptedType"/> which wraps
        /// the supplied <paramref name="target"/>.
        /// </summary>
        /// <param name="interceptedType">The type to intercept.</param>
        /// <param name="target">The instance to intercept.</param>
        /// <param name="interceptor">The <see cref="IInstanceInterceptor"/> to use when creating the proxy.</param>
        /// <param name="interceptionBehaviors">The interception behaviors for the new proxy.</param>
        /// <param name="additionalInterfaces">Any additional interfaces the proxy must implement.</param>
        /// <returns>A proxy for <paramref name="target"/> compatible with <paramref name="interceptedType"/>.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptedType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptor"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptionBehaviors"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="additionalInterfaces"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">when <paramref name="interceptor"/> cannot intercept
        /// <paramref name="interceptedType"/>.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public static object ThroughProxyWithAdditionalInterfaces(
            Type interceptedType,
            object target,
            IInstanceInterceptor interceptor,
            IEnumerable<IInterceptionBehavior> interceptionBehaviors,
            IEnumerable<Type> additionalInterfaces)
        {
            Guard.ArgumentNotNull(interceptedType, "interceptedType");
            Guard.ArgumentNotNull(target, "target");
            Guard.ArgumentNotNull(interceptor, "interceptor");
            Guard.ArgumentNotNull(interceptionBehaviors, "interceptionBehaviors");
            Guard.ArgumentNotNull(additionalInterfaces, "additionalInterfaces");

            if (!interceptor.CanIntercept(interceptedType))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.InterceptionNotSupported,
                        interceptedType.FullName),
                    nameof(interceptedType));
            }

            var behaviors = interceptionBehaviors.ToList();
            if (behaviors.Count(ib => ib == null) > 0)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Resources.NullBehavior),
                    nameof(interceptionBehaviors));
            }

            var activeBehaviors = behaviors.Where(ib => ib.WillExecute).ToList();

            var allAdditionalInterfaces
                = GetAllAdditionalInterfaces(activeBehaviors, additionalInterfaces).ToList();

            // If no behaviors and no extra interfaces, nothing to do.
            if (activeBehaviors.Count == 0 && allAdditionalInterfaces.Count == 0)
            {
                return target;
            }

            IInterceptingProxy proxy =
                interceptor.CreateProxy(interceptedType, target, allAdditionalInterfaces.ToArray());

            foreach (IInterceptionBehavior interceptionBehavior in activeBehaviors)
            {
                proxy.AddInterceptionBehavior(interceptionBehavior);
            }

            return proxy;
        }

        /// <summary>
        /// Returns a <see cref="IInterceptingProxy"/> for type <paramref name="interceptedType"/> which wraps
        /// the supplied <paramref name="target"/>.
        /// </summary>
        /// <param name="interceptedType">The type to intercept.</param>
        /// <param name="target">The instance to intercept.</param>
        /// <param name="interceptor">The <see cref="IInstanceInterceptor"/> to use when creating the proxy.</param>
        /// <param name="interceptionBehaviors">The interception behaviors for the new proxy.</param>
        /// <returns>A proxy for <paramref name="target"/> compatible with <paramref name="interceptedType"/>.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptedType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptor"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptionBehaviors"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">when <paramref name="interceptor"/> cannot intercept
        /// <paramref name="interceptedType"/>.</exception>
        public static object ThroughProxy(
            Type interceptedType,
            object target,
            IInstanceInterceptor interceptor,
            IEnumerable<IInterceptionBehavior> interceptionBehaviors)
        {
            return ThroughProxyWithAdditionalInterfaces(interceptedType, target, interceptor, interceptionBehaviors, Type.EmptyTypes);
        }

        /// <summary>
        /// Creates a new instance of type <typeparamref name="T"/> that is intercepted with the behaviors in
        /// <paramref name="interceptionBehaviors"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to create.</typeparam>
        /// <param name="interceptor">The <see cref="ITypeInterceptor"/> to use when creating the proxy.</param>
        /// <param name="interceptionBehaviors">The interception behaviors for the new proxy.</param>
        /// <param name="additionalInterfaces">Any additional interfaces the proxy must implement.</param>
        /// <param name="constructorParameters">The arguments for the creation of the new instance.</param>
        /// <returns>An instance of a class compatible with <typeparamref name="T"/> that includes execution of the
        /// given <paramref name="interceptionBehaviors"/>.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptor"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptionBehaviors"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="additionalInterfaces"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">when <paramref name="interceptor"/> cannot intercept
        /// <typeparamref name="T"/>.</exception>
        public static T NewInstanceWithAdditionalInterfaces<T>(
            ITypeInterceptor interceptor,
            IEnumerable<IInterceptionBehavior> interceptionBehaviors,
            IEnumerable<Type> additionalInterfaces,
            params object[] constructorParameters)
            where T : class
        {
            return (T)NewInstanceWithAdditionalInterfaces(typeof(T), interceptor, interceptionBehaviors, additionalInterfaces, constructorParameters);
        }

        /// <summary>
        /// Creates a new instance of type <typeparamref name="T"/> that is intercepted with the behaviors in
        /// <paramref name="interceptionBehaviors"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to create.</typeparam>
        /// <param name="interceptor">The <see cref="ITypeInterceptor"/> to use when creating the proxy.</param>
        /// <param name="interceptionBehaviors">The interception behaviors for the new proxy.</param>
        /// <param name="constructorParameters">The arguments for the creation of the new instance.</param>
        /// <returns>An instance of a class compatible with <typeparamref name="T"/> that includes execution of the
        /// given <paramref name="interceptionBehaviors"/>.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptor"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptionBehaviors"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">when <paramref name="interceptor"/> cannot intercept
        /// <typeparamref name="T"/>.</exception>
        public static T NewInstance<T>(
            ITypeInterceptor interceptor,
            IEnumerable<IInterceptionBehavior> interceptionBehaviors,
            params object[] constructorParameters)
            where T : class
        {
            return
                (T)NewInstanceWithAdditionalInterfaces(typeof(T), interceptor, interceptionBehaviors, Type.EmptyTypes, constructorParameters);
        }

        /// <summary>
        /// Creates a new instance of type <paramref name="type"/> that is intercepted with the behaviors in
        /// <paramref name="interceptionBehaviors"/>.
        /// </summary>
        /// <param name="type">The type of the object to create.</param>
        /// <param name="interceptor">The <see cref="ITypeInterceptor"/> to use when creating the proxy.</param>
        /// <param name="interceptionBehaviors">The interception behaviors for the new proxy.</param>
        /// <param name="additionalInterfaces">Any additional interfaces the instance must implement.</param>
        /// <param name="constructorParameters">The arguments for the creation of the new instance.</param>
        /// <returns>An instance of a class compatible with <paramref name="type"/> that includes execution of the
        /// given <paramref name="interceptionBehaviors"/>.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptor"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptionBehaviors"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="additionalInterfaces"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">when <paramref name="interceptor"/> cannot intercept
        /// <paramref name="type"/>.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public static object NewInstanceWithAdditionalInterfaces(
            Type type,
            ITypeInterceptor interceptor,
            IEnumerable<IInterceptionBehavior> interceptionBehaviors,
            IEnumerable<Type> additionalInterfaces,
            params object[] constructorParameters)
        {
            Guard.ArgumentNotNull(type, "type");
            Guard.ArgumentNotNull(interceptor, "interceptor");
            Guard.ArgumentNotNull(interceptionBehaviors, "interceptionBehaviors");
            Guard.ArgumentNotNull(additionalInterfaces, "additionalInterfaces");

            if (!interceptor.CanIntercept(type))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.InterceptionNotSupported,
                        type.FullName),
                    nameof(type));
            }

            var behaviors = interceptionBehaviors.ToList();

            if (behaviors.Count(ib => ib == null) > 0)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Resources.NullBehavior),
                    nameof(interceptionBehaviors));
            }

            Type implementationType = type;

            var activeBehaviors = behaviors.Where(ib => ib.WillExecute);

            Type[] allAdditionalInterfaces = GetAllAdditionalInterfaces(activeBehaviors, additionalInterfaces);

            Type interceptionType = interceptor.CreateProxyType(implementationType, allAdditionalInterfaces);

            var proxy = (IInterceptingProxy)Activator.CreateInstance(interceptionType, constructorParameters);

            foreach (IInterceptionBehavior interceptionBehavior in activeBehaviors)
            {
                proxy.AddInterceptionBehavior(interceptionBehavior);
            }

            return proxy;
        }

        /// <summary>
        /// Creates a new instance of type <paramref name="type"/> that is intercepted with the behaviors in
        /// <paramref name="interceptionBehaviors"/>.
        /// </summary>
        /// <param name="type">The type of the object to create.</param>
        /// <param name="interceptor">The <see cref="ITypeInterceptor"/> to use when creating the proxy.</param>
        /// <param name="interceptionBehaviors">The interception behaviors for the new proxy.</param>
        /// <param name="constructorParameters">The arguments for the creation of the new instance.</param>
        /// <returns>An instance of a class compatible with <paramref name="type"/> that includes execution of the
        /// given <paramref name="interceptionBehaviors"/>.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptor"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="interceptionBehaviors"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">when <paramref name="interceptor"/> cannot intercept
        /// <paramref name="type"/>.</exception>
        public static object NewInstance(
            Type type,
            ITypeInterceptor interceptor,
            IEnumerable<IInterceptionBehavior> interceptionBehaviors,
            params object[] constructorParameters)
        {
            return NewInstanceWithAdditionalInterfaces(type, interceptor, interceptionBehaviors, Type.EmptyTypes, constructorParameters);
        }

        /// <summary>
        /// Computes the array with all the additional interfaces for the interception of an object.
        /// </summary>
        /// <param name="interceptionBehaviors">The interception behaviors for the new proxy.</param>
        /// <param name="additionalInterfaces">Any additional interfaces the instance must implement.</param>
        /// <returns>An array with the required interfaces for </returns>
        /// <exception cref="ArgumentException">when the interfaces are not valid.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage",
            "CA2208:InstantiateArgumentExceptionsCorrectly",
            Justification = "Argument ok, confused by use within a lambda expression")]
        public static Type[] GetAllAdditionalInterfaces(
            IEnumerable<IInterceptionBehavior> interceptionBehaviors,
            IEnumerable<Type> additionalInterfaces)
        {
            var validatedRequiredInterfaces =
                interceptionBehaviors.SelectMany(ib =>
                    {
                        if (ib == null)
                        {
                            throw new ArgumentException(Resources.ExceptionContainsNullElement, nameof(interceptionBehaviors));
                        }
                        return
                            CheckInterfaces(
                                ib.GetRequiredInterfaces(),
                                "interceptionBehaviors",
                                error =>
                                    string.Format(
                                        CultureInfo.CurrentCulture,
                                        Resources.ExceptionRequiredInterfacesInvalid,
                                        error,
                                        ib.GetType().Name));
                    });
            var validatedAdditionalInterfaces =
                CheckInterfaces(
                    additionalInterfaces,
                    "additionalInterfaces",
                    error =>
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.ExceptionAdditionalInterfacesInvalid,
                            error));

            return validatedRequiredInterfaces.Concat(validatedAdditionalInterfaces).ToArray();
        }

        private static IEnumerable<Type> CheckInterfaces(
            IEnumerable<Type> interfaces,
            string argumentName,
            Func<string, string> messageFormatter)
        {
            if (interfaces == null)
            {
                throw new ArgumentException(messageFormatter(Resources.ExceptionNullInterfacesCollection));
            }

            return
                interfaces
                    .Select(type =>
                        {
                            if (type == null)
                            {
                                throw new ArgumentException(
                                    messageFormatter(Resources.ExceptionContainsNullElement),
                                    argumentName);
                            }
                            if (!type.IsInterface)
                            {
                                throw new ArgumentException(
                                    messageFormatter(
                                        string.Format(
                                            CultureInfo.CurrentCulture,
                                            Resources.ExceptionTypeIsNotInterface,
                                            type.Name)),
                                    argumentName);
                            }
                            if (type.IsGenericTypeDefinition)
                            {
                                throw new ArgumentException(
                                    messageFormatter(
                                        string.Format(
                                            CultureInfo.CurrentCulture,
                                            Resources.ExceptionTypeIsOpenGeneric,
                                            type.Name)),
                                    argumentName);
                            }
                            return type;
                        });
        }
    }
}
