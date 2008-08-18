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
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using Microsoft.Practices.Unity.Configuration.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A <see cref='TypeConverter' /> that converts types
    /// to and from fully qualified type names.
    /// </summary>
    class AssemblyQualifiedTypeNameConverter : ConfigurationConverterBase
    {
        ///<summary>
        ///Determines whether the conversion is allowed.
        ///</summary>
        ///
        ///<returns>
        ///true if the conversion is allowed; otherwise, false. 
        ///</returns>
        ///
        ///<param name="type">The type to convert to.</param>
        ///<param name="ctx">The <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> object used for type conversion.</param>
        public override bool CanConvertTo(ITypeDescriptorContext ctx, Type type)
        {
            return type == typeof(string);
        }

        ///<summary>
        ///Determines whether the conversion is allowed.
        ///</summary>
        ///
        ///<returns>
        ///true if the conversion is allowed; otherwise, false.
        ///</returns>
        ///
        ///<param name="type">The <see cref="T:System.Type"></see> to convert from.</param>
        ///<param name="ctx">The <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> object used for type conversions.</param>
        public override bool CanConvertFrom(ITypeDescriptorContext ctx, Type type)
        {
            return type == typeof(string);
        }

        ///<summary>
        ///Converts the given object to the type of this converter, using the specified context and culture information.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Object"></see> that represents the converted value.
        ///</returns>
        ///
        ///<param name="culture">The <see cref="T:System.Globalization.CultureInfo"></see> to use as the current culture. </param>
        ///<param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context. </param>
        ///<param name="value">The <see cref="T:System.Object"></see> to convert. </param>
        ///<exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(
            ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Guard.ArgumentNotNull(value, "value");
            if(value.GetType() != typeof(string))
            {
                throw new NotSupportedException(Resources.ConversionNotSupported);
            }

            Type result = Type.GetType((string)value, true);
            return result;
        }

        ///<summary>
        ///Converts the given value object to the specified type, using the specified context and culture information.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Object"></see> that represents the converted value.
        ///</returns>
        ///
        ///<param name="culture">A <see cref="T:System.Globalization.CultureInfo"></see>. If null is passed, the current culture is assumed. </param>
        ///<param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context. </param>
        ///<param name="destinationType">The <see cref="T:System.Type"></see> to convert the value parameter to. </param>
        ///<param name="value">The <see cref="T:System.Object"></see> to convert. </param>
        ///<exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        ///<exception cref="T:System.ArgumentNullException">The destinationType parameter is null. </exception>
        public override object ConvertTo(
            ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Guard.ArgumentNotNull(value, "value");
            Guard.ArgumentNotNull(destinationType, "destinationType");
            if(destinationType != typeof(string))
            {
                throw new NotSupportedException(Resources.ConversionNotSupported);
            }
            string result = ( (Type)value ).AssemblyQualifiedName;
            return result;
        }
    }
}
