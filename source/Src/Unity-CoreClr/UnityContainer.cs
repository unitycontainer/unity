// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ObjectBuilder2;
using Unity.ObjectBuilder;
using Unity.Properties;
using Guard = Unity.Utility.Guard;

namespace Unity
{
    /// <summary>
    /// A simple, extensible dependency injection container.
    /// </summary>
    public class UnityContainer : IUnityContainer
    {
        private readonly UnityContainer parent;

        private LifetimeContainer lifetimeContainer;
        private StagedStrategyChain<UnityBuildStage> strategies;
        private StagedStrategyChain<UnityBuildStage> buildPlanStrategies;
        private PolicyList policies;
        private NamedTypesRegistry registeredNames;
        private List<UnityContainerExtension> extensions;

        private IStrategyChain cachedStrategies;
        private object cachedStrategiesLock;

        private event EventHandler<RegisterEventArgs> Registering;
        private event EventHandler<RegisterInstanceEventArgs> RegisteringInstance;
        private event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated;

        private IServiceProvider alternativeServiceProvider;

        /// <summary>
        /// Create a default <see cref="UnityContainer"/>.
        /// </summary>
        public UnityContainer()
            : this(null)
        {
            // Only a root container (one without a parent) gets the default strategies.
            this.AddExtension(new UnityDefaultStrategiesExtension());
        }

        /// <summary>
        /// Create a <see cref="UnityContainer"/> with the given parent container.
        /// </summary>
        /// <param name="parent">The parent <see cref="UnityContainer"/>. The current object
        /// will apply its own settings first, and then check the parent for additional ones.</param>
        private UnityContainer(UnityContainer parent)
        {
            this.parent = parent;

            if (parent != null)
            {
                parent.lifetimeContainer.Add(this);
            }

            InitializeBuilderState();
            // Put a noop at the beginning of each of our events so we don't have to worry
            // about nulls
            Registering += delegate { };
            RegisteringInstance += delegate { };
            ChildContainerCreated += delegate { };

            // Every container gets the default behavior
            this.AddExtension(new UnityDefaultBehaviorExtension());

#pragma warning disable 618
            this.AddExtension(new InjectedMembers());
#pragma warning restore 618
        }

        #region Type Mapping

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

        #endregion

        #region Instance Registration

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
            try
            {
                return DoBuildUp(t, name, resolverOverrides);
            }
            catch(Exception ex)
            {
                // Ariel. No usar el catch para esta logica

                //if (t.Name == "IDataProtectionProvider") System.Diagnostics.Debugger.Break();
                //if (t.Name == "IApplicationDiscriminator") System.Diagnostics.Debugger.Break();
                //if (t.Name == "IXmlRepository") System.Diagnostics.Debugger.Break();

                //if (t.Name == "IActionInvokerFactory") System.Diagnostics.Debugger.Break();

                //if (t.Name == "ViewResultExecutor") System.Diagnostics.Debugger.Break();

                if (alternativeServiceProvider != null)
                {
                    return alternativeServiceProvider.GetService(t);
                }
                else
                {
                    throw;
                }
            }
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

