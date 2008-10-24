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
