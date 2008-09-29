//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Configuration;
using System.Globalization;
using System.Xml;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Properties;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Implements shared structure and behavior for the configuration elements representing call handlers
    /// and matching rules in the configuration file.
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
    /// This element is similar to the <see cref="UnityTypeElement"/>, except that it does not provide 
    /// an extension point for arbitrary configuration like the type element's &lt;typeConfig&gt; child.
    /// </para>
    /// </remarks>
    /// <seealso cref="CallHandlerConfigurationElement"/>
    /// <seealso cref="MatchingRuleConfigurationElement"/>
    /// <seealso cref="UnityTypeElement"/>
    public abstract class RuleDrivenPolicyElementConfigurationElement : TypeResolvingConfigurationElement
    {
        /// <summary>
        /// Returns name of the element.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// The type for the represented element.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = false)]
        public string TypeName
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        /// <summary>
        /// Sets the injection configuration for the given type and name.
        /// </summary>
        [ConfigurationProperty("injection", IsRequired = false)]
        public PolicyElementConfigurationElement Injection
        {
            get
            {
                PolicyElementConfigurationElement element = (PolicyElementConfigurationElement)this["injection"];
                element.TypeResolver = TypeResolver;
                return element;
            }
            set { this["injection"] = value; }
        }

        /// <summary>
        /// Sets the lifetime for the given type and name. Transient means 
        /// to create a new instance every type and is the default.
        /// Singleton means to return the same instance on every request.
        /// </summary>
        [ConfigurationProperty("lifetime", IsRequired = false)]
        public UnityLifetimeElement Lifetime
        {
            get
            {
                UnityLifetimeElement element = (UnityLifetimeElement)this["lifetime"];
                element.TypeResolver = TypeResolver;
                return element;
            }
            set { this["lifetime"] = value; }
        }

        /// <summary>
        /// The actual <see cref="System.Type"/> object for the 
        /// type this element is registering.
        /// </summary>
        public Type Type
        {
            get { return this.TypeResolver.ResolveType(this.TypeName); }
        }

        /// <summary>
        /// Reads XML from the configuration file.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> that reads from the configuration file.</param>
        /// <param name="serializeCollectionKey"><see langword="true"/> to serialize only the collectionkey properties; 
        /// otherwise, <see langword="false"/>.</param>
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);

            if (string.IsNullOrEmpty(this.TypeName) && this.Lifetime != null && this.Lifetime.HasData)
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.CannotHaveLifetimeWithoutTypeName,
                        this.Name),
                    reader);
            }

            if (string.IsNullOrEmpty(this.TypeName) && this.Injection != null && this.Injection.HasData)
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.CannotHaveInjectionWithoutTypeName,
                        this.Name),
                    reader);
            }
        }
    }
}