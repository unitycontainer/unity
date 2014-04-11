// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Configuration;
using System.Xml;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Configuration element for building up an interception policy.
    /// </summary>
    public class PolicyElement : DeserializableConfigurationElement
    {
        private const string CallHandlersPropertyName = "callHandlers";
        private const string MatchingRulesPropertyName = "matchingRules";
        private const string NamePropertyName = "name";

        private static readonly UnknownElementHandlerMap<PolicyElement> UnknownElementHandlerMap =
            new UnknownElementHandlerMap<PolicyElement>
                {
                    { "matchingRule", (pe, xr) => pe.ReadUnwrappedElement(xr, pe.MatchingRules) },
                    { "callHandler", (pe, xr) => pe.ReadUnwrappedElement(xr, pe.CallHandlers) }
                };

        /// <summary>
        /// Name of this policy.
        /// </summary>
        [ConfigurationProperty(NamePropertyName, IsRequired = true)]
        public string Name
        {
            get { return (string)base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }

        /// <summary>
        /// Matching rules for this policy.
        /// </summary>
        [ConfigurationProperty(MatchingRulesPropertyName)]
        public MatchingRuleElementCollection MatchingRules
        {
            get { return (MatchingRuleElementCollection)base[MatchingRulesPropertyName]; }
        }

        /// <summary>
        /// Call handlers for this policy.
        /// </summary>
        [ConfigurationProperty(CallHandlersPropertyName)]
        public CallHandlerElementCollection CallHandlers
        {
            get { return (CallHandlerElementCollection)base[CallHandlersPropertyName]; }
        }

        /// <summary>
        /// Gets a value indicating whether an unknown element is encountered during deserialization.
        /// </summary>
        /// <returns>
        /// true when an unknown element is encountered while deserializing; otherwise, false.
        /// </returns>
        /// <param name="elementName">The name of the unknown subelement.</param>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> being used for deserialization.</param>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">The element identified by <paramref name="elementName"/> is locked.
        /// - or -
        /// One or more of the element's attributes is locked.
        /// - or -
        /// <paramref name="elementName"/> is unrecognized, or the element has an unrecognized attribute.
        /// - or -
        /// The element has a Boolean attribute with an invalid value.
        /// - or -
        /// An attempt was made to deserialize a property more than once.
        /// - or -
        /// An attempt was made to deserialize a property that is not a valid member of the element.
        /// - or -
        /// The element cannot contain a CDATA or text element.
        /// </exception>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            return UnknownElementHandlerMap.ProcessElement(this, elementName, reader) ||
                base.OnDeserializeUnrecognizedElement(elementName, reader);
        }

        /// <summary>
        /// Write the contents of this element to the given <see cref="XmlWriter"/>.
        /// </summary>
        /// <remarks>The caller of this method has already written the start element tag before
        /// calling this method, so deriving classes only need to write the element content, not
        /// the start or end tags.</remarks>
        /// <param name="writer">Writer to send XML content to.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public override void SerializeContent(XmlWriter writer)
        {
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(writer, "writer");

            writer.WriteAttributeString(NamePropertyName, this.Name);
            foreach (var matchingRuleElement in this.MatchingRules)
            {
                writer.WriteElement("matchingRule", matchingRuleElement.SerializeContent);
            }
            foreach (var callHandlerElement in this.CallHandlers)
            {
                writer.WriteElement("callHandler", callHandlerElement.SerializeContent);
            }
        }

        internal void ConfigureContainer(IUnityContainer container)
        {
            PolicyDefinition policyDefinition = container.Configure<Interception>().AddPolicy(this.Name);
            foreach (var matchingRuleElement in this.MatchingRules)
            {
                matchingRuleElement.Configure(container, policyDefinition);
            }

            foreach (var callHandlerElement in this.CallHandlers)
            {
                callHandlerElement.Configure(container, policyDefinition);
            }
        }
    }
}
