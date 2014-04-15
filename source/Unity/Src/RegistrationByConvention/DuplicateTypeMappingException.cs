// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Practices.Unity.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// The exception that is thrown when registering multiple types would result in an type mapping being overwritten.
    /// </summary>
    // FxCop suppression: The standard constructors don't make sense for this exception,
    // as calling them will leave out the information that makes the exception useful
    // in the first place.
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors",
        Justification = "The standard constructors don't make sense for this exception, as calling them will leave out the information that makes the exception useful in the first place.")]
    public partial class DuplicateTypeMappingException : Exception
    {
        private string name;
        private string mappedFromType;
        private string currentMappedToType;
        private string newMappedToType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateTypeMappingException"/> class.
        /// </summary>
        /// <param name="name">The name for the mapping.</param>
        /// <param name="mappedFromType">The source type for the mapping.</param>
        /// <param name="currentMappedToType">The type currently mapped.</param>
        /// <param name="newMappedToType">The new type to map.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public DuplicateTypeMappingException(string name, Type mappedFromType, Type currentMappedToType, Type newMappedToType)
            : base(CreateMessage(name, mappedFromType, currentMappedToType, newMappedToType))
        {
            Guard.ArgumentNotNull(mappedFromType, "mappedFromType");
            Guard.ArgumentNotNull(currentMappedToType, "currentMappedToType");
            Guard.ArgumentNotNull(newMappedToType, "newMappedToType");

            this.name = name;
            this.mappedFromType = mappedFromType.AssemblyQualifiedName;
            this.currentMappedToType = currentMappedToType.AssemblyQualifiedName;
            this.newMappedToType = newMappedToType.AssemblyQualifiedName;

            this.RegisterSerializationHandler();
        }

        private static string CreateMessage(string name, Type mappedFromType, Type currentMappedToType, Type newMappedToType)
        {
            return string.Format(CultureInfo.CurrentCulture, Resources.DuplicateTypeMappingException, name, mappedFromType, currentMappedToType, newMappedToType);
        }

        partial void RegisterSerializationHandler();

        /// <summary>
        /// Gets the name for the mapping.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the source type for the mapping.
        /// </summary>
        public string MappedFromType
        {
            get
            {
                return this.mappedFromType;
            }
        }

        /// <summary>
        /// Gets the type currently mapped.
        /// </summary>
        public string CurrentMappedToType
        {
            get
            {
                return this.currentMappedToType;
            }
        }

        /// <summary>
        /// Gets the new type to map.
        /// </summary>
        public string NewMappedToType
        {
            get
            {
                return this.newMappedToType;
            }
        }
    }
}
