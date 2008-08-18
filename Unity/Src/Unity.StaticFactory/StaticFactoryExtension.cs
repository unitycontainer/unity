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

using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.StaticFactory
{
    /// <summary>
    /// A <see cref="UnityContainerExtension"/> that lets you register a
    /// delegate with the container to create an object, rather than calling
    /// the object's constructor.
    /// </summary>
    public class StaticFactoryExtension : UnityContainerExtension, IStaticFactoryConfiguration
    {
        /// <summary>
        /// Initialize this extension. This particular extension requires no
        /// initialization work.
        /// </summary>
        protected override void Initialize()
        {
        }

        /// <summary>
        /// Register the given factory delegate to be called when the container is
        /// asked to resolve <typeparamref name="=TTypeToBuild"/> and <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="TTypeToBuild">Type that will be requested from the container.</typeparam>
        /// <param name="name">The name that will be used when requesting to resolve this type.</param>
        /// <param name="factoryMethod">Delegate to invoke to create the instance.</param>
        /// <returns>The container extension object this method was invoked on.</returns>
        public IStaticFactoryConfiguration RegisterFactory<TTypeToBuild>(
            string name, FactoryDelegate factoryMethod)
        {
            FactoryBuildPlanDelegate planDelegate = delegate { return factoryMethod(Container); };
            Context.Policies.Set<IBuildPlanPolicy>(
                new FactoryDelegateBuildPlanPolicy(planDelegate),
                NamedTypeBuildKey.Make<TTypeToBuild>(name));
            return this;
        }

        /// <summary>
        /// Register the given factory delegate to be called when the container is
        /// asked to resolve <typeparamref name="=TTypeToBuild"/>.
        /// </summary>
        /// <typeparam name="TTypeToBuild">Type that will be requested from the container.</typeparam>
        /// <param name="factoryMethod">Delegate to invoke to create the instance.</param>
        /// <returns>The container extension object this method was invoked on.</returns>
        public IStaticFactoryConfiguration RegisterFactory<TTypeToBuild>(
            FactoryDelegate factoryMethod)
        {
            return RegisterFactory<TTypeToBuild>(null, factoryMethod);
        }
    }
}
