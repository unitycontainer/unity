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
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A <see cref="ConfigurationElement"/> that holds the information from
    /// the configuration file about which extensions to add to the container.
    /// </summary>
    public class UnityContainerExtensionElement : TypeResolvingConfigurationElement, IContainerConfigurationCommand
    {
        /// <summary>
        /// The type or alias for the extension to add to the container.
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = null, IsRequired = true, IsKey = true )]
        public string TypeName
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        /// <summary>
        /// The type of extension to add to the container.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification="Back compat")]
        public Type Type
        {
            get
            {
                if(string.IsNullOrEmpty(TypeName))
                {
                    return typeof(string);
                }
                return TypeResolver.ResolveType(TypeName);
            }
        }

        /// <summary>
        /// Execute this command against the given container.
        /// </summary>
        /// <remarks>
        /// Interface implementors will implement this method to
        /// apply configuration changes to the container.</remarks>
        /// <param name="container">The <see cref="IUnityContainer"/> to configure.</param>
        // FxCop suppression: Validation done by Guard class
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification="Validation done by Guard class")]
        public void Configure(IUnityContainer container)
        {
            Guard.ArgumentNotNull(container, "container");
            ConstructorInfo ctor = Type.GetConstructor(Type.EmptyTypes);
            UnityContainerExtension instance = (UnityContainerExtension)ctor.Invoke(null);
            container.AddExtension(instance);
        }
    }
}
