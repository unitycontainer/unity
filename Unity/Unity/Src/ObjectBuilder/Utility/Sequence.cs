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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// A series of helper methods to deal with sequences -
    /// objects that implement <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class Sequence
    {

        /// <summary>
        /// A function that turns an arbitrary parameter list into an
        /// <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of arguments.</typeparam>
        /// <param name="arguments">The items to put into the collection.</param>
        /// <returns>An array that contains the values of the <paramref name="arguments"/>.</returns>
        public static T[] Collect<T>(params T[] arguments)
        {
            return arguments;
        }
    }
}
