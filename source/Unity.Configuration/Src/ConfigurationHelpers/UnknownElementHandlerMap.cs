// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Microsoft.Practices.Unity.Configuration.ConfigurationHelpers
{
    /// <summary>
    /// A helper class used to map element tag names to a handler method
    /// used to interpret that element.
    /// </summary>
    /// <typeparam name="TContainingElement"></typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Not a collection, only implements IEnumerable to get initialization syntax.")]
    public class UnknownElementHandlerMap<TContainingElement> :
        IEnumerable<KeyValuePair<string, Action<TContainingElement, XmlReader>>>
        where TContainingElement : ConfigurationElement
    {
        private readonly Dictionary<string, Action<TContainingElement, XmlReader>> elementActionMap =
            new Dictionary<string, Action<TContainingElement, XmlReader>>();

        /// <summary>
        /// Add method to enable dictionary initializer syntax 
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="processingAction"></param>
        public void Add(string elementName, Action<TContainingElement, XmlReader> processingAction)
        {
            elementActionMap.Add(elementName, processingAction);
        }

        /// <summary>
        /// Process an unknown element according to the map entries.
        /// </summary>
        /// <param name="parentElement">Parent element that hit this unknown element.</param>
        /// <param name="elementName">Name of the unknown element.</param>
        /// <param name="reader">XmlReader positioned at start of element.</param>
        /// <returns>true if processed, false if not.</returns>
        public bool ProcessElement(TContainingElement parentElement, string elementName, XmlReader reader)
        {
            Action<TContainingElement, XmlReader> action;
            if (elementActionMap.TryGetValue(elementName, out action))
            {
                action(parentElement, reader);
                return true;
            }
            return false;
        }

        #region IEnumerable implementation

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<KeyValuePair<string, Action<TContainingElement, XmlReader>>> GetEnumerator()
        {
            return elementActionMap.GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// A helper class used to map element tag names to a handler method
    /// used to interpret that element.
    /// </summary>
    internal class UnknownElementHandlerMap : IEnumerable<KeyValuePair<string, Action<XmlReader>>>
    {
        private readonly Dictionary<string, Action<XmlReader>> elementActionMap =
            new Dictionary<string, Action<XmlReader>>();

        // Add method to enable dictionary initializer syntax

        public void Add(string elementName, Action<XmlReader> processingAction)
        {
            elementActionMap.Add(elementName, processingAction);
        }

        public bool ProcessElement(string elementName, XmlReader reader)
        {
            Action<XmlReader> action;
            if (elementActionMap.TryGetValue(elementName, out action))
            {
                action(reader);
                return true;
            }
            return false;
        }

        #region IEnumerable implementation

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<KeyValuePair<string, Action<XmlReader>>> GetEnumerator()
        {
            return elementActionMap.GetEnumerator();
        }

        #endregion
    }
}
