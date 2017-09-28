using System;
using System.Reflection;
using ObjectBuilder2;
using Unity.Properties;
using System.Linq;
using System.Collections.Generic;
using Unity.ObjectBuilder;
using System.Globalization;

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


        #region ObjectBuilder

        private object DoBuildUp(Type t, object existing, string name, IEnumerable<ResolverOverride> resolverOverrides)
        {
            IBuilderContext context = null;

            try
            {
                context =
                    new BuilderContext(this, GetStrategies(), lifetimeContainer, policies, new NamedTypeBuildKey(t, name), existing);
                context.AddResolverOverrides(resolverOverrides);

                if (t.GetTypeInfo().IsGenericTypeDefinition)
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.CurrentCulture,
                        Resources.CannotResolveOpenGenericType,
                        t.FullName), nameof(t));
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


        #region Registrations

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
                    // Avoid infinite loop when someone
                    //  registers something which would end up 
                    //  disposing this container (e.g. container.RegisterInsance(container))
                    LifetimeContainer lifetimeContainerCopy = lifetimeContainer;
                    lifetimeContainer = null;
                    lifetimeContainerCopy.Dispose();

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


        #region Nested Types

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

        #endregion
    }
}
