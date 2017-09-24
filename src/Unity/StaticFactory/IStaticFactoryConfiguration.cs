// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Unity.StaticFactory
{
    /// <summary>
    /// Interface defining the configuration interface exposed by the
    /// Static Factory extension.
    /// </summary>
    public interface IStaticFactoryConfiguration : IUnityContainerExtensionConfigurator
    {
        /// <summary>
        /// Register the given factory delegate to be called when the container is
        /// asked to resolve <typeparamref name="TTypeToBuild"/>.
        /// </summary>
        /// <typeparam name="TTypeToBuild">Type that will be requested from the container.</typeparam>
        /// <param name="factoryMethod">Delegate to invoke to create the instance.</param>
        /// <returns>The container extension object this method was invoked on.</returns>
        IStaticFactoryConfiguration RegisterFactory<TTypeToBuild>(Func<IUnityContainer, object> factoryMethod);

        /// <summary>
        /// Register the given factory delegate to be called when the container is
        /// asked to resolve <typeparamref name="TTypeToBuild"/> and <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="TTypeToBuild">Type that will be requested from the container.</typeparam>
        /// <param name="name">The name that will be used when requesting to resolve this type.</param>
        /// <param name="factoryMethod">Delegate to invoke to create the instance.</param>
        /// <returns>The container extension object this method was invoked on.</returns>
        IStaticFactoryConfiguration RegisterFactory<TTypeToBuild>(string name,
            Func<IUnityContainer, object> factoryMethod);
    }
}
