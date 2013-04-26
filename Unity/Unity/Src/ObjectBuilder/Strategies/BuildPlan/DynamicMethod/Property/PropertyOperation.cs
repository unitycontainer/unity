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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// A base class that holds the information shared by all operations
    /// performed by the container while setting properties.
    /// </summary>
    public abstract class PropertyOperation : BuildOperation
    {
        private readonly string propertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        protected PropertyOperation(Type typeBeingConstructed, string propertyName)
            : base(typeBeingConstructed)
        {
            this.propertyName = propertyName;
        }

        /// <summary>
        /// The property value currently being resolved.
        /// </summary>
        public string PropertyName
        {
            get { return propertyName; }
        }

        /// <summary>
        /// Generate the description of this operation.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                GetDescriptionFormat(),
                TypeBeingConstructed.GetTypeInfo().Name, propertyName);
        }

        /// <summary>
        /// Get a format string used to create the description. Called by
        /// the base <see cref='ToString'/> method.
        /// </summary>
        /// <returns>The format string.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
            Justification = "This could theoretically be expensive, and is easier to override for clients if it's a method.")]
        protected abstract string GetDescriptionFormat();
    }
}