            return (IEnumerable<object>)this.Resolve(t.MakeArrayType(), resolverOverrides);
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
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Guard class is doing validation")]
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
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class")]
        public void Teardown(object o)
        {
            IBuilderContext context = null;

            try
            {
                Guard.ArgumentNotNull(o, "o");

                context =
                    new BuilderContext(GetStrategies().Reverse(), lifetimeContainer, policies, null, o);
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
        /// Implementation of the ExtensionContext that is actually used
        /// by the UnityContainer implementation.
        /// </summary>
        /// <remarks>
        /// This is a nested class so that it can access state in the
        /// container that would otherwise be inaccessible.
        /// </remarks>
        private class ExtensionContextImpl : ExtensionContext
        {
            private readonly UnityContainer container;

            public ExtensionContextImpl(UnityContainer container)
            {
                this.container = container;
            }

            public override IUnityContainer Container
            {
                get { return this.container; }
            }

            public override StagedStrategyChain<UnityBuildStage> Strategies
            {
                get { return this.container.strategies; }
            }

            public override StagedStrategyChain<UnityBuildStage> BuildPlanStrategies
            {
                get { return this.container.buildPlanStrategies; }
            }

            public override IPolicyList Policies
            {
                get { return this.container.policies; }
            }

            public override ILifetimeContainer Lifetime
            {
                get { return this.container.lifetimeContainer; }
            }

            public override void RegisterNamedType(Type t, string name)
            {
                this.container.registeredNames.RegisterType(t, name);
            }

            public override event EventHandler<RegisterEventArgs> Registering
            {
                add { this.container.Registering += value; }
                remove { this.container.Registering -= value; }
            }

            /// <summary>
            /// This event is raised when the <see cref="UnityContainer.RegisterInstance(Type,string,object,LifetimeManager)"/> method,
            /// or one of its overloads, is called.
            /// </summary>
            public override event EventHandler<RegisterInstanceEventArgs> RegisteringInstance
            {
                add { this.container.RegisteringInstance += value; }
                remove { this.container.RegisteringInstance -= value; }
            }

            public override event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated
            {
                add { this.container.ChildContainerCreated += value; }
                remove { this.container.ChildContainerCreated -= value; }
            }
        }

        /// <summary>
        /// Add an extension object to the container.
        /// </summary>
        /// <param name="extension"><see cref="UnityContainerExtension"/> to add.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
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

            return this;
        }

        #endregion

        #region Child container management

        /// <summary>
        /// Create a child container.
        /// </summary>
        /// <remarks>
        /// A child container shares the parent's configuration, but can be configured with different
        /// settings or lifetime.</remarks>
        /// <returns>The new child container.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope",
            Justification = "Factory method that creates disposable object but does not own its lifetime.")]
        public IUnityContainer CreateChildContainer()
        {
            var child = new UnityContainer(this);

            // Ariel
            if (this.AlternativeServiceProvider != null)
            {
                child.AlternativeServiceProvider = this.AlternativeServiceProvider;
            }

            var childContext = new ExtensionContextImpl(child);
            ChildContainerCreated(this, new ChildContainerCreatedEventArgs(childContext));
            return child;
        }

        /// <summary>
        /// The parent of this container.
        /// </summary>
        /// <value>The parent container, or null if this container doesn't have one.</value>
        public IUnityContainer Parent
        {
            get { return parent; }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Dispose this container instance.
        /// </summary>
        /// <remarks>
        /// Disposing the container also disposes any child containers,
        /// and disposes any instances whose lifetimes are managed
        /// by the container.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Shut FxCop up
        }

        /// <summary>
        /// Dispose this container instance.
        /// </summary>
        /// <remarks>
        /// This class doesn't have a finalizer, so <paramref name="disposing"/> will always be true.</remarks>
        /// <param name="disposing">True if being called from the IDisposable.Dispose
        /// method, false if being called from a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (lifetimeContainer != null)
                {
                    lifetimeContainer.Dispose();
                    lifetimeContainer = null;

                    if (parent != null && parent.lifetimeContainer != null)
                    {
                        parent.lifetimeContainer.Remove(this);
                    }
                }

