// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using ObjectBuilder2;

namespace Unity.TestSupport
{
    [TypeConverter(typeof(SessionKeyTypeConverter))]
    public class SessionLifetimeManager : LifetimeManager
    {
        private readonly string sessionKey;
        public static string LastUsedSessionKey;

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
            LastUsedSessionKey = this.sessionKey;
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
            string key = Reverse((string)value);
            return new SessionLifetimeManager(key);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            string key = Reverse(((SessionLifetimeManager)value).SessionKey);
            return key;
        }

        private static string Reverse(IEnumerable<char> s)
        {
            var chars = new Stack<char>(s);
            return chars.JoinStrings(String.Empty);
        }
    }
}
