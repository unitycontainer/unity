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
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Microsoft.Practices.Unity.Configuration.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A configuration element that specifies that a value
    /// is optional.
    /// </summary>
    public class OptionalElement : ParameterValueElement
    {
        private const string NamePropertyName = "name";
        private const string TypeNamePropertyName = "type";

        /// <summary>
        /// Name used to resolve the dependency, leave out or blank to resolve default.
        /// </summary>
        [ConfigurationProperty(NamePropertyName, IsRequired = false)]
        public string Name
        {
            get { return (string)base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }

        /// <summary>
        /// Type of dependency to resolve. If left out, resolved the type of
        /// the containing parameter or property.
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
        public override void SerializeContent(XmlWriter writer)
        {
            writer.WriteAttributeIfNotEmpty(NamePropertyName, Name)
                .WriteAttributeIfNotEmpty(TypeNamePropertyName, TypeName);
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
        public override InjectionParameterValue GetInjectionParameterValue(IUnityContainer container, Type parameterType)
        {
            Guard.ArgumentNotNull(parameterType, "parameterType");

            string dependencyName = Name;
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
                            Resources.DependencyForOptionalGenericParameterWithTypeSet,
                            parameterType.Name,
                            this.TypeName));
                }

                return new OptionalGenericParameter(parameterType.Name, dependencyName);
            }

            return new OptionalParameter(TypeResolver.ResolveTypeWithDefault(TypeName, parameterType), dependencyName);
        }
    }
}
