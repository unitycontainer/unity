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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> that lets you override
    /// the value for a specified property.
    /// </summary>
    public class PropertyOverride : ResolverOverride
    {
        private readonly string propertyName;
        private readonly InjectionParameterValue propertyValue;

        ///<summary>
        /// Create an instance of <see cref="PropertyOverride"/>.
        ///</summary>
        ///<param name="propertyName">The property name.</param>
        ///<param name="propertyValue">Value to use for the property.</param>
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
            var currentOperation = context.CurrentOperation as ResolvingPropertyValueOperation;

            if(currentOperation != null
                && currentOperation.PropertyName == propertyName)
            {
                return propertyValue.GetResolverPolicy(dependencyType);   
            }
            return null;
        }
    }
}
