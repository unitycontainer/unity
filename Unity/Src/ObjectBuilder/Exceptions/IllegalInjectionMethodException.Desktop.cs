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
