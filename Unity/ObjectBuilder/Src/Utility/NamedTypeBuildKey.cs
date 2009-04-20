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
using System.Globalization;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Build key used to combine a type object with a string name. Used by
    /// ObjectBuilder to indicate exactly what is being built.
    /// </summary>
    public struct NamedTypeBuildKey : IBuildKey
    {
        private Type type;
        private string name;

        /// <summary>
        /// Create a new <see cref="NamedTypeBuildKey"/> instance with the given
        /// type and name.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to build.</param>
        /// <param name="name">Key to use to look up type mappings and singletons.</param>
        public NamedTypeBuildKey(Type type, string name)
        {
            this.type = type;
            this.name = !string.IsNullOrEmpty(name) ? name : null;
        }

        /// <summary>
        /// Create a new <see cref="NamedTypeBuildKey"/> instance for the default
        /// buildup of the given type.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to build.</param>
        public NamedTypeBuildKey(Type type)
            : this(type, null)
        {
            
        }

        /// <summary>
        /// This helper method creates a new <see cref="NamedTypeBuildKey"/> instance. It is
        /// initialized for the default key for the given type.
        /// </summary>
        /// <typeparam name="T">Type to build.</typeparam>
        /// <returns>A new <see cref="NamedTypeBuildKey"/> instance.</returns>
        public static NamedTypeBuildKey Make<T>()
        {
            return new NamedTypeBuildKey(typeof (T));
        }

        /// <summary>
        /// This helper method creates a new <see cref="NamedTypeBuildKey"/> instance for
        /// the given type and key.
        /// </summary>
        /// <typeparam name="T">Type to build</typeparam>
        /// <param name="name">Key to use to look up type mappings and singletons.</param>
        /// <returns>A new <see cref="NamedTypeBuildKey"/> instance initialized with the given type and name.</returns>
        public static NamedTypeBuildKey Make<T>(string name)
        {
            return new NamedTypeBuildKey(typeof (T), name);
        }

        /// <summary>
        /// Return the <see cref="Type"/> stored in this build key.
        /// </summary>
        /// <value>The type to build.</value>
        public Type Type
        {
            get { return type; }
        }

        /// <summary>
        /// Returns the name stored in this build key.
        /// </summary>
        /// <remarks>The name to use when building.</remarks>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Construct a new build key object with the current type
        /// replaced with the specified <paramref name="newType"/>.
        /// </summary>
        /// <remarks>This method creates a new build key object, the original is unchanged.</remarks>
        /// <param name="newType">New type to place in the build key.</param>
        /// <returns>The new build key.</returns>
        public object ReplaceType(Type newType)
        {
            return new NamedTypeBuildKey(newType, name);
        }

        /// <summary>
        /// Compare two <see cref="NamedTypeBuildKey"/> instances.
        /// </summary>
        /// <remarks>Two <see cref="NamedTypeBuildKey"/> instances compare equal
        /// if they contain the same name and the same type. Also, comparing
        /// against a different type will also return false.</remarks>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>True if the two keys are equal, false if not.</returns>
        public override bool Equals(object obj)
        {
            if(!(obj is NamedTypeBuildKey))
            {
                return false;
            }
            return this == (NamedTypeBuildKey)obj;
        }

        /// <summary>
        /// Calculate a hash code for this instance.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            int typeHash = type == null ? 0 : type.GetHashCode();
            int nameHash = name == null ? 0 : name.GetHashCode();
            return typeHash ^ nameHash;
        }

        /// <summary>
        /// Compare two <see cref="NamedTypeBuildKey"/> instances for equality.
        /// </summary>
        /// <remarks>Two <see cref="NamedTypeBuildKey"/> instances compare equal
        /// if they contain the same name and the same type.</remarks>
        /// <param name="left">First of the two keys to compare.</param>
        /// <param name="right">Second of the two keys to compare.</param>
        /// <returns>True if the values of the keys are the same, else false.</returns>
        public static bool operator ==(NamedTypeBuildKey left, NamedTypeBuildKey right)
        {
            return left.type == right.type &&
                   string.Compare(left.name, right.name, StringComparison.Ordinal) == 0;
            
        }

        /// <summary>
        /// Compare two <see cref="NamedTypeBuildKey"/> instances for inequality.
        /// </summary>
        /// <remarks>Two <see cref="NamedTypeBuildKey"/> instances compare equal
        /// if they contain the same name and the same type. If either field differs
        /// the keys are not equal.</remarks>
        /// <param name="left">First of the two keys to compare.</param>
        /// <param name="right">Second of the two keys to compare.</param>
        /// <returns>false if the values of the keys are the same, else true.</returns>
        public static bool operator !=(NamedTypeBuildKey left, NamedTypeBuildKey right)
        {
            return !( left == right );
        }

        /// <summary>
        /// Formats the build key as a string (primarily for debugging).
        /// </summary>
        /// <returns>A readable string representation of the build key.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Build Key[{0}, {1}]", type, name ?? "null");
        }
    }
}
