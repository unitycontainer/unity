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

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// Base interface for all extension configuration interfaces.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Configurator",
        Justification = "Configurator IS spelled correctly")]
    public interface IUnityContainerExtensionConfigurator
    {
        /// <summary>
        /// Retrieve the container instance that we are currently configuring.
        /// </summary>
        IUnityContainer Container { get; }
    }
}
