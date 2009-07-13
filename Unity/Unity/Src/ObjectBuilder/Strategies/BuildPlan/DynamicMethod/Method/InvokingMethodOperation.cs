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
using Microsoft.Practices.Unity.Properties;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// A class that records that a constructor is about to be call, and is 
    /// responsible for generating the error string required when
    /// an error has occurred.
    /// </summary>
    public class InvokingMethodOperation
    {
        private readonly Type typeBeingConstructed;
        private readonly string methodSignature;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvokingMethodOperation"/> class.
        /// </summary>
        public InvokingMethodOperation(Type typeBeingConstructed, string methodSignature)
        {
            this.typeBeingConstructed = typeBeingConstructed;
            this.methodSignature = methodSignature;
        }

        /// <summary>
        /// The type currently being constructed.
        /// </summary>
        public Type TypeBeingConstructed
        {
            get { return typeBeingConstructed; }
        }

        /// <summary>
        /// Method we're trying to call.
        /// </summary>
        public string MethodSignature
        {
            get { return methodSignature; }
        }

        /// <summary>
        /// Generate the description string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                Resources.InvokingMethodOperation,
                typeBeingConstructed.Name,
                methodSignature);
        }
    }
}
