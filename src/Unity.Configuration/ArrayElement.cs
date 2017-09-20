// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using Unity.Configuration.ConfigurationHelpers;
using Unity.Configuration.Properties;

namespace Unity.Configuration
{
    /// <summary>
    /// A configuration element used to configure injection of
    /// a specific set of values into an array.
    /// </summary>
    public class ArrayElement : ParameterValueElement
    {
        private const string TypeNamePropertyName = "type";
        private const string ValuesPropertyName = "";

        /// <summary>
        /// Type of array to inject. This is actually the type of the array elements,
        /// not the array type. Optional, if not specified we take the type from
        /// our containing element.
        /// </summary>
        [ConfigurationProperty(TypeNamePropertyName, IsRequired = false)]
        public string TypeName
        {
            get { return (string)base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
        }

        /// <summary>
        /// Values used to calculate the contents of the array.
        /// </summary>
        [ConfigurationProperty(ValuesPropertyName, IsDefaultCollection = true)]
        public ParameterValueElementCollection Values
        {
            get { return (ParameterValueElementCollection)base[ValuesPropertyName]; }
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
            writer.WriteAttributeIfNotEmpty(TypeNamePropertyName, this.TypeName);
            foreach (var valueElement in this.Values)
            {
                ValueElementHelper.SerializeParameterValueElement(writer, valueElement, true);
            }
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
            this.GuardTypeIsAnArray(parameterType);

            Type elementType = this.GetElementType(parameterType);

            var values = this.Values.Select(v => v.GetInjectionParameterValue(container, elementType));

            if (elementType.IsGenericParameter)
            {
                return new GenericResolvedArrayParameter(elementType.Name, values.ToArray());
            }
            return new ResolvedArrayParameter(elementType, values.ToArray());
        }

        private void GuardTypeIsAnArray(Type externalParameterType)
        {
            if (string.IsNullOrEmpty(this.TypeName))
            {
                if (!externalParameterType.IsArray)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                        Resources.NotAnArray, externalParameterType.Name));
                }
            }
        }

        private Type GetElementType(Type parameterType)
        {
            return TypeResolver.ResolveTypeWithDefault(this.TypeName, null) ?? parameterType.GetElementType();
        }
    }
}