                extensions.OfType<IDisposable>().ForEach(ex => ex.Dispose());
                extensions.Clear();
            }
        }

        #endregion

        #region Running ObjectBuilder

        private object DoBuildUp(Type t, string name, IEnumerable<ResolverOverride> resolverOverrides)
        {
            return DoBuildUp(t, null, name, resolverOverrides);
        }

        private object DoBuildUp(Type t, object existing, string name, IEnumerable<ResolverOverride> resolverOverrides)
        {
            IBuilderContext context = null;

            // Ariel
            //if (t.Name == "IDataProtectionProvider") System.Diagnostics.Debugger.Break();
            if (t.Name == "IApplicationDiscriminator") System.Diagnostics.Debugger.Break();

            //if (t.Name == "IActionInvokerFactory") System.Diagnostics.Debugger.Break();

            try
            {
                context =
                    new BuilderContext(
                        GetStrategies(),
                        lifetimeContainer,
                        policies,
                        new NamedTypeBuildKey(t, name),
                        existing);
                context.AddResolverOverrides(resolverOverrides);

                if (t.GetTypeInfo().IsGenericTypeDefinition)
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.CurrentCulture,
                        Resources.CannotResolveOpenGenericType,
                        t.FullName), "t");
                }

                return context.Strategies.ExecuteBuildUp(context);
            }
            catch (Exception ex)
            {
                throw new ResolutionFailedException(t, name, ex, context);
            }
        }

        private IStrategyChain GetStrategies()
        {
            IStrategyChain buildStrategies = cachedStrategies;
            if (buildStrategies == null)
            {
                lock (cachedStrategiesLock)
                {
                    if (cachedStrategies == null)
                    {
                        buildStrategies = strategies.MakeStrategyChain();
                        cachedStrategies = buildStrategies;
                    }
                    else
                    {
                        buildStrategies = cachedStrategies;
                    }
                }
            }
            return buildStrategies;
        }

        #endregion

        #region ObjectBuilder initialization

        private void InitializeBuilderState()
        {
            registeredNames = new NamedTypesRegistry(ParentNameRegistry);
            extensions = new List<UnityContainerExtension>();

            lifetimeContainer = new LifetimeContainer();
            strategies = new StagedStrategyChain<UnityBuildStage>(ParentStrategies);
            buildPlanStrategies = new StagedStrategyChain<UnityBuildStage>(ParentBuildPlanStrategies);
            policies = new PolicyList(ParentPolicies);
            policies.Set<IRegisteredNamesPolicy>(new RegisteredNamesPolicy(registeredNames), null);

            cachedStrategies = null;
            cachedStrategiesLock = new object();
        }

        private StagedStrategyChain<UnityBuildStage> ParentStrategies
        {
            get { return parent == null ? null : parent.strategies; }
        }

        private StagedStrategyChain<UnityBuildStage> ParentBuildPlanStrategies
        {
            get { return parent == null ? null : parent.buildPlanStrategies; }
        }

        private PolicyList ParentPolicies
        {
            get { return parent == null ? null : parent.policies; }
        }

        private NamedTypesRegistry ParentNameRegistry
        {
            get { return parent == null ? null : parent.registeredNames; }
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

        /// <summary>
        /// Remove policies associated with building this type. This removes the
        /// compiled build plan so that it can be rebuilt with the new settings
        /// the next time this type is resolved.
        /// </summary>
        /// <param name="typeToInject">Type of object to clear the plan for.</param>
        /// <param name="name">Name the object is being registered with.</param>
        private void ClearExistingBuildPlan(Type typeToInject, string name)
        {
            var buildKey = new NamedTypeBuildKey(typeToInject, name);
            DependencyResolverTrackerPolicy.RemoveResolvers(policies, buildKey);
            policies.Set<IBuildPlanPolicy>(new OverriddenBuildPlanMarkerPolicy(), buildKey);
        }

        private void FillTypeRegistrationDictionary(IDictionary<Type, List<string>> typeRegistrations)
        {
            if (parent != null)
            {
                parent.FillTypeRegistrationDictionary(typeRegistrations);
            }

            foreach (Type t in registeredNames.RegisteredTypes)
            {
                if (!typeRegistrations.ContainsKey(t))
                {
                    typeRegistrations[t] = new List<string>();
                }

                typeRegistrations[t] =
                    (typeRegistrations[t].Concat(registeredNames.GetKeys(t))).Distinct().ToList();
            }
        }


        public IServiceProvider AlternativeServiceProvider
        {
            get { return alternativeServiceProvider; }
            set { alternativeServiceProvider = value; }
        }
    }
}
