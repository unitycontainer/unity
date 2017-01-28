﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Unity
{
    /// <summary>
    /// A convenience form of <see cref="DependencyOverride"/> that lets you
    /// specify multiple parameter overrides in one shot rather than having
    /// to construct multiple objects.
    /// </summary>
    /// <remarks>
    /// This class isn't really a collection, it just implements IEnumerable
    /// so that we get use of the nice C# collection initializer syntax.
    /// </remarks>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Not really a collection, only implements IEnumerable to get convenient initialization syntax.")]
    public class DependencyOverrides : OverrideCollection<DependencyOverride, Type, object>
    {
        /// <summary>
        /// When implemented in derived classes, this method is called from the <see cref="OverrideCollection{TOverride,TKey,TValue}.Add"/>
        /// method to create the actual <see cref="ResolverOverride"/> objects.
        /// </summary>
        /// <param name="key">Key value to create the resolver.</param>
        /// <param name="value">Value to store in the resolver.</param>
        /// <returns>The created <see cref="ResolverOverride"/>.</returns>
        protected override DependencyOverride MakeOverride(Type key, object value)
        {
            return new DependencyOverride(key, value);
        }
    }
}
