// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// An object that gives the ability to add
    /// elements and aliases to a configuration section.
    /// </summary>
    public abstract class SectionExtensionContext
    {
        /// <summary>
        /// Add a new alias to the configuration section. This is useful
        /// for those extensions that add commonly used types to configuration
        /// so users don't have to alias them repeatedly.
        /// </summary>
        /// <param name="newAlias">The alias to use.</param>
        /// <param name="aliasedType">Type the alias maps to.</param>
        public abstract void AddAlias(string newAlias, Type aliasedType);

        /// <summary>
        /// Add a new alias to the configuration section. This is useful
        /// for those extensions that add commonly used types to configuration
        /// so users don't have to alias them repeatedly.
        /// </summary>
        /// <typeparam name="TAliased">Type the alias maps to.</typeparam>
        /// <param name="alias">The alias to use</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void AddAlias<TAliased>(string alias)
        {
            AddAlias(alias, typeof (TAliased));
        }

        /// <summary>
        /// Add a new element to the configuration section schema.
        /// </summary>
        /// <param name="tag">Tag name in the XML.</param>
        /// <param name="elementType">Type the tag maps to.</param>
        public abstract void AddElement(string tag, Type elementType);

        /// <summary>
        /// Add a new element to the configuration section schema.
        /// </summary>
        /// <typeparam name="TElement">Type the tag maps to.</typeparam>
        /// <param name="tag">Tag name in the XML.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void AddElement<TElement>(string tag)
            where TElement : DeserializableConfigurationElement
        {
            AddElement(tag, typeof (TElement));
        }
    }
}
