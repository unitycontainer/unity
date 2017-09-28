// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ObjectBuilder2;
using Unity.ObjectBuilder;
using Unity.Properties;
using Guard = Unity.Utility.Guard;

namespace Unity
{
    /// <summary>
    /// A simple, extensible dependency injection container.
    /// </summary>
    public partial class UnityContainer : IUnityContainer
    {
        #region Registrations

        /// <summary>
        /// RegisterType a type mapping with the container, where the created instances will use
        /// the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <param name="from"><see cref="Type"/> that will be requested.</param>
        /// <param name="to"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="name">Name to use for registration, null if a default registration.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public IUnityContainer RegisterType(Type from, Type to, string name, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
        {
            Guard.ArgumentNotNull(to, "to");
            Guard.ArgumentNotNull(injectionMembers, "injectionMembers");

            if (string.IsNullOrEmpty(name))
            {
                name = null;
            }

            if (from != null && !from.GetTypeInfo().IsGenericType && !to.GetTypeInfo().IsGenericType)
            {
                Guard.TypeIsAssignable(from, to, "from");
            }

            Registering(this, new RegisterEventArgs(from, to, name, lifetimeManager));

            if (injectionMembers.Length > 0)
            {
                ClearExistingBuildPlan(to, name);
                foreach (var member in injectionMembers)
                {
                    member.AddPolicies(from, to, name, policies);
                }
            }
            return this;
        }

        /// <summary>
        /// RegisterType an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// </remarks>
        /// <param name="t">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to returned.</param>
        /// <param name="name">Name for registration.</param>
        /// <param name="lifetime">
        /// <para>If true, the container will take over the lifetime of the instance,
        /// calling Dispose on it (if it's <see cref="IDisposable"/>) when the container is Disposed.</para>
        /// <para>
        ///  If false, container will not maintain a strong reference to <paramref name="instance"/>. User is responsible
        /// for disposing instance, and for keeping the instance from being garbage collected.</para></param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public IUnityContainer RegisterInstance(Type t, string name, object instance, LifetimeManager lifetime)
        {
            Guard.ArgumentNotNull(instance, "instance");
            Guard.ArgumentNotNull(lifetime, "lifetime");
            Guard.InstanceIsAssignable(t, instance, "instance");
            RegisteringInstance(this,
                                new RegisterInstanceEventArgs(t,
                                                              instance,
                                                              name,
                                                              lifetime));
            return this;
        }

        #endregion


        #region Getting objects

        /// <summary>
        /// Get an instance of the requested type with the given name from the container.
        /// </summary>
        /// <param name="t"><see cref="Type"/> of object to get from the container.</param>
        /// <param name="name">Name of the object to retrieve.</param>
        /// <param name="resolverOverrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        public object Resolve(Type t, string name, params ResolverOverride[] resolverOverrides)
        {
            return DoBuildUp(t, null, name, resolverOverrides);
        }

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
        /// <param name="t">The type requested.</param>
        /// <param name="resolverOverrides">Any overrides for the resolve calls.</param>
        /// <returns>Set of objects of type <paramref name="t"/>.</returns>
        public IEnumerable<object> ResolveAll(Type t, params ResolverOverride[] resolverOverrides)
        {
            Guard.ArgumentNotNull(t, "t");

            var result = this.Resolve(t.MakeArrayType(), resolverOverrides);
            return result is IEnumerable<object> 
                ? (IEnumerable<object>) result 
                : ((Array)result).Cast<object>();
        }

        #endregion


        #region BuildUp existing object

        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful when you don't control the construction of an
        /// instance (ASP.NET pages or objects created via XAML, for instance)
        /// but you still want properties and other injection performed.
        /// </para></remarks>
        /// <param name="t"><see cref="Type"/> of object to perform injection on.</param>
        /// <param name="existing">Instance to build up.</param>
        /// <param name="name">name to use when looking up the typemappings and other configurations.</param>
        /// <param name="resolverOverrides">Any overrides for the buildup.</param>
        /// <returns>The resulting object. By default, this will be <paramref name="existing"/>, but
        /// container extensions may add things like automatic proxy creation which would
        /// cause this to return a different object (but still type compatible with <paramref name="t"/>).</returns>
        public object BuildUp(Type t, object existing, string name, params ResolverOverride[] resolverOverrides)
        {
            Guard.ArgumentNotNull(existing, "existing");
            Guard.InstanceIsAssignable(t, existing, "existing");
            return DoBuildUp(t, existing, name, resolverOverrides);
        }

