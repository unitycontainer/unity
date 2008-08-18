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
using System.Runtime.Serialization;
using Microsoft.Practices.ObjectBuilder2.Properties;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents that a dependency could not be resolved.
    /// </summary>
    [Serializable]
    public class DependencyMissingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyMissingException"/> class with no extra information.
        /// </summary>
        public DependencyMissingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyMissingException"/> class with the given message.
        /// </summary>
        /// <param name="message">Some random message.</param>
        public DependencyMissingException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="DependencyMissingException"/> class with the given
        /// message and inner exception.
        /// </summary>
        /// <param name="message">Some random message</param>
        /// <param name="innerException">Inner exception.</param>

        public DependencyMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyMissingException"/> class with the build key of the object begin built.
        /// </summary>
        /// <param name="buildKey">The build key of the object begin built.</param>
        public DependencyMissingException(object buildKey)
            : base(string.Format(CultureInfo.CurrentCulture, 
                Resources.MissingDependency,
                buildKey))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyMissingException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination. </param>
        protected DependencyMissingException(SerializationInfo info,
            StreamingContext context)
            : base(info, context) {}
    }
}
