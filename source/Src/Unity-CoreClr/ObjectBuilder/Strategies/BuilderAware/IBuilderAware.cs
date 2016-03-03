// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace ObjectBuilder2
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
        void OnBuiltUp(NamedTypeBuildKey buildKey);

        /// <summary>
        /// Called by the <see cref="BuilderAwareStrategy"/> when the object is being torn down.
        /// </summary>
        void OnTearingDown();
    }
}
