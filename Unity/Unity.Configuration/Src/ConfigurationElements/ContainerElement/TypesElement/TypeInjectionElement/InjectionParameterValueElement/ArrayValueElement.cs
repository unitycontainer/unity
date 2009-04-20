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
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using Microsoft.Practices.Unity.Configuration.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A configuration element that represents an array to be resolved.
    /// </summary>
    public class ArrayValueElement : InjectionParameterValueElement
    {
        private List<InjectionParameterValueElement> elements = new List<InjectionParameterValueElement>();

        /// <summary>
        /// Return an instance of <see cref="InjectionParameterValue"/> based
        /// on the contents of this element.
        /// </summary>
        /// <param name="targetType">Type of parent parameter. Ignored by this implementation.</param>
        /// <returns>The created InjectionParameterValue, ready to pass to the container config API.</returns>
        /// <seealso cref="ResolvedArrayParameter"/>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done via Guard class")]
        public override InjectionParameterValue CreateParameterValue(Type targetType)
        {
            Guard.ArgumentNotNull(targetType, "targetType");

            if (!targetType.IsArray)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.ParameterTypeIsNotArray,
                        targetType.Name,
                        this.ElementInformation.Source,
                        this.ElementInformation.LineNumber));
            }

            Type elementType = targetType.GetElementType();
            string elementTypeName = elementType.AssemblyQualifiedName;

            object[] elementValues = new object[elements.Count];
            for (int i = 0; i < elements.Count; i++)
            {
                elementValues[i]
                    = InjectionParameterValueHelper.CreateParameterValue(
                        elementTypeName,
                        null,
                        elements[i],
                        TypeResolver);
            }
            return new ResolvedArrayParameter(elementType, elementValues);
        }

        internal InjectionParameterValue CreateParameterValue(string genericParameterName)
        {
            object[] elementValues = new object[elements.Count];
            for (int i = 0; i < elements.Count; i++)
            {
                elementValues[i]
                    = InjectionParameterValueHelper.CreateParameterValue(
                        null,
                        genericParameterName,
                        elements[i],
                        TypeResolver);
            }
            return new GenericResolvedArrayParameter(genericParameterName, elementValues);
        }

        ///<summary>
        ///Gets a value indicating whether an unknown element is encountered during deserialization.
        ///</summary>
        ///
        ///<returns>
        ///true when an unknown element is encountered while deserializing; otherwise, false.
        ///</returns>
        ///
        ///<param name="reader">The <see cref="T:System.Xml.XmlReader"></see> being used for deserialization.</param>
        ///<param name="elementName">The name of the unknown subelement.</param>
        ///<exception cref="T:System.Configuration.ConfigurationErrorsException">The element identified by elementName is locked.- or -One or more of the element's attributes is locked.- or -elementName is unrecognized, or the element has an unrecognized attribute.- or -The element has a Boolean attribute with an invalid value.- or -An attempt was made to deserialize a property more than once.- or -An attempt was made to deserialize a property that is not a valid member of the element.- or -The element cannot contain a CDATA or text element.</exception>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            InjectionParameterValueElement element;

            if (InjectionParameterValueHelper.DeserializeUnrecognizedElement(
                elementName,
                reader,
                "array",
                null,
                out element))
            {
                this.elements.Add(element);

                return true;
            }

            return false;
        }
    }
}
