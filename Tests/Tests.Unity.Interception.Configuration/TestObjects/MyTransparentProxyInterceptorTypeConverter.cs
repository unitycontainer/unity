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

using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests.TestObjects
{
    public class MyTransparentProxyInterceptorTypeConverter : TypeConverter
    {
        public static string sourceValue;

        public override object ConvertFrom(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value)
        {
            sourceValue = (string) value;
            return new TransparentProxyInterceptor();
        }
    }
}
