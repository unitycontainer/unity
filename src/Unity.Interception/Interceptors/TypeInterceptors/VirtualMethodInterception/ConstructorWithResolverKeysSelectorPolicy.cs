// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using ObjectBuilder2;

namespace Unity.InterceptionExtension
{
    /// <summary>
    /// A small implementation of <see cref="IConstructorSelectorPolicy"/> that returns the
    /// given <see cref="SelectedConstructor"/> object.
    /// </summary>
    public class ConstructorWithResolverKeysSelectorPolicy : IConstructorSelectorPolicy
    {
        private readonly SelectedConstructor selectedConstructor;

        /// <summary>
        /// Create a new <see cref="ConstructorWithResolverKeysSelectorPolicy"/> instance.
        /// </summary>
        /// <param name="selectedConstructor">Information about which constructor to select.</param>
        public ConstructorWithResolverKeysSelectorPolicy(SelectedConstructor selectedConstructor)
        {
            this.selectedConstructor = selectedConstructor;
        }

        /// <summary>
        /// Choose the constructor to call for the given type.
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <param name="resolverPolicyDestination">The <see cref='IPolicyList'/> to add any
        /// generated resolver objects into.</param>
        /// <returns>The chosen constructor.</returns>
        public SelectedConstructor SelectConstructor(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            return selectedConstructor;
        }
    }
}
