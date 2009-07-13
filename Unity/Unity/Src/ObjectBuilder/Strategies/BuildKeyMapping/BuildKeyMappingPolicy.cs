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
    /// Represents a builder policy for mapping build keys.
    /// </summary>
	public class BuildKeyMappingPolicy : IBuildKeyMappingPolicy
	{
		readonly object newBuildKey;

        /// <summary>
        /// Initialize a new instance of the <see cref="BuildKeyMappingPolicy"/> with the new build key.
        /// </summary>
        /// <param name="newBuildKey"></param>
		public BuildKeyMappingPolicy(object newBuildKey)
		{
			this.newBuildKey = newBuildKey;
		}

        /// <summary>
        /// Map the <paramref name="buildKey"/> to a new build key.
        /// </summary>
        /// <param name="buildKey">The build key to mapl</param>
        /// <returns>The new build key.</returns>
        public object Map(object buildKey)
		{
			return newBuildKey;
		}
	}
}
