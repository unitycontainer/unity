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
        private readonly Type typeToConstruct;

        /// <summary>
        /// Create an instance of <see cref="ParameterOverrides"/> that lets you specify
        /// overrides when calling the constructor for <paramref name="typeToConstruct"/>.
        /// </summary>
        /// <param name="typeToConstruct">Type to override constructor parameters.</param>
        public ParameterOverrides(Type typeToConstruct)
        {
            this.typeToConstruct = typeToConstruct;
        }

        /// <summary>
        /// When implemented in derived classes, this method is called from the <see cref="OverrideCollection{TOverride,TKey,TValue}.Add"/>
        /// method to create the actual <see cref="ResolverOverride"/> objects.
        /// </summary>
        /// <param name="key">Key value to create the resolver.</param>
        /// <param name="value">Value to store in the resolver.</param>
        /// <returns>The created <see cref="ResolverOverride"/>.</returns>
        protected override ParameterOverride MakeOverride(string key, object value)
        {
            return new ParameterOverride(typeToConstruct, key, value);
        }
    }

    /// <summary>
    /// A generic version of <see cref="ParameterOverrides"/> that let you specify the type
    /// using generics syntax.
    /// </summary>
    /// <typeparam name="T">Type of the object being constructed.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Not really a collection, only implements IEnumerable to get convenient initialization syntax.")]
    public class ParameterOverrides<T> : ParameterOverrides
    {

        /// <summary>
        /// Create an instance of <see cref="ParameterOverrides"/> that lets you specify
        /// overrides when calling the constructor for <typeparamref name="T"/>.
        /// </summary>
        public ParameterOverrides()
            : base(typeof(T))
        {
            
        }
    }
}
