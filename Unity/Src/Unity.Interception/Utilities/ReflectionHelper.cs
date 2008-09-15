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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A collection of utility functions to encapsulate details of
    /// reflection and finding attributes.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Given a MethodBase for a property's get or set method,
        /// return the corresponding property info.
        /// </summary>
        /// <param name="method">MethodBase for the property's get or set method.</param>
        /// <returns>PropertyInfo for the property, or null if method is not part of a property.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class.")]
        public static PropertyInfo GetPropertyFromMethod(MethodBase method)
        {
            Guard.ArgumentNotNull(method, "method");

            PropertyInfo property = null;
            if (method.IsSpecialName)
            {
                Type containingType = method.DeclaringType;
                if (containingType != null)
                {
                    if (method.Name.StartsWith("get_", StringComparison.InvariantCulture) ||
                        method.Name.StartsWith("set_", StringComparison.InvariantCulture))
                    {
                        string propertyName = method.Name.Substring(4);
                        property = containingType.GetProperty(propertyName);
                    }
                }
            }
            return property;
        }

        /// <summary>
        /// Given a particular MemberInfo, return the custom attributes of the
        /// given type on that member.
        /// </summary>
        /// <typeparam name="TAttribute">Type of attribute to retrieve.</typeparam>
        /// <param name="member">The member to look at.</param>
        /// <param name="inherits">True to include attributes inherited from base classes.</param>
        /// <returns>Array of found attributes.</returns>
        public static TAttribute[] GetAttributes<TAttribute>(MemberInfo member, bool inherits) where TAttribute : Attribute
        {
            object[] attributesAsObjects = member.GetCustomAttributes(typeof(TAttribute), inherits);
            TAttribute[] attributes = new TAttribute[attributesAsObjects.Length];
            int index = 0;
            Array.ForEach(attributesAsObjects,
                delegate(object o)
                {
                    attributes[index++] = (TAttribute)o;
                });
            return attributes;
        }

        /// <summary>
        /// Given a particular MemberInfo, find all the attributes that apply to this
        /// member. Specifically, it returns the attributes on the type, then (if it's a
        /// property accessor) on the property, then on the member itself.
        /// </summary>
        /// <typeparam name="TAttribute">Type of attribute to retrieve.</typeparam>
        /// <param name="member">The member to look at.</param>
        /// <param name="inherits">true to include attributes inherited from base classes.</param>
        /// <returns>Array of found attributes.</returns>
        public static TAttribute[] GetAllAttributes<TAttribute>(MemberInfo member, bool inherits)
            where TAttribute : Attribute
        {
            List<TAttribute> attributes = new List<TAttribute>();

            if (member.DeclaringType != null)
            {
                attributes.AddRange(GetAttributes<TAttribute>(member.DeclaringType, inherits));

                MethodBase methodBase = member as MethodBase;
                if (methodBase != null)
                {
                    PropertyInfo prop = GetPropertyFromMethod(methodBase);
                    if (prop != null)
                    {
                        attributes.AddRange(GetAttributes<TAttribute>(prop, inherits));
                    }
                }
            }
            attributes.AddRange(GetAttributes<TAttribute>(member, inherits));
            return attributes.ToArray();
        }
    }
}
