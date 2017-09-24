// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


namespace Unity
{
    /// <summary>
    /// Base interface for all extension configuration interfaces.
    /// </summary>
    public interface IUnityContainerExtensionConfigurator
    {
        /// <summary>
        /// Retrieve the container instance that we are currently configuring.
        /// </summary>
        IUnityContainer Container { get; }
    }
}
