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

using System.Configuration;

namespace Microsoft.Practices.Unity.Configuration
{
    

    /// <summary>
    /// A <see cref="ConfigurationElement"/> that stores the configuration information
    /// for a single <see cref="IUnityContainer"/>.
    /// </summary>
    public class UnityContainerElement : TypeResolvingConfigurationElement, IContainerConfigurationCommand
    {
        /// <summary>
        /// Name of this container configuration. The config may be retrieved by name.
        /// </summary>
        /// <remarks>This name is only used for lookup from the config file. If not 
        /// specified, this is the default container configuration.
        /// </remarks>
        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// The collection of &lt;type&gt; elements that contain the actual configuration entries.
        /// </summary>
        [ConfigurationProperty("types")]
        [ConfigurationCollection(typeof(UnityTypeElementCollection), AddItemName = "type")]
        public UnityTypeElementCollection Types
        {
            get 
            { 
                UnityTypeElementCollection unityTypeElementCollection = (UnityTypeElementCollection)this["types"];
                unityTypeElementCollection.TypeResolver = TypeResolver;
                return unityTypeElementCollection;
            }
        }

        /// <summary>
        /// The collection of &lt;extension&gt; elements that specify which extensions to add to the container.
        /// </summary>
        [ConfigurationProperty("extensions")]
        [ConfigurationCollection(typeof(UnityContainerExtensionElementCollection))]
        public UnityContainerExtensionElementCollection Extensions
        {
            get
            {
                UnityContainerExtensionElementCollection extensionElements = 
                    (UnityContainerExtensionElementCollection)this["extensions"];
                extensionElements.TypeResolver = TypeResolver;
                return extensionElements;
            }
        }

        /// <summary>
        /// The collection of &lt;instance&gt; element that specify which instances to add to the container.
        /// </summary>
        [ConfigurationProperty("instances")]
        [ConfigurationCollection(typeof(UnityInstanceElementCollection))]
        public UnityInstanceElementCollection Instances
        {
            get
            {
                UnityInstanceElementCollection instanceElements = (UnityInstanceElementCollection) this["instances"];
                instanceElements.TypeResolver = TypeResolver;
                return instanceElements;
            }
        }

        /// <summary>
        /// The collection of extension configuration elements that are used to add
        /// arbitrary new extensions to the configuration file.
        /// </summary>
        [ConfigurationProperty("extensionConfig")]
        [ConfigurationCollection(typeof(UnityContainerExtensionConfigurationElementCollection))]
        public UnityContainerExtensionConfigurationElementCollection ExtensionConfig
        {
            get
            {
                UnityContainerExtensionConfigurationElementCollection extensionConfigs =
                    (UnityContainerExtensionConfigurationElementCollection)this["extensionConfig"];
                extensionConfigs.TypeResolver = TypeResolver;
                return extensionConfigs;
            }
        }

        /// <summary>
        /// Execute this command against the given container.
        /// </summary>
        /// <remarks>
        /// Interface implementors will implement this method to
        /// apply configuration changes to the container.</remarks>
        /// <param name="container">The <see cref="IUnityContainer"/> to configure.</param>
        public void Configure(IUnityContainer container)
        {
            foreach (UnityContainerExtensionElement extension in Extensions)
            {
                extension.Configure(container);
            }

            foreach (UnityTypeElement typeElement in Types)
            {
                typeElement.Configure(container);
            }

            foreach (UnityInstanceElement instanceElement in Instances)
            {
                instanceElement.Configure(container);
            }

            foreach(IContainerConfigurationCommand command in ExtensionConfig)
            {
                command.Configure(container);
            }
        }
    }
}
