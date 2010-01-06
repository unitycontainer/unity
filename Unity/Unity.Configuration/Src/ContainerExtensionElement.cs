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
using System.Configuration;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Configuration element representing an extension to add to a container.
    /// </summary>
    public class ContainerExtensionElement : ContainerConfiguringElement
    {
        private const string TypeNamePropertyName = "type";

        /// <summary>
        /// Type of the extension to add.
        /// </summary>
        [ConfigurationProperty(TypeNamePropertyName, IsRequired = true)]
        public string TypeName
        {
            get { return (string) base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
        }

        /// <summary>
        /// Add the extension specified in this element to the container.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        protected override void ConfigureContainer(IUnityContainer container)
        {
            var extensionType = GetExtensionType();
            var extension = (UnityContainerExtension)container.Resolve(extensionType);
            container.AddExtension(extension);
        }

        private Type GetExtensionType()
        {
            return TypeResolver.ResolveType(TypeName, true);
        }
    }
}
