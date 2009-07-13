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
    /// A <see cref="IBuilderPolicy"/> that, when implemented,
    /// will determine which constructor to call from the build plan.
    /// </summary>
    public interface IConstructorSelectorPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Choose the constructor to call for the given type.
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>The chosen constructor.</returns>
        SelectedConstructor SelectConstructor(IBuilderContext context);
    }
}
