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
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// An implementation of <see cref="IDependencyResolverPolicy"/> that
    /// calls back into the build chain to build up the dependency, passing
    /// a type given at compile time as its build key.
    /// </summary>
    public class FixedTypeResolverPolicy : IDependencyResolverPolicy
    {
        private Type typeToBuild;

        /// <summary>
        /// Create a new instance storing the given type.
        /// </summary>
        /// <param name="typeToBuild">Type to resolve.</param>
        public FixedTypeResolverPolicy(Type typeToBuild)
        {
            this.typeToBuild = typeToBuild;
        }

        #region IDependencyResolverPolicy Members

        /// <summary>
        /// Get the value for a dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>The value for the dependency.</returns>
        // FxCop suppression: Validation is done via Guard class
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public object Resolve(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            return context.NewBuildUp(typeToBuild);
        }

        #endregion
    }
}
