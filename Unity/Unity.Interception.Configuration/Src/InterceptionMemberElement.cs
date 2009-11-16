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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Properties;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Base class for interception-related configuration elements.
    /// </summary>
    public class InterceptionMemberElement : InjectionMemberElement
    {
        /// <summary>
        /// Gets a value indicating whether an unknown attribute is encountered during deserialization.
        /// </summary>
        /// <param name="name">The name of the unrecognized attribute.</param>
        /// <param name="value">The value of the unrecognized attribute.</param>
        /// <returns><see langword="true"/> when an unknown attribute is encountered while deserializing; otherwise, 
        /// <see langword="false"/>.</returns>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            return name == "elementType" ? true : base.OnDeserializeUnrecognizedAttribute(name, value);
        }

        /// <summary>
        /// Creates an instance of the type represented by <paramref name="typeName"/> through the default constructor.
        /// </summary>
        /// <typeparam name="TInterface">The base type the created instance is casted to.</typeparam>
        /// <param name="typeName">The name or the alias for the type of the instance to create.</param>
        /// <returns>A new instace of <paramref name="typeName"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Creation method")]
        protected TInterface CreateInstance<TInterface>(string typeName)
            where TInterface : class
        {
            TInterface instance = null;

            Type instanceType = GetResolvedType<TInterface>(typeName);
            try
            {
                instance = (TInterface)Activator.CreateInstance(instanceType);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.ExceptionCannotCreateInstance,
                        instanceType.Name),
                    e);
            }

            return instance;
        }

        /// <summary>
        /// A helper method that will resolve the type given by name to an actual type object.
        /// </summary>
        /// <typeparam name="TInterface">Target interface for the type name. Resolved type must
        /// be assignable to this type.</typeparam>
        /// <param name="typeName">Type string to resolve.</param>
        /// <returns>The resolved type.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Generic needed for type checking/validation")]
        protected Type GetResolvedType<TInterface>(string typeName)
        {
            Type instanceType = TypeResolver.ResolveType(typeName);
            if(!typeof(TInterface).IsAssignableFrom(instanceType))
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.ExceptionResolvedTypeNotCompatible,
                        typeName,
                        instanceType.Name,
                        typeof(TInterface).Name));                
            }
            return instanceType;
        }
    }
}
