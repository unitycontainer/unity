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
    /// A configuration element storing information about a single type alias.
    /// </summary>
    public class AliasElement : DeserializableConfigurationElement
    {
        private const string AliasPropertyName = "alias";
        private const string TypeNamePropertyName = "type";

        /// <summary>
        /// Construct a new, uninitialized <see cref="AliasElement"/>.
        /// </summary>
        public AliasElement()
        {
            
        }

        /// <summary>
        /// Construct a new <see cref="AliasElement"/> that is initialized
        /// to alias <paramref name="alias"/> to the target <paramref name="targetType"/>.
        /// </summary>
        /// <param name="alias">Alias to use.</param>
        /// <param name="targetType">Type that is aliased.</param>
        public AliasElement(string alias, Type targetType)
        {
            Alias = alias;
            TypeName = targetType.AssemblyQualifiedName;
        }

        /// <summary>
        /// The alias used for this type.
        /// </summary>
        [ConfigurationProperty(AliasPropertyName, IsRequired = true)]
        public string Alias
        {
            get { return (string) base[AliasPropertyName]; }
            set { base[AliasPropertyName] = value; }
        }

        /// <summary>
        /// The fully qualified name this alias refers to.
        /// </summary>
        [ConfigurationProperty(TypeNamePropertyName, IsRequired = true)]
        public string TypeName
        {
            get { return (string) base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
        }
    }
}
