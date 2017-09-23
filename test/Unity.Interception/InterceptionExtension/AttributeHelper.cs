// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Tests.InterceptionExtension
{
    /// <summary>
    /// Helper class to retrieve attributes.
    /// </summary>
    public class AttributeHelper
    {
        public static Attribute[] GetAttributes<T>(Type type)
            where T : Attribute
        {
            List<T> list = new List<T>();

            Attribute[] attributes = Attribute.GetCustomAttributes(type);
            foreach (Attribute item in attributes)
            {
                if (item.GetType() == typeof(T))
                {
                    list.Add(item as T);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo">The member information.</param>
        /// <returns></returns>
        public static Attribute[] GetAttributes<T>(System.Reflection.MemberInfo memberInfo)
            where T : Attribute
        {
            List<T> list = new List<T>();

            Attribute[] attributes = Attribute.GetCustomAttributes(memberInfo);
            foreach (Attribute item in attributes)
            {
                if (item.GetType() == typeof(T))
                {
                    list.Add(item as T);
                }
            }

            return list.ToArray();
        }
    }
}
