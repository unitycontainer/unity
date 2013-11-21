// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// A <see cref="IBuilderPolicy"/> that, when implemented,
    /// will determine which constructor to call from the build plan.
    /// </summary>
    public interface IConstructorSelectorPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Choose the constructor to call for the given type.
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <param name="resolverPolicyDestination">The <see cref='IPolicyList'/> to add any
        /// generated resolver objects into.</param>
        /// <returns>The chosen constructor.</returns>
        SelectedConstructor SelectConstructor(IBuilderContext context, IPolicyList resolverPolicyDestination);
    }
}
