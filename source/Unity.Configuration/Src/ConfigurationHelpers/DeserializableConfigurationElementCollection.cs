// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Configuration;

namespace Microsoft.Practices.Unity.Configuration.ConfigurationHelpers
{
    /// <summary>
    /// Specialization of <see cref="DeserializableConfigurationElementCollectionBase{TElement}"/>
    /// that provides a canned implmentation of <see cref="ConfigurationElementCollection.CreateNewElement()"/>.
    /// </summary>
    /// <typeparam name="TElement">Type of configuration element in the collection.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Deserializable",
        Justification = "It is spelled correctly.")]
    public abstract class DeserializableConfigurationElementCollection<TElement> :
        DeserializableConfigurationElementCollectionBase<TElement>
        where TElement : DeserializableConfigurationElement, new()
    {
        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new TElement();
        }
    }
}
