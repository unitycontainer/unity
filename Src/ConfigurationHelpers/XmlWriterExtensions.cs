// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Xml;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.Configuration.ConfigurationHelpers
{
    /// <summary>
    /// Helper methods on <see cref="XmlWriter"/>.
    /// </summary>
    public static class XmlWriterExtensions
    {
        /// <summary>
        /// A helper method to make it more foolproof to write elements. This takes care of writing the
        /// start and end element tags, and takes a nested closure with the code to write the content of
        /// the tag. That way the caller doesn't need to worry about the details of getting the start
        /// and end tags correct.
        /// </summary>
        /// <remarks>
        /// We don't support XML Namespaces here because .NET configuration doesn't use them so
        /// we don't need it for this current implementation.
        /// </remarks>
        /// <param name="writer">XmlWriter to write to.</param>
        /// <param name="elementName">Name of element.</param>
        /// <param name="writeContent">Nested lambda which, when executed, will create the content for the
        /// element.</param>
        /// <returns><paramref name="writer"/> (for method chaining if desired).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public static XmlWriter WriteElement(this XmlWriter writer, string elementName, Action<XmlWriter> writeContent)
        {
            Guard.ArgumentNotNull(writer, "writer");
            Guard.ArgumentNotNull(writeContent, "writeContent");
            writer.WriteStartElement(elementName);
            writeContent(writer);
            writer.WriteEndElement();
            return writer;
        }

        /// <summary>
        /// A helper method to make it easier to output attributes. If the <paramref name="attributeValue"/> is
        /// null or an empty string, output nothing, else output the given XML attribute.
        /// </summary>
        /// <param name="writer">Writer to output to.</param>
        /// <param name="attributeName">Attribute name to write.</param>
        /// <param name="attributeValue">Value for the attribute.</param>
        /// <returns><paramref name="writer"/> (for method chaining if desired).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public static XmlWriter WriteAttributeIfNotEmpty(this XmlWriter writer, string attributeName, string attributeValue)
        {
            Guard.ArgumentNotNull(writer, "writer");
            if (!string.IsNullOrEmpty(attributeValue))
            {
                writer.WriteAttributeString(attributeName, attributeValue);
            }
            return writer;
        }
    }
}
