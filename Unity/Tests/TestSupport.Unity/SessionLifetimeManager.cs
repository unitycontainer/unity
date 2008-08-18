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
using System.ComponentModel;
using System.Globalization;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.TestSupport
{
    [TypeConverter(typeof(SessionKeyTypeConverter))]
    public class SessionLifetimeManager : LifetimeManager
    {
        private string sessionKey;

        public SessionLifetimeManager(string sessionKey)
        {
            this.sessionKey = sessionKey;
        }

        public string SessionKey
        {
            get { return sessionKey; }
        }

        public override object GetValue()
        {
            return null;
        }

        public override void SetValue(object newValue)
        {
            
        }

        public override void RemoveValue()
        {
            
        }
    }

    public class SessionKeyTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType.GetType() == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(
            ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return new SessionLifetimeManager((string)value);
        }

        public override object ConvertTo(
            ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return ( (SessionLifetimeManager)value ).SessionKey;
        }
    }

    public class ReversedSessionKeyTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(
            ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string key = Reverse( (string)value );
            return new SessionLifetimeManager(key);

        }

        public override object ConvertTo(
            ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            string key = Reverse(( (SessionLifetimeManager)value ).SessionKey);
            return key;
        }

        private string Reverse(string s)
        {
            Stack<char> chars = new Stack<char>(s);
            return Sequence.ToString(chars, "");
        }
    }
}
