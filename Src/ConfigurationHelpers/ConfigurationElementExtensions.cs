// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.Configuration.ConfigurationHelpers
{
    /// <summary>
    /// Helpful extension methods when implementing configuration sections
    /// that deserialize "unwrapped" elements - elements that should be
    /// deserialized into a container but can be present outside
    /// that container in the actual config file.
    /// </summary>
    public static class ConfigurationElementExtensions
    {
        /// <summary>
        /// Deserialize an element of the given type, store it in
        /// the collection object, and 
        /// </summary>
        /// <typeparam name="TElementType">Type of element to create and deserialize.</typeparam>
        /// <param name="baseElement">Parent element containing element to deserialize.</param>
        /// <param name="reader">Xml reader containing state to deserialize from.</param>
        /// <param name="elementCollection">Collection to store the created element into.</param>
        /// <returns>The created element.</returns>
        //[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "baseElement",
            //Justification = "Made this an extension method to get nice usage syntax.")]
        //[SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            //Justification = "Validation done by Guard class")]
        public static TElementType ReadUnwrappedElement<TElementType>(this ConfigurationElement baseElement,
            XmlReader reader, DeserializableConfigurationElementCollectionBase<TElementType> elementCollection)
            where TElementType : DeserializableConfigurationElement, new()
        {
            Guard.ArgumentNotNull(reader, "reader");
            Guard.ArgumentNotNull(elementCollection, "elementCollection");

            var element = new TElementType();
            element.Deserialize(reader);
            elementCollection.Add(element);
            return element;
        }

        /// <summary>
        /// Deserialize an element, basing the element type on the one
        /// supplied at runtime, and then store the element into the
        /// given <paramref name="elementCollection"/>.
        /// </summary>
        /// <remarks>This method is useful when reading elements into a polymorphic collection.</remarks>
        /// <typeparam name="TElementType">Base type of element to store.</typeparam>
        /// <param name="baseElement">Element that contains the collection being stored into.</param>
        /// <param name="reader">Xml Reader containing state to deserialize from.</param>
        /// <param name="elementType">Runtime type of element to create.</param>
        /// <param name="elementCollection">Collection to store the created element into.</param>
        /// <returns>The created element.</returns>
        //[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "baseElement",
            //Justification = "Made this an extension method to get nice usage syntax.")]
        //[SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            //Justification = "Validation done by Guard class")]
        public static TElementType ReadElementByType<TElementType>(this ConfigurationElement baseElement,
            XmlReader reader, Type elementType, DeserializableConfigurationElementCollectionBase<TElementType> elementCollection)
            where TElementType : DeserializableConfigurationElement
        {
            Guard.ArgumentNotNull(reader, "reader");
            Guard.ArgumentNotNull(elementType, "elementType");
            Guard.ArgumentNotNull(elementCollection, "elementCollection");
            Guard.TypeIsAssignable(typeof(TElementType), elementType, "elementType");

            var element = (TElementType)Activator.CreateInstance(elementType);
            element.Deserialize(reader);
            elementCollection.Add(element);
            return element;
        }
    }
}
