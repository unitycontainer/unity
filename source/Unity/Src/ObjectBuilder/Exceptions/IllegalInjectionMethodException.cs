// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// The exception thrown when injection is attempted on a method
    /// that is an open generic or has out or ref params.
    /// </summary>
    public partial class IllegalInjectionMethodException : Exception
    {
        /// <summary>
        /// Construct a new <see cref="IllegalInjectionMethodException"/> with no
        /// message.
        /// </summary>
        public IllegalInjectionMethodException()
        {
        }

        /// <summary>
        /// Construct a <see cref="IllegalInjectionMethodException"/> with the given message
        /// </summary>
        /// <param name="message">Message to return.</param>
        public IllegalInjectionMethodException(string message) : base(message)
        {
        }

        /// <summary>
        /// Construct a <see cref="IllegalInjectionMethodException"/> with the given message
        /// and inner exception.
        /// </summary>
        /// <param name="message">Message to return.</param>
        /// <param name="innerException">Inner exception</param>
        public IllegalInjectionMethodException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
