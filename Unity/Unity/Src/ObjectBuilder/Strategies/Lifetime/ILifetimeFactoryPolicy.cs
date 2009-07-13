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
    }
}
