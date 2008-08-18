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
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// This class provides a base class for configuration
    /// of a Unity container using user-installed container
    /// types.
    /// </summary>
    public class UnityContainerTypeConfigurationElement : TypeResolvingConfigurationElement, IContainerConfigurationCommand
    {
        private UnityTypeElement parentElement;

        /// <summary>
        /// The <see cref="UnityTypeElement"/> that contains this element
        /// in the config file.
        /// </summary>
        public UnityTypeElement ParentElement
        {
            get { return parentElement; }
            set { parentElement = value; }
        }

        /// <summary>
        /// Concrete type name for this configuration element.
        /// </summary>
        [ConfigurationProperty("extensionType", IsRequired = true)]
        public string ExtensionType
        {
            get { return (string)this["extensionType"]; }
            set { this["extensionType"] = value; }
        }

        /// <summary>
        /// Read in the contents of this element from the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader"><see cref="XmlReader"/> containing contents of element.</param>
        public void DeserializeElement(XmlReader reader)
        {
            base.DeserializeElement(reader, false);
        }

        /// <summary>
        /// Execute this command against the given container.
        /// </summary>
        /// <remarks>
        /// Interface implementors will implement this method to
        /// apply configuration changes to the container.</remarks>
        /// <param name="container">The <see cref="IUnityContainer"/> to configure.</param>
        public virtual void Configure(IUnityContainer container)
        {
            
        }
    }
}
