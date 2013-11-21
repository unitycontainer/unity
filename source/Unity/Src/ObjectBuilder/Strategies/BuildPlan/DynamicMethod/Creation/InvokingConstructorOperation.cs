// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
