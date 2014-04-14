// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Build key used to combine a type object with a string name. Used by
    /// ObjectBuilder to indicate exactly what is being built.
    /// </summary>
    public class NamedTypeBuildKey
    {
        private readonly Type type;
        private readonly string name;

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
            return new NamedTypeBuildKey(typeof(T));
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
            return new NamedTypeBuildKey(typeof(T), name);
        }

        /// <summary>
        /// Return the <see cref="Type"/> stored in this build key.
        /// </summary>
        /// <value>The type to build.</value>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "This is the type part of the key.")]
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
        /// Compare two <see cref="NamedTypeBuildKey"/> instances.
        /// </summary>
        /// <remarks>Two <see cref="NamedTypeBuildKey"/> instances compare equal
        /// if they contain the same name and the same type. Also, comparing
        /// against a different type will also return false.</remarks>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>True if the two keys are equal, false if not.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as NamedTypeBuildKey;
            if (other == null)
            {
                return false;
            }
            return this == other;
        }

        /// <summary>
        /// Calculate a hash code for this instance.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            int typeHash = type == null ? 0 : type.GetHashCode();
            int nameHash = name == null ? 0 : name.GetHashCode();
            return (typeHash + 37) ^ (nameHash + 17);
        }

        /// <summary>
        /// Compare two <see cref="NamedTypeBuildKey"/> instances for equality.
        /// </summary>
        /// <remarks>Two <see cref="NamedTypeBuildKey"/> instances compare equal
        /// if they contain the same name and the same type.</remarks>
        /// <param name="left">First of the two keys to compare.</param>
        /// <param name="right">Second of the two keys to compare.</param>
        /// <returns>True if the values of the keys are the same, else false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Null is accounted for")]
        public static bool operator ==(NamedTypeBuildKey left, NamedTypeBuildKey right)
        {
            var leftIsNull = ReferenceEquals(left, null);
            var rightIsNull = ReferenceEquals(right, null);
            if (leftIsNull && rightIsNull)
            {
                return true;
            }
            if (leftIsNull || rightIsNull)
            {
                return false;
            }

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
            return !(left == right);
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

    /// <summary>
    /// A generic version of <see cref="NamedTypeBuildKey"/> so that
    /// you can new up a key using generic syntax.
    /// </summary>
    /// <typeparam name="T">Type for the key.</typeparam>
    public class NamedTypeBuildKey<T> : NamedTypeBuildKey
    {
        /// <summary>
        /// Construct a new <see cref="NamedTypeBuildKey{T}"/> that
        /// specifies the given type.
        /// </summary>
        public NamedTypeBuildKey()
            : base(typeof(T), null)
        {
        }

        /// <summary>
        /// Construct a new <see cref="NamedTypeBuildKey{T}"/> that
        /// specifies the given type and name.
        /// </summary>
        /// <param name="name">Name for the key.</param>
        public NamedTypeBuildKey(string name)
            : base(typeof(T), name)
        {
        }
    }
}
