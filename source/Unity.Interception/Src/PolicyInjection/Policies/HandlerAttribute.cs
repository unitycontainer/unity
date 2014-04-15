// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Base class for handler attributes used in the attribute-driven
    /// interception policy.
    /// </summary>
    public abstract class HandlerAttribute : Attribute
    {
        /// <summary>
        /// Derived classes implement this method. When called, it
        /// creates a new call handler as specified in the attribute
        /// configuration.
        /// </summary>
        /// <param name="container">The <see cref="IUnityContainer"/> to use when creating handlers,
        /// if necessary.</param>
        /// <returns>A new call handler object.</returns>
        public abstract ICallHandler CreateHandler(IUnityContainer container);

        private int order;

        /// <summary>
        /// Gets or sets the order in which the handler will be executed.
        /// </summary>
        public int Order
        {
            get { return this.order; }
            set { this.order = value; }
        }
    }
}
