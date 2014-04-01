// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// Provides access to the names registered for a container.
    /// </summary>
    public interface IRegisteredNamesPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Gets the names registered for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The names registered for <paramref name="type"/>.</returns>
        IEnumerable<string> GetRegisteredNames(Type type);
    }
}