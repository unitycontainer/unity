// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// An implementation of <see cref="ResolverOverride"/> that
    /// acts as a decorator over another <see cref="ResolverOverride"/>.
    /// This checks to see if the current type being built is the
    /// right one before checking the inner <see cref="ResolverOverride"/>.
    /// </summary>
    public class TypeBasedOverride : ResolverOverride
    {
        private readonly Type targetType;
        private readonly ResolverOverride innerOverride;

        /// <summary>
        /// Create an instance of <see cref="TypeBasedOverride"/>
        /// </summary>
        /// <param name="targetType">Type to check for.</param>
        /// <param name="innerOverride">Inner override to check after type matches.</param>
        public TypeBasedOverride(Type targetType, ResolverOverride innerOverride)
        {
            Guard.ArgumentNotNull(targetType, "targetType");
            Guard.ArgumentNotNull(innerOverride, "innerOverride");

            this.targetType = targetType;
            this.innerOverride = innerOverride;
        }

        /// <summary>
        /// Return a <see cref="IDependencyResolverPolicy"/> that can be used to give a value
        /// for the given desired dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="dependencyType">Type of dependency desired.</param>
        /// <returns>a <see cref="IDependencyResolverPolicy"/> object if this override applies, null if not.</returns>
        public override IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            Guard.ArgumentNotNull(context, "context");

            var operation = context.CurrentOperation as BuildOperation;
            if (operation != null
                && operation.TypeBeingConstructed == this.targetType)
            {
                return innerOverride.GetResolver(context, dependencyType);
            }
            return null;
        }
    }

    /// <summary>
    /// A convenience version of <see cref="TypeBasedOverride"/> that lets you
    /// specify the type to construct via generics syntax.
    /// </summary>
    /// <typeparam name="T">Type to check for.</typeparam>
    public class TypeBasedOverride<T> : TypeBasedOverride
    {
        /// <summary>
        /// Create an instance of <see cref="TypeBasedOverride{T}"/>.
        /// </summary>
        /// <param name="innerOverride">Inner override to check after type matches.</param>
        public TypeBasedOverride(ResolverOverride innerOverride)
            : base(typeof(T), innerOverride)
        {
        }
    }
}
