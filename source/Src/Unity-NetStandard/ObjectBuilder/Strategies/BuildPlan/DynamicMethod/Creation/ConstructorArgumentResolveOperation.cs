﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Unity.Properties;

namespace ObjectBuilder2
{
    /// <summary>
    /// This class records the information about which constructor argument is currently
    /// being resolved, and is responsible for generating the error string required when
    /// an error has occurred.
    /// </summary>
    public class ConstructorArgumentResolveOperation : BuildOperation
    {
        private readonly string constructorSignature;
        private readonly string parameterName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorArgumentResolveOperation"/> class.
        /// </summary>
        /// <param name="typeBeingConstructed">The type that is being constructed.</param>
        /// <param name="constructorSignature">A string representing the constructor being called.</param>
        /// <param name="parameterName">Parameter being resolved.</param>
        public ConstructorArgumentResolveOperation(Type typeBeingConstructed, string constructorSignature, string parameterName)
            : base(typeBeingConstructed)
        {
            this.constructorSignature = constructorSignature;
            this.parameterName = parameterName;
        }

        /// <summary>
        /// Generate the string describing what parameter was being resolved.
        /// </summary>
        /// <returns>The description string.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                Resources.ConstructorArgumentResolveOperation,
                this.parameterName, this.constructorSignature);
        }

        /// <summary>
        /// String describing the constructor being set up.
        /// </summary>
        public string ConstructorSignature
        {
            get { return this.constructorSignature; }
        }

        /// <summary>
        /// Parameter that's being resolved.
        /// </summary>
        public string ParameterName
        {
            get { return this.parameterName; }
        }
    }
}
