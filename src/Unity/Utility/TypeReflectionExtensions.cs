// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Utility
{
    /// <summary>
    /// Provides extension methods to the <see cref="Type"/> class due to the introduction 
    /// of <see cref="TypeInfo"/> class in the .NET for Windows Store apps.
    /// </summary>
    public static class TypeReflectionExtensions
    {
        /// <summary>
        /// Returns the constructor in <paramref name="type"/> that matches the specified constructor parameter types.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <param name="constructorParameters">The constructor parameter types.</param>
        /// <returns>The constructor that matches the specified parameter types.</returns>
        public static ConstructorInfo GetConstructorInfo(this Type type, params Type[] constructorParameters)
        {
            return type.GetTypeInfo().DeclaredConstructors
                .Single(c => !c.IsStatic && ParametersMatch(c.GetParameters(), constructorParameters));
        }

        /// <summary>
        /// Returns the non-static declared methods of a type or its base types.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <returns>An enumerable of the <see cref="MethodInfo"/> objects.</returns>
        public static IEnumerable<MethodInfo> GetMethodsHierarchical(this Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<MethodInfo>();
            }

            if (type.Equals(typeof(object)))
            {
                return type.GetTypeInfo().DeclaredMethods.Where(m => !m.IsStatic);
            }

            return type.GetTypeInfo().DeclaredMethods.Where(m => !m.IsStatic)
                    .Concat(GetMethodsHierarchical(type.GetTypeInfo().BaseType));
        }

        /// <summary>
        /// Returns the non-static method of a type or its based type.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <param name="methodName">The name of the method to seek.</param>
        /// <param name="closedParameters">The (closed) parameter type signature of the method.</param>
        /// <returns>The discovered <see cref="MethodInfo"/></returns>
        public static MethodInfo GetMethodHierarchical(this Type type, string methodName, Type[] closedParameters)
        {
            return type.GetMethodsHierarchical().Single(
                    m => m.Name.Equals(methodName) &&
                        ParametersMatch(m.GetParameters(), closedParameters));
        }

        /// <summary>
        /// Returns the declared properties of a type or its base types.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <returns>An enumerable of the <see cref="PropertyInfo"/> objects.</returns>
        public static IEnumerable<PropertyInfo> GetPropertiesHierarchical(this Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<PropertyInfo>();
            }

            if (type.Equals(typeof(object)))
            {
                return type.GetTypeInfo().DeclaredProperties;
            }

            return type.GetTypeInfo().DeclaredProperties
                                      .Concat(GetPropertiesHierarchical(type.GetTypeInfo().BaseType));
        }

        /// <summary>
        /// Determines if the types in a parameter set ordinally matches the set of supplied types.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="closedConstructorParameterTypes"></param>
        /// <returns></returns>
        public static bool ParametersMatch(ParameterInfo[] parameters, System.Type[] closedConstructorParameterTypes)
        {
            Unity.Utility.Guard.ArgumentNotNull(parameters, "parameters");
            Unity.Utility.Guard.ArgumentNotNull(closedConstructorParameterTypes, "closedConstructorParameterTypes");

            if (parameters.Length != closedConstructorParameterTypes.Length)
            {
                return false;
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                if (!parameters[i].ParameterType.Equals(closedConstructorParameterTypes[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
