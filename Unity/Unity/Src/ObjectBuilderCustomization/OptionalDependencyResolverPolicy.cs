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
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.Properties;

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
            if(type.IsValueType)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                                  Resources.OptionalDependenciesMustBeReferenceTypes,
                                  type.Name));
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
