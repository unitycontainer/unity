// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents that a dependency could not be resolved.
    /// </summary>
    [Serializable]
    public partial class DependencyMissingException
    {
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
