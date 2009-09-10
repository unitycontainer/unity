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

namespace Microsoft.Practices.Unity.StaticFactory
{
    /// <summary>
    /// Interface defining the configuration interface exposed by the
    /// Static Factory extension.
    /// </summary>
    public interface IStaticFactoryConfiguration : IUnityContainerExtensionConfigurator
    {
        /// <summary>
        /// Register the given factory delegate to be called when the container is
        /// asked to resolve <typeparamref name="=TTypeToBuild"/>.
        /// </summary>
        /// <typeparam name="TTypeToBuild">Type that will be requested from the container.</typeparam>
        /// <param name="factoryMethod">Delegate to invoke to create the instance.</param>
        /// <returns>The container extension object this method was invoked on.</returns>
        IStaticFactoryConfiguration RegisterFactory<TTypeToBuild>(Func<IUnityContainer, object> factoryMethod);

        /// <summary>
        /// Register the given factory delegate to be called when the container is
        /// asked to resolve <typeparamref name="=TTypeToBuild"/> and <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="TTypeToBuild">Type that will be requested from the container.</typeparam>
        /// <param name="name">The name that will be used when requesting to resolve this type.</param>
        /// <param name="factoryMethod">Delegate to invoke to create the instance.</param>
        /// <returns>The container extension object this method was invoked on.</returns>
        IStaticFactoryConfiguration RegisterFactory<TTypeToBuild>(string name,
            Func<IUnityContainer, object> factoryMethod);
    }
}
