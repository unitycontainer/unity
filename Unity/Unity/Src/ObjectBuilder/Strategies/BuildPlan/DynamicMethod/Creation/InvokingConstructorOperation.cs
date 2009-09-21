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
    public class InvokingConstructorOperation : BuildOperation
    {
        private readonly string constructorSignature;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvokingConstructorOperation"/> class.
        /// </summary>
        public InvokingConstructorOperation(Type typeBeingConstructed, string constructorSignature)
            : base(typeBeingConstructed)
        {
            this.constructorSignature = constructorSignature;
        }

        /// <summary>
        /// Constructor we're trying to call.
        /// </summary>
        public string ConstructorSignature
        {
            get { return constructorSignature; }
        }

        /// <summary>
        /// Generate the description string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                Resources.InvokingConstructorOperation,
                constructorSignature);
        }
    }
}
