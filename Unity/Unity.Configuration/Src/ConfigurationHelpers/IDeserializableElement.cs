using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Microsoft.Practices.Unity.Configuration.ConfigurationHelpers
{
    /// <summary>
    /// Interface exposing a public Deserialize method for configuration elements.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Deserializable",
        Justification = "It is spelled correctly")]
    public interface IDeserializableElement
    {
        /// <summary>
        /// Load this element from the given <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">Contains the XML to initialize from.</param>
        void Deserialize(XmlReader reader);
    }
}
