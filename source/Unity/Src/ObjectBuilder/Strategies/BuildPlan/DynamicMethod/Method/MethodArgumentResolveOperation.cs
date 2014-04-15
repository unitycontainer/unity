// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.Practices.Unity.Properties;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// This class records the information about which constructor argument is currently
    /// being resolved, and is responsible for generating the error string required when
    /// an error has occurred.
    /// </summary>
    public class MethodArgumentResolveOperation : BuildOperation
    {
        private readonly string methodSignature;
        private readonly string parameterName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorArgumentResolveOperation"/> class.
        /// </summary>
        /// <param name="typeBeingConstructed">The type that is being constructed.</param>
        /// <param name="methodSignature">A string representing the method being called.</param>
        /// <param name="parameterName">Parameter being resolved.</param>
        public MethodArgumentResolveOperation(Type typeBeingConstructed, string methodSignature, string parameterName)
            : base(typeBeingConstructed)
        {
            this.methodSignature = methodSignature;
            this.parameterName = parameterName;
        }

        /// <summary>
        /// Generate the string describing what parameter was being resolved.
        /// </summary>
        /// <returns>The description string.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                Resources.MethodArgumentResolveOperation,
                this.parameterName, TypeBeingConstructed.GetTypeInfo().Name, this.methodSignature);
        }

        /// <summary>
        /// String describing the method being set up.
        /// </summary>
        public string MethodSignature
        {
            get { return this.methodSignature; }
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
