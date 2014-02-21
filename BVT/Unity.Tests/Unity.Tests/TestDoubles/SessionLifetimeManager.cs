// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Unity.Tests.TestDoubles
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
            get { return this.sessionKey; }
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
            return ((SessionLifetimeManager)value).SessionKey;
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
            string key = this.Reverse((string)value);
            return new SessionLifetimeManager(key);
        }

        public override object ConvertTo(
            ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            string key = this.Reverse(((SessionLifetimeManager)value).SessionKey);
            return key;
        }

        private string Reverse(string s)
        {
            char[] temp = s.ToCharArray();
            temp = (char[])temp.Reverse<char>();

            return temp.JoinStrings<char>(String.Empty);
        }
    }
}
