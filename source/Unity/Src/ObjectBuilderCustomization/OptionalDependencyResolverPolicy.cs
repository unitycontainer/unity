// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// A <see cref="IDependencyResolverPolicy"/> that will attempt to
    /// resolve a value, and return null if it cannot rather than throwing.
    /// </summary>
    public class OptionalDependencyResolverPolicy : IDependencyResolverPolicy
    {
        private readonly Type type;
        private readonly string name;

        /// <summary>
        /// Construct a new <see cref="OptionalDependencyResolverPolicy"/> object
        /// that will attempt to resolve the given name and type from the container.
        /// </summary>
        /// <param name="type">Type to resolve. Must be a reference type.</param>
        /// <param name="name">Name to resolve with.</param>
        public OptionalDependencyResolverPolicy(Type type, string name)
        {
            Guard.ArgumentNotNull(type, "type");
            if (type.GetTypeInfo().IsValueType)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                                  Resources.OptionalDependenciesMustBeReferenceTypes,
                                  type.GetTypeInfo().Name));
            }

            this.type = type;
            this.name = name;
        }

        /// <summary>
        /// Construct a new <see cref="OptionalDependencyResolverPolicy"/> object
        /// that will attempt to resolve the given type from the container.
        /// </summary>
        /// <param name="type">Type to resolve. Must be a reference type.</param>
        public OptionalDependencyResolverPolicy(Type type)
            : this(type, null)
        {
        }

        /// <summary>
        /// Type this resolver will resolve.
        /// </summary>
        public Type DependencyType
        {
            get { return type; }
        }

        /// <summary>
        /// Name this resolver will resolve.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        #region IDependencyResolverPolicy Members

        /// <summary>
        /// Get the value for a dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>The value for the dependency.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Entire purpose of this class is to eat the exception")]
        public object Resolve(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            var newKey = new NamedTypeBuildKey(type, name);
            try
            {
                return context.NewBuildUp(newKey);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}
