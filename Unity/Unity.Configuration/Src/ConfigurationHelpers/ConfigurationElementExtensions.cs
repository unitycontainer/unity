using System.Configuration;
using System.Xml;

namespace Microsoft.Practices.Unity.Configuration.ConfigurationHelpers
{
    internal static class ConfigurationElementExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "baseElement",
            Justification = "Made this an extension method to get nice usage syntax.")]
        public static void ReadUnwrappedElement<TElementType, TCollectionType>(this ConfigurationElement baseElement,
            XmlReader reader, TCollectionType elementCollection)
            where TElementType : DeserializableConfigurationElement, new()
            where TCollectionType : DeserializableConfigurationElementCollectionBase<TElementType>
        {
            var element = new TElementType();
            element.Deserialize(reader);
            elementCollection.Add(element);
        }
    }
}