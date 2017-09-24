// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using ObjectBuilder2;
using Unity.ObjectBuilder;

namespace Unity
{
    /// <summary>
    /// The <see cref="ExtensionContext"/> class provides the means for extension objects
    /// to manipulate the internal state of the <see cref="UnityContainer"/>.
    /// </summary>
    public abstract class ExtensionContext
    {
        /// <summary>
        /// The container that this context is associated with.
        /// </summary>
        /// <value>The <see cref="IUnityContainer"/> object.</value>
        public abstract IUnityContainer Container { get; }

        /// <summary>
        /// The strategies this container uses.
        /// </summary>
        /// <value>The <see cref="StagedStrategyChain{TStageEnum}"/> that the container uses to build objects.</value>
        public abstract StagedStrategyChain<UnityBuildStage> Strategies { get; }

        /// <summary>
        /// The strategies this container uses to construct build plans.
        /// </summary>
        /// <value>The <see cref="StagedStrategyChain{TStageEnum}"/> that this container uses when creating
        /// build plans.</value>
        public abstract StagedStrategyChain<UnityBuildStage> BuildPlanStrategies { get; }

        /// <summary>
        /// The policies this container uses.
        /// </summary>
        /// <remarks>The <see cref="IPolicyList"/> the that container uses to build objects.</remarks>
        public abstract IPolicyList Policies { get; }

        /// <summary>
        /// The <see cref="ILifetimeContainer"/> that this container uses.
        /// </summary>
        /// <value>The <see cref="ILifetimeContainer"/> is used to manage <see cref="IDisposable"/> objects that the container is managing.</value>
        public abstract ILifetimeContainer Lifetime { get; }

        /// <summary>
        /// Store a type/name pair for later resolution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When users register type mappings (or other things) with a named key, this method
        /// allows you to register that name with the container so that when the <see cref="IUnityContainer.ResolveAll"/>
        /// method is called, that name is included in the list that is returned.
        /// </para></remarks>
        /// <param name="t"><see cref="Type"/> to register.</param>
        /// <param name="name">Name associated with that type.</param>
        public abstract void RegisterNamedType(Type t, string name);

        /// <summary>
        /// This event is raised when the <see cref="IUnityContainer.RegisterType(Type,Type,string,LifetimeManager, InjectionMember[])"/> method,
        /// or one of its overloads, is called.
        /// </summary>
        public abstract event EventHandler<RegisterEventArgs> Registering;

        /// <summary>
        /// This event is raised when the <see cref="IUnityContainer.RegisterInstance(Type,string,object,LifetimeManager)"/> method,
        /// or one of its overloads, is called.
        /// </summary>
        public abstract event EventHandler<RegisterInstanceEventArgs> RegisteringInstance;

        /// <summary>
        /// This event is raised when the <see cref="IUnityContainer.CreateChildContainer"/> method is called, providing 
        /// the newly created child container to extensions to act on as they see fit.
        /// </summary>
        public abstract event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated;
    }
}
