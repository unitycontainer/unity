// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents a builder policy for mapping build keys.
    /// </summary>
    public interface IBuildKeyMappingPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Maps the build key.
        /// </summary>
        /// <param name="buildKey">The build key to map.</param>
        /// <param name="context">Current build context. Used for contextual information
        /// if writing a more sophisticated mapping. This parameter can be null
        /// (called when getting container registrations).</param>
        /// <returns>The new build key.</returns>
        NamedTypeBuildKey Map(NamedTypeBuildKey buildKey, IBuilderContext context);
    }
}
