// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace ObjectBuilder2
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
            get { return this.propertyName; }
        }

        /// <summary>
        /// Generate the description of this operation.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                this.GetDescriptionFormat(),
                TypeBeingConstructed.GetTypeInfo().Name, this.propertyName);
        }

        /// <summary>
        /// Get a format string used to create the description. Called by
        /// the base <see cref='ToString'/> method.
        /// </summary>
        /// <returns>The format string.</returns>
        protected abstract string GetDescriptionFormat();
    }
}
