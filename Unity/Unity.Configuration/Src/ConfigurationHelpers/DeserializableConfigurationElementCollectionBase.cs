using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Microsoft.Practices.Unity.Configuration.ConfigurationHelpers
{
    /// <summary>
    /// A base helper class for implementing configuration collections.
    /// </summary>
    /// <typeparam name="TElement">Type of configuration element contained in 
    /// the collection.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", 
        Justification = "This is a base class, name is reasonable")] 
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Deserializable",
        Justification = "It is spelled correctly")]
    public abstract class DeserializableConfigurationElementCollectionBase<TElement> :
        ConfigurationElementCollection,
        IEnumerable<TElement>,
        IDeserializableElement
        where TElement : ConfigurationElement
    {
        /// <summary>
        /// Indexer to retrieve items in the collection by index.
        /// </summary>
        /// <param name="index">Index of the item to get or set.</param>
        /// <returns>The item at the given index.</returns>
        public TElement this[int index]
        {
            get { return (TElement) BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        #region IDeserializableElement Members

        /// <summary>
        /// Load this element from the given <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">Contains the XML to initialize from.</param>
        public virtual void Deserialize(XmlReader reader)
        {
            DeserializeElement(reader, false);
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
            for (int index = 0; index < Count; ++index)
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
            BaseAdd(element);
        }

        /// <summary>
        /// Remove an element from the collection at the given index.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        /// <summary>
        /// Remove all the items in the collection.
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }
    }
}