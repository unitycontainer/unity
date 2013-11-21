// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// A builder policy used to create lifetime policy instances.
    /// Used by the LifetimeStrategy when instantiating open
    /// generic types.
    /// </summary>
    public interface ILifetimeFactoryPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Create a new instance of <see cref="ILifetimePolicy"/>.
        /// </summary>
        /// <returns>The new instance.</returns>
        ILifetimePolicy CreateLifetimePolicy();

        /// <summary>
        /// The type of Lifetime manager that will be created by this factory.
        /// </summary>
        Type LifetimeType { get; }
    }
}
