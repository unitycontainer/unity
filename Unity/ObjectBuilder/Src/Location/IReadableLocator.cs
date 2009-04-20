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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents a locator that can be read from.
    /// </summary>
    /// <remarks>
    /// <para>A locator is a dictionary of keys to values, but it keeps the values with
    /// weak references, so that locating an object does not keep it alive. If you
    /// want to keep the object alive too, you should consider using an
    /// <see cref="ILifetimeContainer"/>.</para>
    /// <para>Locators have a built-in concept of hierarchy, so you can ask questions
    /// of a locator and tell it whether to return results from the current locator
    /// only, or whether to ask the parent locator when local lookups fail.</para>
    /// </remarks>
    // FxCop suppression: No
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public interface IReadableLocator : IEnumerable<KeyValuePair<object, object>>
    {
        /// <summary>
        /// Gets the number of items in the locator.
        /// </summary>
        /// <value>
        /// The number of items in the locator.
        /// </value>
        int Count { get; }

        /// <summary>
        /// Gets the parent locator.
        /// </summary>
        /// <value>
        /// The parent locator.
        /// </value>
        IReadableLocator ParentLocator { get; }

        /// <summary>
        /// Determines if the locator is read-only.
        /// </summary>
        /// <value>
        /// true if the locator is read-only; otherwise, false.
        /// </value>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "ReadOnly",
            Justification = "Back compat with ObjectBuilder")]
        bool ReadOnly { get; }

        /// <summary>
        /// Determine if the locator contains an object for the given key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        /// true if the locator contains an object for the key; returns
        /// false otherwise.
        /// </returns>
        bool Contains(object key);

        /// <summary>
        /// Finds objects in the locator using the predicate, and returns a temporary locator
        /// filled with the found objects.
        /// </summary>
        /// <param name="predicate">The predicate to test whether to include an object.</param>
        /// <returns>The new locator</returns>
        /// <exception cref="ArgumentNullException">Predicate is null.</exception>
        IReadableLocator FindBy(Predicate<KeyValuePair<object, object>> predicate);

        /// <summary>
        /// Gets an object from the locator, registered with the key of typeof(T).
        /// </summary>
        /// <typeparam name="TItem">The type of the object to find.</typeparam>
        /// <returns>The object, if found; null otherwise.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get",
            Justification = "Back compat with ObjectBuilder")]
        TItem Get<TItem>();

        /// <summary>
        /// Gets an object from the locator, registered with the given key.
        /// </summary>
        /// <typeparam name="TItem">The type of the object to find.</typeparam>
        /// <param name="key">The key that the object is registered with.</param>
        /// <returns>The object, if found; null otherwise.</returns>
        /// <exception cref="ArgumentNullException">Key is null.</exception>
        // FxCop Suppression: The whole point of the overloads is to vary by return type
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get"), SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType",
            Justification = "Back compat with ObjectBuilder")]
        TItem Get<TItem>(object key);


        /// <summary>
        /// Gets an object from the locator, registered with the given key.
        /// </summary>
        /// <param name="key">The key that the object is registered with.</param>
        /// <returns>The object, if found; null otherwise.</returns>
        /// <exception cref="ArgumentNullException">Key is null.</exception>
        // FxCop Suppression: The whole point of the overloads is to vary by return type
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get"), SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType",
            Justification = "Back compat with ObjectBuilder")]
        object Get(object key);
    }
}
