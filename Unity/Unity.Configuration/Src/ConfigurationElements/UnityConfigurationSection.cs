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
    /// A <see cref="ConfigurationSection"/> that contains the configuration
    /// for one or more <see cref="UnityContainer"/>s.
    /// </summary>
    public class UnityConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Provides access to the container information in the section.
        /// </summary>
        [ConfigurationProperty("containers")]
        [ConfigurationCollection(typeof(UnityContainerElementCollection), AddItemName = "container")]
        public UnityContainerElementCollection Containers
        {
            get 
            { 
                UnityContainerElementCollection containers = (UnityContainerElementCollection)this["containers"];
                containers.TypeResolver = new UnityTypeResolver(this.TypeAliases);
                return containers;
            }
        }

        /// <summary>
        /// Provides access to the type alias information in the section.
        /// </summary>
        [ConfigurationProperty("typeAliases")]
        [ConfigurationCollection(typeof(UnityTypeAliasCollection), AddItemName = "typeAlias")]
        public UnityTypeAliasCollection TypeAliases
        {
            get { return (UnityTypeAliasCollection)this["typeAliases"]; }
        }
    }
}
