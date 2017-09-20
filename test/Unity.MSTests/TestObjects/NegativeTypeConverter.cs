// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Unity.Tests.TestObjects
{
    internal class NegativeTypeConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        ///
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        ///
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context. </param>
        /// <param name="sourceType">A <see cref="T:System.Type"></see> that represents the type you want to convert from. </param>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context. </param>
        /// <param name="destinationType">A <see cref="T:System.Type"></see> that represents the type you want to convert to. </param>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        ///
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"></see> to use as the current culture. </param>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context. </param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert. </param>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                int result = int.Parse(value.ToString());
                return -result;
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        ///
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"></see>. If null is passed, the current culture is assumed. </param>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context. </param>
        /// <param name="destinationType">The <see cref="T:System.Type"></see> to convert the value parameter to. </param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert. </param>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        /// <exception cref="T:System.ArgumentNullException">The destinationType parameter is null. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                int intValue = (int)value;
                return (-intValue).ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
