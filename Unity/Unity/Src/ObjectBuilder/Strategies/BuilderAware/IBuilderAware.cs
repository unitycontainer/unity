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
    /// Implemented on a class when it wants to receive notifications
    /// about the build process.
    /// </summary>
	public interface IBuilderAware
	{
        /// <summary>
        /// Called by the <see cref="BuilderAwareStrategy"/> when the object is being built up.
        /// </summary>
        /// <param name="buildKey">The key of the object that was just built up.</param>
		void OnBuiltUp(object buildKey);

        /// <summary>
        /// Called by the <see cref="BuilderAwareStrategy"/> when the object is being torn down.
        /// </summary>
		void OnTearingDown();
	}
}
