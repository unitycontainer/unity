// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the
    /// <see cref="IUnityContainer"/> interface.
    /// </summary>
    public static class UnityContainerExtensions
    {
        #region RegisterType overloads

        #region Generics overloads

        /// <summary>
        /// Register a type with specific members to be injected.
        /// </summary>
        /// <typeparam name="T">Type this registration is for.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static IUnityContainer RegisterType<T>(this IUnityContainer container, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(null, typeof(T), null, null, injectionMembers);
        }

        /// <summary>
        /// Register a type mapping with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is used to tell the container that when asked for type <typeparamref name="TFrom"/>,
        /// actually return an instance of type <typeparamref name="TTo"/>. This is very useful for
        /// getting instances of interfaces.
        /// </para>
        /// <para>
        /// This overload registers a default mapping and transient lifetime.
        /// </para>
        /// </remarks>
        /// <typeparam name="TFrom"><see cref="Type"/> that will be requested.</typeparam>
        /// <typeparam name="TTo"><see cref="Type"/> that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static IUnityContainer RegisterType<TFrom, TTo>(this IUnityContainer container, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(typeof(TFrom), typeof(TTo), null, null, injectionMembers);
        }

        /// <summary>
        /// Register a type mapping with the container, where the created instances will use
        /// the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <typeparam name="TFrom"><see cref="Type"/> that will be requested.</typeparam>
        /// <typeparam name="TTo"><see cref="Type"/> that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static IUnityContainer RegisterType<TFrom, TTo>(this IUnityContainer container, LifetimeManager lifetimeManager,
                                                               params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(typeof(TFrom), typeof(TTo), null, lifetimeManager, injectionMembers);
        }

        /// <summary>
        /// Register a type mapping with the container.
        /// </summary>
        /// <remarks>
        /// This method is used to tell the container that when asked for type <typeparamref name="TFrom"/>,
        /// actually return an instance of type <typeparamref name="TTo"/>. This is very useful for
        /// getting instances of interfaces.
        /// </remarks>
        /// <typeparam name="TFrom"><see cref="Type"/> that will be requested.</typeparam>
        /// <typeparam name="TTo"><see cref="Type"/> that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name of this mapping.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static IUnityContainer RegisterType<TFrom, TTo>(this IUnityContainer container, string name, params InjectionMember[] injectionMembers)
            where TTo : TFrom
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(typeof(TFrom), typeof(TTo), name, null, injectionMembers);
        }

        /// <summary>
        /// Register a type mapping with the container, where the created instances will use
        /// the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <typeparam name="TFrom"><see cref="Type"/> that will be requested.</typeparam>
        /// <typeparam name="TTo"><see cref="Type"/> that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name to use for registration, null if a default registration.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static IUnityContainer RegisterType<TFrom, TTo>(this IUnityContainer container, string name, LifetimeManager lifetimeManager,
                                                               params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(typeof(TFrom), typeof(TTo), name, lifetimeManager, injectionMembers);
        }

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <typeparam name="T">The type to apply the <paramref name="lifetimeManager"/> to.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static IUnityContainer RegisterType<T>(this IUnityContainer container, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(null, typeof(T), null, lifetimeManager, injectionMembers);
        }

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <typeparam name="T">The type to configure injection on.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name that will be used to request the type.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static IUnityContainer RegisterType<T>(this IUnityContainer container, string name, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(null, typeof(T), name, null, injectionMembers);
        }

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type and name with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <typeparam name="T">The type to apply the <paramref name="lifetimeManager"/> to.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name that will be used to request the type.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static IUnityContainer RegisterType<T>(this IUnityContainer container, string name, LifetimeManager lifetimeManager,
                                                      params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(null, typeof(T), name, lifetimeManager, injectionMembers);
        }

        #endregion

        #region Non-generics overloads

        /// <summary>
        /// Register a type with specific members to be injected.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="t">Type this registration is for.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t", Justification = "Parameter name is meaningful enough in context")]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type t, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(null, t, null, null, injectionMembers);
        }

        /// <summary>
        /// Register a type mapping with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is used to tell the container that when asked for type <paramref name="from"/>,
        /// actually return an instance of type <paramref name="to"/>. This is very useful for
        /// getting instances of interfaces.
        /// </para>
        /// <para>
        /// This overload registers a default mapping.
        /// </para>
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="from"><see cref="Type"/> that will be requested.</param>
        /// <param name="to"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "To",
            Justification = "Identifier name 'to' makes sense. Avoid changing public API names.")]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type from, Type to, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(from, to, null, null, injectionMembers);
        }

        /// <summary>
        /// Register a type mapping with the container.
        /// </summary>
        /// <remarks>
        /// This method is used to tell the container that when asked for type <paramref name="from"/>,
        /// actually return an instance of type <paramref name="to"/>. This is very useful for
        /// getting instances of interfaces.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="from"><see cref="Type"/> that will be requested.</param>
        /// <param name="to"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="name">Name to use for registration, null if a default registration.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "To",
            Justification = "Identifier name 'to' makes sense. Avoid changing public API names.")]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type from, Type to, string name, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(from, to, name, null, injectionMembers);
        }

        /// <summary>
        /// Register a type mapping with the container, where the created instances will use
        /// the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="from"><see cref="Type"/> that will be requested.</param>
        /// <param name="to"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "To",
            Justification = "Identifier name 'to' makes sense. Avoid changing public API names.")]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type from, Type to, LifetimeManager lifetimeManager,
                                                   params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(from, to, null, lifetimeManager, injectionMembers);
        }

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type and name with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="t">The <see cref="Type"/> to apply the <paramref name="lifetimeManager"/> to.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t", Justification = "Parameter name is meaningful enough in context")]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type t, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(null, t, null, lifetimeManager, injectionMembers);
        }

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type and name with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="t">The <see cref="Type"/> to configure in the container.</param>
        /// <param name="name">Name to use for registration, null if a default registration.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t", Justification = "Parameter name is meaningful enough in context")]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type t, string name, params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(null, t, name, null, injectionMembers);
        }

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type and name with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="t">The <see cref="Type"/> to apply the <paramref name="lifetimeManager"/> to.</param>
        /// <param name="name">Name to use for registration, null if a default registration.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t", Justification = "Parameter name is meaningful enough in context")]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type t, string name, LifetimeManager lifetimeManager,
                                                   params InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(null, t, name, lifetimeManager, injectionMembers);
        }

        #endregion

        #endregion

        #region RegisterInstance overloads

        #region Generics overloads

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// <para>
        /// This overload does a default registration and has the container take over the lifetime of the instance.</para>
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="instance">Object to returned.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, TInterface instance)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterInstance(typeof(TInterface), null, instance, CreateDefaultInstanceLifetimeManager());
        }

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// <para>
        /// This overload does a default registration (name = null).
        /// </para>
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="instance">Object to returned.</param>
        /// <param name="lifetimeManager">
        /// <see cref="LifetimeManager"/> object that controls how this instance will be managed by the container.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, TInterface instance, LifetimeManager lifetimeManager)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterInstance(typeof(TInterface), null, instance, lifetimeManager);
        }

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// <para>
        /// This overload automatically has the container take ownership of the <paramref name="instance"/>.</para>
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="instance">Object to returned.</param>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name for registration.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, string name, TInterface instance)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterInstance(typeof(TInterface), name, instance, CreateDefaultInstanceLifetimeManager());
        }

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="instance">Object to returned.</param>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name for registration.</param>
        /// <param name="lifetimeManager">
        /// <see cref="LifetimeManager"/> object that controls how this instance will be managed by the container.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, string name, TInterface instance, LifetimeManager lifetimeManager)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterInstance(typeof(TInterface), name, instance, lifetimeManager);
        }

        #endregion

        #region Non-generic overloads

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// <para>
        /// This overload does a default registration and has the container take over the lifetime of the instance.</para>
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="t">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to returned.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t", Justification = "Parameter name is meaningful enough in context")]
        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type t, object instance)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterInstance(t, null, instance, CreateDefaultInstanceLifetimeManager());
        }

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// <para>
        /// This overload does a default registration (name = null).
        /// </para>
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="t">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to returned.</param>
        /// <param name="lifetimeManager">
        /// <see cref="LifetimeManager"/> object that controls how this instance will be managed by the container.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t", Justification = "Parameter name is meaningful enough in context")]
        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type t, object instance, LifetimeManager lifetimeManager)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterInstance(t, null, instance, lifetimeManager);
        }

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// <para>
        /// This overload automatically has the container take ownership of the <paramref name="instance"/>.</para>
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="t">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to returned.</param>
        /// <param name="name">Name for registration.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t", Justification = "Parameter name is meaningful enough in context")]
        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type t, string name, object instance)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterInstance(t, name, instance, CreateDefaultInstanceLifetimeManager());
        }

        #endregion

        #endregion

        #region Resolve overloads

        /// <summary>
        /// Resolve an instance of the default requested type from the container.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of object to get from the container.</typeparam>
        /// <param name="container">Container to resolve from.</param>
        /// <param name="overrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        public static T Resolve<T>(this IUnityContainer container, params ResolverOverride[] overrides)
        {
            Guard.ArgumentNotNull(container, "container");
            return (T)container.Resolve(typeof(T), null, overrides);
        }

        /// <summary>
        /// Resolve an instance of the requested type with the given name from the container.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of object to get from the container.</typeparam>
        /// <param name="container">Container to resolve from.</param>
        /// <param name="name">Name of the object to retrieve.</param>
        /// <param name="overrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        public static T Resolve<T>(this IUnityContainer container, string name, params ResolverOverride[] overrides)
        {
            Guard.ArgumentNotNull(container, "container");
            return (T)container.Resolve(typeof(T), name, overrides);
        }

        /// <summary>
        /// Resolve an instance of the default requested type from the container.
        /// </summary>
        /// <param name="container">Container to resolve from.</param>
        /// <param name="t"><see cref="Type"/> of object to get from the container.</param>
        /// <param name="overrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t", Justification = "Parameter name is meaningful enough in context")]
        public static object Resolve(this IUnityContainer container, Type t, params ResolverOverride[] overrides)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.Resolve(t, null, overrides);
        }

        #endregion

        #region ResolveAll overloads

        /// <summary>
        /// Return instances of all registered types requested.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful if you've registered multiple types with the same
        /// <see cref="Type"/> but different names.
        /// </para>
        /// <para>
        /// Be aware that this method does NOT return an instance for the default (unnamed) registration.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The type requested.</typeparam>
        /// <param name="container">Container to resolve from.</param>
        /// <param name="resolverOverrides">Any overrides for the resolve calls.</param>
        /// <returns>Set of objects of type <typeparamref name="T"/>.</returns>
        public static IEnumerable<T> ResolveAll<T>(this IUnityContainer container, params ResolverOverride[] resolverOverrides)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.ResolveAll(typeof(T), resolverOverrides).Cast<T>();
        }

        #endregion

        #region BuildUp overloads

        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful when you don't control the construction of an
        /// instance (ASP.NET pages or objects created via XAML, for instance)
        /// but you still want properties and other injection performed.
        /// </para>
        /// <para>
        /// This overload uses the default registrations.
        /// </para>
        /// </remarks>
        /// <typeparam name="T"><see cref="Type"/> of object to perform injection on.</typeparam>
        /// <param name="container">Container to resolve through.</param>
        /// <param name="existing">Instance to build up.</param>
        /// <param name="resolverOverrides">Any overrides for the buildup.</param>
        /// <returns>The resulting object. By default, this will be <paramref name="existing"/>, but
        /// container extensions may add things like automatic proxy creation which would
        /// cause this to return a different object (but still type compatible with <typeparamref name="T"/>).</returns>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BuildUp", Justification = "BuildUp is correct.")]
        public static T BuildUp<T>(this IUnityContainer container, T existing, params ResolverOverride[] resolverOverrides)
        {
            Guard.ArgumentNotNull(container, "container");
            return (T)container.BuildUp(typeof(T), existing, null, resolverOverrides);
        }

        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful when you don't control the construction of an
        /// instance (ASP.NET pages or objects created via XAML, for instance)
        /// but you still want properties and other injection performed.
        /// </para></remarks>
        /// <typeparam name="T"><see cref="Type"/> of object to perform injection on.</typeparam>
        /// <param name="container">Container to resolve through.</param>
        /// <param name="existing">Instance to build up.</param>
        /// <param name="name">name to use when looking up the typemappings and other configurations.</param>
        /// <param name="resolverOverrides">Any overrides for the Buildup.</param>
        /// <returns>The resulting object. By default, this will be <paramref name="existing"/>, but
        /// container extensions may add things like automatic proxy creation which would
        /// cause this to return a different object (but still type compatible with <typeparamref name="T"/>).</returns>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BuildUp", Justification = "BuildUp is correct.")]
        public static T BuildUp<T>(this IUnityContainer container, T existing, string name, params ResolverOverride[] resolverOverrides)
        {
            Guard.ArgumentNotNull(container, "container");
            return (T)container.BuildUp(typeof(T), existing, name, resolverOverrides);
        }

        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful when you don't control the construction of an
        /// instance (ASP.NET pages or objects created via XAML, for instance)
        /// but you still want properties and other injection performed.
        /// </para>
        /// <para>
        /// This overload uses the default registrations.
        /// </para>
        /// </remarks>
        /// <param name="container">Container to resolve through.</param>
        /// <param name="t"><see cref="Type"/> of object to perform injection on.</param>
        /// <param name="existing">Instance to build up.</param>
        /// <param name="resolverOverrides">Any overrides for the Buildup.</param>
        /// <returns>The resulting object. By default, this will be <paramref name="existing"/>, but
        /// container extensions may add things like automatic proxy creation which would
        /// cause this to return a different object (but still type compatible with <paramref name="t"/>).</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t", Justification = "Parameter name is meaningful enough in context")]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BuildUp",
            Justification = "Backwards compatibility with ObjectBuilder")]
        public static object BuildUp(this IUnityContainer container, Type t, object existing, params ResolverOverride[] resolverOverrides)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.BuildUp(t, existing, null, resolverOverrides);
        }

        #endregion

        #region Extension management and configuration

        /// <summary>
        /// Creates a new extension object and adds it to the container.
        /// </summary>
        /// <typeparam name="TExtension">Type of <see cref="UnityContainerExtension"/> to add. The extension type
        /// will be resolved from within the supplied <paramref name="container"/>.</typeparam>
        /// <param name="container">Container to add the extension to.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static IUnityContainer AddNewExtension<TExtension>(this IUnityContainer container)
            where TExtension : UnityContainerExtension
        {
            Guard.ArgumentNotNull(container, "container");
            TExtension newExtension = container.Resolve<TExtension>();
            return container.AddExtension(newExtension);
        }

        /// <summary>
        /// Resolve access to a configuration interface exposed by an extension.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <typeparam name="TConfigurator">The configuration interface required.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Configurator",
            Justification = "Configurator IS spelled correctly")]
        public static TConfigurator Configure<TConfigurator>(this IUnityContainer container)
            where TConfigurator : IUnityContainerExtensionConfigurator
        {
            Guard.ArgumentNotNull(container, "container");
            return (TConfigurator)container.Configure(typeof(TConfigurator));
        }

        #endregion

        #region Introspection Helpers

        /// <summary>
        /// Check if a particular type has been registered with the container with
        /// the default name.
        /// </summary>
        /// <param name="container">Container to inspect.</param>
        /// <param name="typeToCheck">Type to check registration for.</param>
        /// <returns>True if this type has been registered, false if not.</returns>
        public static bool IsRegistered(this IUnityContainer container, Type typeToCheck)
        {
            Guard.ArgumentNotNull(container, "container");
            Guard.ArgumentNotNull(typeToCheck, "typeToCheck");
            return container.IsRegistered(typeToCheck, null);
        }

        /// <summary>
        /// Check if a particular type/name pair has been registered with the container.
        /// </summary>
        /// <param name="container">Container to inspect.</param>
        /// <param name="typeToCheck">Type to check registration for.</param>
        /// <param name="nameToCheck">Name to check registration for.</param>
        /// <returns>True if this type/name pair has been registered, false if not.</returns>
        public static bool IsRegistered(this IUnityContainer container, Type typeToCheck, string nameToCheck)
        {
            Guard.ArgumentNotNull(container, "container");
            Guard.ArgumentNotNull(typeToCheck, "typeToCheck");

            var registration = from r in container.Registrations
                               where r.RegisteredType == typeToCheck && r.Name == nameToCheck
                               select r;
            return registration.FirstOrDefault() != null;
        }

        /// <summary>
        /// Check if a particular type has been registered with the container with the default name.
        /// </summary>
        /// <typeparam name="T">Type to check registration for.</typeparam>
        /// <param name="container">Container to inspect.</param>
        /// <returns>True if this type has been registered, false if not.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static bool IsRegistered<T>(this IUnityContainer container)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.IsRegistered(typeof(T));
        }

        /// <summary>
        /// Check if a particular type/name pair has been registered with the container.
        /// </summary>
        /// <typeparam name="T">Type to check registration for.</typeparam>
        /// <param name="container">Container to inspect.</param>
        /// <param name="nameToCheck">Name to check registration for.</param>
        /// <returns>True if this type/name pair has been registered, false if not.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static bool IsRegistered<T>(this IUnityContainer container, string nameToCheck)
        {
            Guard.ArgumentNotNull(container, "container");
            return container.IsRegistered(typeof(T), nameToCheck);
        }

        #endregion

        #region Helper methods
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope",
            Justification = "Factory method to create a disposable but is not expected to owns its lifetime.")]
        private static LifetimeManager CreateDefaultInstanceLifetimeManager()
        {
            return new ContainerControlledLifetimeManager();
        }

        #endregion
    }
}