        /// <summary>
        /// Run an existing object through the container, and clean it up.
        /// </summary>
        /// <param name="o">The object to tear down.</param>
        public void Teardown(object o)
        {
            IBuilderContext context = null;

            try
            {
                Guard.ArgumentNotNull(o, "o");

                context =
                    new BuilderContext(this, GetStrategies().Reverse(), lifetimeContainer, policies, null, o);
                context.Strategies.ExecuteTearDown(context);
            }
            catch (Exception ex)
            {
                throw new ResolutionFailedException(o.GetType(), null, ex, context);
            }
        }

        #endregion


        #region Extension Management

        /// <summary>
        /// Add an extension object to the container.
        /// </summary>
        /// <param name="extension"><see cref="UnityContainerExtension"/> to add.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public IUnityContainer AddExtension(UnityContainerExtension extension)
        {
            Unity.Utility.Guard.ArgumentNotNull(extensions, "extensions");

            extensions.Add(extension);
            extension.InitializeExtension(new ExtensionContextImpl(this));
            lock (cachedStrategiesLock)
            {
                cachedStrategies = null;
            }
            return this;
        }

        /// <summary>
        /// Get access to a configuration interface exposed by an extension.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <param name="configurationInterface"><see cref="Type"/> of configuration interface required.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        public object Configure(Type configurationInterface)
        {
            return extensions.Where(ex => configurationInterface.GetTypeInfo().IsAssignableFrom(ex.GetType().GetTypeInfo())).FirstOrDefault();
        }

        /// <summary>
        /// Remove all installed extensions from this container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method removes all extensions from the container, including the default ones
        /// that implement the out-of-the-box behavior. After this method, if you want to use
        /// the container again you will need to either read the default extensions or replace
        /// them with your own.
        /// </para>
        /// <para>
        /// The registered instances and singletons that have already been set up in this container
        /// do not get removed.
        /// </para>
        /// </remarks>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public IUnityContainer RemoveAllExtensions()
        {
            var toRemove = new List<UnityContainerExtension>(extensions);
            toRemove.Reverse();
            foreach (UnityContainerExtension extension in toRemove)
            {
                extension.Remove();
                var disposable = extension as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            extensions.Clear();

            // Reset our policies, strategies, and registered names to reset to "zero"
            strategies.Clear();
            policies.ClearAll();
            registeredNames.Clear();

            // Restore defaults
            Registering = OnRegister;
            RegisteringInstance = OnRegisterInstance;
            InitializeDefaultPolicies();

            return this;
        }

        #endregion


        #region Hierarchy management

        /// <summary>
        /// The parent of this container.
        /// </summary>
        /// <value>The parent container, or null if this container doesn't have one.</value>
        public IUnityContainer Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Create a child container.
        /// </summary>
        /// <remarks>
        /// A child container shares the parent's configuration, but can be configured with different
        /// settings or lifetime.</remarks>
        /// <returns>The new child container.</returns>
        public IUnityContainer CreateChildContainer()
        {
            var child = new UnityContainer(this);

            var childContext = new ExtensionContextImpl(child);
            ChildContainerCreated(this, new ChildContainerCreatedEventArgs(childContext));
            return child;
        }

        #endregion

        
        /// <summary>
        /// Get a sequence of <see cref="ContainerRegistration"/> that describe the current state
        /// of the container.
        /// </summary>
        public IEnumerable<ContainerRegistration> Registrations
        {
            get
            {
                var allRegisteredNames = new Dictionary<Type, List<string>>();
                FillTypeRegistrationDictionary(allRegisteredNames);

                return
                    from type in allRegisteredNames.Keys
                    from name in allRegisteredNames[type]
                    select new ContainerRegistration(type, name, policies);
            }
        }
    }
}
