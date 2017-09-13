// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Unity.Configuration.ConfigurationHelpers
{
    /// <summary>
    /// A base helper class for implementing configuration collections.
    /// </summary>
    /// <typeparam name="TElement">Type of configuration element contained in 
    /// the collection.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is a base class, name is reasonable")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Deserializable",
        Justification = "It is spelled correctly")]
    public abstract class DeserializableConfigurationElementCollectionBase<TElement> :
        ConfigurationElementCollection,
        IEnumerable<TElement>
        where TElement : DeserializableConfigurationElement
    {
        /// <summary>
        /// Indexer to retrieve items in the collection by index.
        /// </summary>
        /// <param name="index">Index of the item to get or set.</param>
        /// <returns>The item at the given index.</returns>
        public TElement this[int index]
        {
            get { return this.GetElement(index); }
            set
            {
                if (this.GetElement(index) != null)
                {
                    this.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Plug point to get objects out of the collection.
        /// </summary>
        /// <param name="index">Index in the collection to retrieve the item from.</param>
        /// <returns>Item at that index or null if not present.</returns>
        protected virtual TElement GetElement(int index)
        {
            return (TElement)BaseGet(index);
        }

        /// <summary>
        /// Plug point to get objects out of the collection.
        /// </summary>
        /// <param name="key">Key to look up the object by.</param>
        /// <returns>Item with that key or null if not present.</returns>
        protected virtual TElement GetElement(object key)
        {
            return (TElement)BaseGet(key);
        }

        #region IDeserializableElement Members

        /// <summary>
        /// Load this element from the given <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">Contains the XML to initialize from.</param>
        public virtual void Deserialize(XmlReader reader)
        {
            this.DeserializeElement(reader, false);
        }

        #endregion

        #region IEnumerable<TElement> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public new IEnumerator<TElement> GetEnumerator()
        {
            for (int index = 0; index < this.Count; ++index)
            {
                yield return this[index];
            }
        }

        #endregion

        /// <summary>
        /// Add a new element to the collection.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public void Add(TElement element)
        {
            this.BaseAdd(element);
        }

        /// <summary>
        /// Remove an element from the collection at the given index.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            this.BaseRemoveAt(index);
        }

        /// <summary>
        /// Remove all the items in the collection.
        /// </summary>
        public void Clear()
        {
            this.BaseClear();
        }

        /// <summary>
        /// Write out the contents of this collection to the given
        /// <paramref name="writer"/> without a containing element
        /// corresponding directly to this container element. Each
        /// child element will have a tag name given by
        /// <paramref name="elementName"/>.
        /// </summary>
        /// <param name="writer"><see cref="XmlWriter"/> to output XML to.</param>
        /// <param name="elementName">Name of tag to generate.</param>
        public void SerializeElementContents(XmlWriter writer, string elementName)
        {
            foreach (TElement element in this)
            {
                writer.WriteElement(elementName, element.SerializeContent);
            }
        }
    }
}
