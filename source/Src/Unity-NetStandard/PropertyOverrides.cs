﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace Unity
{
    /// <summary>
    /// A convenience form of <see cref="PropertyOverride"/> that lets you
    /// specify multiple property overrides in one shot rather than having
    /// to construct multiple objects.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Not really a collection, only implements IEnumerable to get convenient initialization syntax.")]
    public class PropertyOverrides : OverrideCollection<PropertyOverride, string, object>
    {
        /// <summary>
        /// When implemented in derived classes, this method is called from the <see cref="OverrideCollection{TOverride,TKey,TValue}.Add"/>
        /// method to create the actual <see cref="ResolverOverride"/> objects.
        /// </summary>
        /// <param name="key">Key value to create the resolver.</param>
        /// <param name="value">Value to store in the resolver.</param>
        /// <returns>The created <see cref="ResolverOverride"/>.</returns>
        protected override PropertyOverride MakeOverride(string key, object value)
        {
            return new PropertyOverride(key, value);
        }
    }
}
