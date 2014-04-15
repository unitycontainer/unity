// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// Base interface for all extension configuration interfaces.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Configurator", Justification = "Configurator IS spelled correctly")]
    public interface IUnityContainerExtensionConfigurator
    {
        /// <summary>
        /// Retrieve the container instance that we are currently configuring.
        /// </summary>
        IUnityContainer Container { get; }
    }
}
