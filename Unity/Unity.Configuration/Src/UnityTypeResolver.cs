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

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Provides logic to resolve a type alias or type fullname to a concrete type
    /// </summary>
    public class UnityTypeResolver
    {
        private UnityTypeAliasCollection typeAliases;

        /// <summary>
        /// Create a new <see cref="UnityTypeResolver"/> which will use the
        /// given <paramref name="typeAliases"/> section to resolve.
        /// </summary>
        /// <param name="typeAliases">The configuration element that stores
        /// our mapping of aliases to type names.</param>
        public UnityTypeResolver(UnityTypeAliasCollection typeAliases)
        {
            this.typeAliases = typeAliases;
        }

        /// <summary>
        /// Resolves a type alias or type fullname to a concrete type
        /// </summary>
        /// <param name="typeName">Type alias or type fullname</param>
        /// <returns>The concrete Type</returns>
        public Type ResolveType(string typeName)
        {
            UnityTypeAlias typeAlias = typeAliases[typeName];

            if (typeAlias == null)
            {
                return Type.GetType(typeName, true);
            }

            return typeAlias.Type;
        }

        /// <summary>
        /// Resolve a type alias or type full name to a concrete type.
        /// If <paramref name="typeName"/> is null or empty, return the
        /// given <paramref name="defaultValue"/> instead.
        /// </summary>
        /// <param name="typeName">Type alias or full name to resolve.</param>
        /// <param name="defaultValue">Value to return if typeName is null or empty.</param>
        /// <returns>The concrete <see cref="Type"/>.</returns>
        public Type ResolveWithDefault(string typeName, Type defaultValue)
        {
            if(string.IsNullOrEmpty(typeName))
            {
                return defaultValue;
            }
            return ResolveType(typeName);
        }
    }
}
