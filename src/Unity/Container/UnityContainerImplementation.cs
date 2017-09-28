using System;
using System.Reflection;
using ObjectBuilder2;
using Unity.Properties;
using System.Linq;
using System.Collections.Generic;
using Unity.ObjectBuilder;

namespace Unity
{
    // This part implements default behavior of the container
    public partial class UnityContainer
    {
        #region Fields 

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
        private event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated = delegate { };

        #endregion


        #region Constructors

        /// <summary>
        /// Create a default <see cref="UnityContainer"/>.
        /// </summary>
        public UnityContainer()
            : this(null)
        {
            InitializeDefaultPolicies();
        }

        /// <summary>
        /// Create a <see cref="UnityContainer"/> with the given parent container.
        /// </summary>
        /// <param name="parentContainer">The parent <see cref="UnityContainer"/>. The current object
        /// will apply its own settings first, and then check the parent for additional ones.</param>
        private UnityContainer(UnityContainer parentContainer)
        {
            parent = parentContainer;

            Registering = OnRegister;
            RegisteringInstance = OnRegisterInstance;

            if (parent != null)
                parent.lifetimeContainer.Add(this);

            InitializeBuilderState();

            RegisterInstance(typeof(IUnityContainer), null, this, new ContainerLifetimeManager());
        }

        #endregion


        #region Default Behavior

        private void InitializeDefaultPolicies()
        {
            // Main strategy chain
            strategies.AddNew<LifetimeStrategy>(UnityBuildStage.Lifetime);
            strategies.AddNew<BuildKeyMappingStrategy>(UnityBuildStage.TypeMapping);

            strategies.AddNew<ArrayResolutionStrategy>(UnityBuildStage.Creation);
            strategies.AddNew<BuildPlanStrategy>(UnityBuildStage.Creation);

            // Build plan strategy chain
            buildPlanStrategies.AddNew<DynamicMethodConstructorStrategy>(UnityBuildStage.Creation);
            buildPlanStrategies.AddNew<DynamicMethodPropertySetterStrategy>(UnityBuildStage.Initialization);
            buildPlanStrategies.AddNew<DynamicMethodCallStrategy>(UnityBuildStage.Initialization);

            // Policies - mostly used by the build plan strategies
            policies.SetDefault<IConstructorSelectorPolicy>(new DefaultUnityConstructorSelectorPolicy());
            policies.SetDefault<IPropertySelectorPolicy>(new DefaultUnityPropertySelectorPolicy());
            policies.SetDefault<IMethodSelectorPolicy>(new DefaultUnityMethodSelectorPolicy());

            policies.SetDefault<IBuildPlanCreatorPolicy>(new DynamicMethodBuildPlanCreatorPolicy(buildPlanStrategies));

            policies.Set<IBuildPlanPolicy>(new DeferredResolveBuildPlanPolicy(), typeof(Func<>));
            policies.Set<ILifetimePolicy>(new PerResolveLifetimeManager(), typeof(Func<>));

            policies.Set<IBuildPlanCreatorPolicy>(new LazyDynamicMethodBuildPlanCreatorPolicy(), typeof(Lazy<>));
            policies.Set<IBuildPlanCreatorPolicy>(new EnumerableDynamicMethodBuildPlanCreatorPolicy(), typeof(IEnumerable<>));
        }

        private void OnRegister(object sender, RegisterEventArgs e)
        {
            registeredNames.RegisterType(e.TypeFrom ?? e.TypeTo, e.Name);
            if (e.TypeFrom != null)
            {
                if (e.TypeFrom.GetTypeInfo().IsGenericTypeDefinition && e.TypeTo.GetTypeInfo().IsGenericTypeDefinition)
                {
                    policies.Set<IBuildKeyMappingPolicy>(
                        new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(e.TypeTo, e.Name)),
                        new NamedTypeBuildKey(e.TypeFrom, e.Name));
                }
                else
                {
                    policies.Set<IBuildKeyMappingPolicy>(
                        new BuildKeyMappingPolicy(new NamedTypeBuildKey(e.TypeTo, e.Name)),
                        new NamedTypeBuildKey(e.TypeFrom, e.Name));
                }
            }
            if (e.LifetimeManager != null)
            {
                SetLifetimeManager(e.TypeFrom ?? e.TypeTo, e.Name, e.LifetimeManager);
            }
        }

        private void OnRegisterInstance(object sender, RegisterInstanceEventArgs e)
        {
            registeredNames.RegisterType(e.RegisteredType, e.Name);
            SetLifetimeManager(e.RegisteredType, e.Name, e.LifetimeManager);
            NamedTypeBuildKey identityKey = new NamedTypeBuildKey(e.RegisteredType, e.Name);
            policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(identityKey), identityKey);
            e.LifetimeManager.SetValue(e.Instance);
        }

        #endregion


        #region Lifetime Management

        private void SetLifetimeManager(Type lifetimeType, string name, LifetimeManager lifetimeManager)
        {
            if (lifetimeManager.InUse)
            {
                throw new InvalidOperationException(Resources.LifetimeManagerInUse);
            }
            if (lifetimeType.GetTypeInfo().IsGenericTypeDefinition)
            {
                LifetimeManagerFactory factory = new LifetimeManagerFactory(new ExtensionContextImpl(this), lifetimeManager.GetType());
                policies.Set<ILifetimeFactoryPolicy>(factory, new NamedTypeBuildKey(lifetimeType, name));
            }
            else
            {
                lifetimeManager.InUse = true;
                policies.Set<ILifetimePolicy>(lifetimeManager,
                    new NamedTypeBuildKey(lifetimeType, name));
                if (lifetimeManager is IDisposable)
                {
                    lifetimeContainer.Add(lifetimeManager);
                }
            }
        }

        // Works like the ExternallyControlledLifetimeManager, but uses regular instead of weak references
        private class ContainerLifetimeManager : LifetimeManager
        {
            private object value;

            public override object GetValue()
            {
                return value;
            }

            public override void SetValue(object newValue)
            {
                value = newValue;
            }

            public override void RemoveValue()
            {
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


    }
}
