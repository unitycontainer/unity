// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Xml;
using Unity.Configuration.ConfigurationHelpers;
using Unity.Configuration.Properties;
using Unity.Utility;

namespace Unity.Configuration
{
    /// <summary>
    /// A <see cref="ParameterValueElement"/> derived class that describes
    /// a parameter that should be resolved through the container.
    /// </summary>
    public class DependencyElement : ParameterValueElement, IAttributeOnlyElement
    {
        private const string NamePropertyName = "name";
        private const string TypeNamePropertyName = "type";

        /// <summary>
        /// Create a new instance of <see cref="DependencyElement"/>.
        /// </summary>
        public DependencyElement()
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="DependencyElement"/> with
        /// properties initialized from the contents of 
        /// <paramref name="attributeValues"/>.
        /// </summary>
        /// <param name="attributeValues">Dictionary of name/value pairs to
        /// initialize this object with.</param>
        public DependencyElement(IDictionary<string, string> attributeValues)
        {
            SetIfPresent(attributeValues, "dependencyName", value => this.Name = value);
            SetIfPresent(attributeValues, "dependencyType", value => this.TypeName = value);
        }

        /// <summary>
        /// Name to use to when resolving. If empty, resolves the default.
        /// </summary>
        [ConfigurationProperty(NamePropertyName, IsRequired = false)]
        public string Name
        {
            get { return (string)base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }

        /// <summary>
        /// Name of type this dependency should resolve to. This is optional;
        /// without it the container will resolve the type of whatever
        /// property or parameter this element is contained in.
        /// </summary>
        [ConfigurationProperty(TypeNamePropertyName, IsRequired = false)]
        public string TypeName
        {
            get { return (string)base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
        }

        /// <summary>
        /// Write the contents of this element to the given <see cref="XmlWriter"/>.
        /// </summary>
        /// <remarks>The caller of this method has already written the start element tag before
        /// calling this method, so deriving classes only need to write the element content, not
        /// the start or end tags.</remarks>
        /// <param name="writer">Writer to send XML content to.</param>
        void IAttributeOnlyElement.SerializeContent(XmlWriter writer)
        {
            writer.WriteAttributeIfNotEmpty("dependencyName", this.Name)
                .WriteAttributeIfNotEmpty("dependencyType", this.TypeName);
        }

        /// <summary>
        /// Write the contents of this element to the given <see cref="XmlWriter"/>. This
        /// method always outputs an explicit &lt;dependency&gt; tag, instead of providing
        /// attributes to the parent method.
        /// </summary>
        /// <param name="writer">Writer to send XML content to.</param>
        public override void SerializeContent(XmlWriter writer)
        {
            writer.WriteAttributeIfNotEmpty(NamePropertyName, this.Name)
                .WriteAttributeIfNotEmpty(TypeNamePropertyName, this.TypeName);
        }

        /// <summary>
        /// Generate an <see cref="InjectionParameterValue"/> object
        /// that will be used to configure the container for a type registration.
        /// </summary>
        /// <param name="container">Container that is being configured. Supplied in order
        /// to let custom implementations retrieve services; do not configure the container
        /// directly in this method.</param>
        /// <param name="parameterType">Type of the </param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public override InjectionParameterValue GetInjectionParameterValue(IUnityContainer container, Type parameterType)
        {
            Guard.ArgumentNotNull(parameterType, "parameterType");

            string dependencyName = this.Name;
            if (string.IsNullOrEmpty(dependencyName))
            {
                dependencyName = null;
            }

            if (parameterType.IsGenericParameter)
            {
                if (!string.IsNullOrEmpty(this.TypeName))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.DependencyForGenericParameterWithTypeSet,
                            parameterType.Name,
                            this.TypeName));
                }

                return new GenericParameter(parameterType.Name, dependencyName);
            }

            return new ResolvedParameter(TypeResolver.ResolveTypeWithDefault(this.TypeName, parameterType), dependencyName);
        }

        private static void SetIfPresent(IDictionary<string, string> attributeValues, string key, Action<string> setter)
        {
            if (attributeValues.ContainsKey(key))
            {
                setter(attributeValues[key]);
            }
        }
    }
}
