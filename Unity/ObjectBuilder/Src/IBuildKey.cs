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

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents a build key based on type.
    /// </summary>
	public interface IBuildKey
	{
        /// <summary>
        /// Gets the <see cref="Type"/> that represents the key.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> that represents the key.
        /// </value>
		Type Type { get; }

        /// <summary>
        /// Construct a new build key object with the current type
        /// replaced with the specified <paramref name="newType"/>.
        /// </summary>
        /// <remarks>This method creates a new build key object, the original is unchanged.</remarks>
        /// <param name="newType">New type to place in the build key.</param>
        /// <returns>The new build key.</returns>
        object ReplaceType(Type newType);
	}
}
