// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Globalization;
using System.Threading;
using Unity.Configuration.ConfigurationHelpers;

namespace Unity.Configuration
{
    /// <summary>
    /// Base class used to derive new elements that can occur
    /// directly within a container element.
    /// </summary>
    public abstract class ExtensionConfigurationElement : DeserializableConfigurationElement
    {
        private static int extensionConfigurationCount;
        private readonly int extensionConfigurationNumber;

        /// <summary>
        /// Initialize a new instance of <see cref="ExtensionConfigurationElement"/>.
        /// </summary>
        protected ExtensionConfigurationElement()
        {
            this.extensionConfigurationNumber = Interlocked.Increment(ref extensionConfigurationCount);
        }

        /// <summary>
        /// Unique key generated for use in the collection class.
        /// </summary>
        public string Key { get { return string.Format(CultureInfo.InvariantCulture, "extensionConfig:{0}", this.extensionConfigurationNumber); } }

        internal void Configure(IUnityContainer container)
        {
            this.ConfigureContainer(container);
        }

        /// <summary>
        /// When overridden in a derived class, this method will make configuration
        /// calls into the given <paramref name="container"/> according to its contents.
        /// </summary>
        /// <param name="container">The container to configure.</param>
        protected abstract void ConfigureContainer(IUnityContainer container);
    }
}
