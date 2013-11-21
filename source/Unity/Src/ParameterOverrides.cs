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

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// A convenience form of <see cref="ParameterOverride"/> that lets you
    /// specify multiple parameter overrides in one shot rather than having
    /// to construct multiple objects.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Not really a collection, only implements IEnumerable to get convenient initialization syntax.")]
    public class ParameterOverrides : OverrideCollection<ParameterOverride, string, object>
    {
        /// <summary>
        /// When implemented in derived classes, this method is called from the <see cref="OverrideCollection{TOverride,TKey,TValue}.Add"/>
        /// method to create the actual <see cref="ResolverOverride"/> objects.
        /// </summary>
        /// <param name="key">Key value to create the resolver.</param>
        /// <param name="value">Value to store in the resolver.</param>
        /// <returns>The created <see cref="ResolverOverride"/>.</returns>
        protected override ParameterOverride MakeOverride(string key, object value)
        {
            return new ParameterOverride(key, value);
        }
    }
}
