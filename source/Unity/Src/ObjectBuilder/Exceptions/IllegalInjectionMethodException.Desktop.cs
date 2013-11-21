// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// The exception thrown when injection is attempted on a method
    /// that is an open generic or has out or ref params.
    /// </summary>
    [Serializable]
    public partial class IllegalInjectionMethodException
    {
        /// <summary>
        /// Used for serialization.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Serialization context.</param>
        protected IllegalInjectionMethodException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
