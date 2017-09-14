// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Configuration;
using System.Globalization;
using System.Xml;
using Unity.Configuration;
using Unity.Configuration.ConfigurationHelpers;
using Unity.InterceptionExtension.Configuration.Properties;

namespace Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Base class for the two children of the Policy element:
    /// MatchingRuleElement and CallHandlerElement.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These configuration elements have a required "name" attribute, an optional "type" attribute, and 
    /// optional child elements &lt;lifetime&gt; and &lt;injection&gt;
    /// </para>
    /// <para>
    /// Elements without a value for the type attribute can only have a value for the name attribute, and 
    /// indicate that the represented handler or rule is configured elsewhere and that a reference to 
    /// the given name must be added to the policy to be resolved, while elements with a value for the type
    /// attribute indicate how the represented handler or rule should be built and can optionally specify
    /// lifetime management and injection configuration.
    /// </para>
    /// <para>
    /// This element is similar to the <see cref="RegisterElement"/>, except that it does not provide 
    /// an extension point for arbitrary configuration.
    /// </para>
    /// </remarks>
    public abstract class PolicyChildElement : DeserializableConfigurationElement
    {
        private const string InjectionPropertyName = "";
        private const string LifetimePropertyName = "lifetime";
        private const string NamePropertyName = "name";
        private const string TypeNamePropertyName = "type";

        /// <summary>
        /// Name of this item
        /// </summary>
        [ConfigurationProperty(NamePropertyName, IsRequired = true)]
        public string Name
        {
            get { return (string)base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }

        /// <summary>
        /// Type that implements this matching rule or call handler.
        /// </summary>
        [ConfigurationProperty(TypeNamePropertyName, IsRequired = false)]
        public string TypeName
        {
            get { return (string)base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
        }

        /// <summary>
        /// Injection members that control how this item is created.
        /// </summary>
        [ConfigurationProperty(InjectionPropertyName, IsDefaultCollection = true)]
        public InjectionMemberElementCollection Injection
        {
            get { return (InjectionMemberElementCollection)base[InjectionPropertyName]; }
        }

        /// <summary>
        /// Lifetime manager for this item.
        /// </summary>
        [ConfigurationProperty(LifetimePropertyName, IsRequired = false, DefaultValue = null)]
        public LifetimeElement Lifetime
        {
            get { return (LifetimeElement)base[LifetimePropertyName]; }
            set { base[LifetimePropertyName] = value; }
        }

        /// <summary>
        /// Reads XML from the configuration file.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> that reads from the configuration file.</param>
        /// <param name="serializeCollectionKey">true to serialize only the collection key properties; otherwise, false.</param>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">The element to read is locked.
        /// - or -
        /// An attribute of the current node is not recognized.
        /// - or -
        /// The lock status of the current node cannot be determined.  
        /// </exception>
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);

            if (string.IsNullOrEmpty(this.TypeName) && this.LifetimeIsPresent())
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.CannotHaveLifetimeWithoutTypeName,
                        this.Name),
                    reader);
            }

            if (string.IsNullOrEmpty(this.TypeName) && this.Injection.Count > 0)
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.CannotHaveInjectionWithoutTypeName,
                        this.Name),
                    reader);
            }
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
            Unity.Utility.Guard.ArgumentNotNull(writer, "writer");

            writer.WriteAttributeString(NamePropertyName, this.Name);
            writer.WriteAttributeIfNotEmpty(TypeNamePropertyName, this.TypeName);
            if (this.LifetimeIsPresent())
            {
                writer.WriteElement("lifetime", this.Lifetime.SerializeContent);
            }
            foreach (var injectionElement in this.Injection)
            {
                writer.WriteElement(injectionElement.ElementName, injectionElement.SerializeContent);
            }
        }

        private bool LifetimeIsPresent()
        {
            return !string.IsNullOrEmpty(this.Lifetime.TypeName) ||
                !string.IsNullOrEmpty(this.Lifetime.TypeConverterTypeName) ||
                    !string.IsNullOrEmpty(this.Lifetime.Value);
        }
    }
}
