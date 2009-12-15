using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// A configuration element that contains the top-level container configuration
    /// information for interception - handler policies and global interceptor definitions.
    /// </summary>
    public class InterceptionElement : ContainerConfiguringElement
    {
        private const string PoliciesPropertyName = "policies";

        private static readonly UnknownElementHandlerMap<InterceptionElement> unknownElementHandlerMap =
            new UnknownElementHandlerMap<InterceptionElement>
                {
                    { "policy", (ie, xr) => ReadUnwrappedElement(xr, ie.Policies)},
                };

            /// <summary>
        /// Policies defined for this container.
        /// </summary>
        [ConfigurationProperty(PoliciesPropertyName)]
        public PolicyElementCollection Policies
        {
            get { return (PolicyElementCollection) base[PoliciesPropertyName]; }
        }

        /// <summary>
        /// Gets a value indicating whether an unknown element is encountered during deserialization.
        /// </summary>
        /// <returns>
        /// true when an unknown element is encountered while deserializing; otherwise, false.
        /// </returns>
        /// <param name="elementName">The name of the unknown subelement.
        ///                 </param><param name="reader">The <see cref="T:System.Xml.XmlReader"/> being used for deserialization.
        ///                 </param><exception cref="T:System.Configuration.ConfigurationErrorsException">The element identified by <paramref name="elementName"/> is locked.
        ///                     - or -
        ///                     One or more of the element's attributes is locked.
        ///                     - or -
        ///                 <paramref name="elementName"/> is unrecognized, or the element has an unrecognized attribute.
        ///                     - or -
        ///                     The element has a Boolean attribute with an invalid value.
        ///                     - or -
        ///                     An attempt was made to deserialize a property more than once.
        ///                     - or -
        ///                     An attempt was made to deserialize a property that is not a valid member of the element.
        ///                     - or -
        ///                     The element cannot contain a CDATA or text element.
        ///                 </exception>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            return unknownElementHandlerMap.ProcessElement(this, elementName, reader) ||
                base.OnDeserializeUnrecognizedElement(elementName, reader);
        }

        /// <summary>
        /// Apply this element's configuration to the given <paramref name="container"/>.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        protected override void ConfigureContainer(IUnityContainer container)
        {
            Policies.ForEach(policy => policy.ConfigureContainer(container));
        }

        private static void ReadUnwrappedElement<TElement>(XmlReader reader, DeserializableConfigurationElementCollectionBase<TElement> collection)
            where TElement : DeserializableConfigurationElement, new()
        {
            var element = new TElement();
            element.Deserialize(reader);
            collection.Add(element);
        }
    }
}
