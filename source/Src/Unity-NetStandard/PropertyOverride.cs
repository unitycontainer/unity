﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using ObjectBuilder2;
using Unity.Utility;

namespace Unity
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> that lets you override
    /// the value for a specified property.
    /// </summary>
    public class PropertyOverride : ResolverOverride
    {
        private readonly string propertyName;
        private readonly InjectionParameterValue propertyValue;

        /// <summary>
        /// Create an instance of <see cref="PropertyOverride"/>.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyValue">Value to use for the property.</param>
        public PropertyOverride(string propertyName, object propertyValue)
        {
            this.propertyName = propertyName;
            this.propertyValue = InjectionParameterValue.ToParameter(propertyValue);
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
            var currentOperation = context.CurrentOperation as ResolvingPropertyValueOperation;

            if (currentOperation != null
                && currentOperation.PropertyName == this.propertyName)
            {
                return this.propertyValue.GetResolverPolicy(dependencyType);
            }
            return null;
        }
    }
}
