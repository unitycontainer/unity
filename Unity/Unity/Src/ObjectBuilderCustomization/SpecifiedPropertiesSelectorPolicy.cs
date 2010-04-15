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
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.ObjectBuilder
{
    /// <summary>
    /// An implemnetation of <see cref="IPropertySelectorPolicy"/> which returns
    /// the set of specific properties that the selector was configured with.
    /// </summary>
    public class SpecifiedPropertiesSelectorPolicy : IPropertySelectorPolicy
    {
        private readonly List<Pair<PropertyInfo, InjectionParameterValue>> propertiesAndValues = 
            new List<Pair<PropertyInfo, InjectionParameterValue>>();

        /// <summary>
        /// Add a property that will be par of the set returned when the 
        /// <see cref="SelectProperties(IBuilderContext, IPolicyList)"/> is called.
        /// </summary>
        /// <param name="property">The property to set.</param>
        /// <param name="value"><see cref="InjectionParameterValue"/> object describing
        /// how to create the value to inject.</param>
        public void AddPropertyAndValue(PropertyInfo property, InjectionParameterValue value)
        {
            propertiesAndValues.Add(Pair.Make(property, value));
        }

        /// <summary>
        /// Returns sequence of properties on the given type that
        /// should be set as part of building that object.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="resolverPolicyDestination">The <see cref='IPolicyList'/> to add any
        /// generated resolver objects into.</param>
        /// <returns>Sequence of <see cref="PropertyInfo"/> objects
        /// that contain the properties to set.</returns>
        public IEnumerable<SelectedProperty> SelectProperties(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            Type typeToBuild = context.BuildKey.Type;
            var currentTypeReflector = new ReflectionHelper(context.BuildKey.Type);
            foreach(Pair<PropertyInfo, InjectionParameterValue> pair in propertiesAndValues)
            {
                PropertyInfo currentProperty = pair.First;

                // Is this the property info on the open generic? If so, get the one
                // for the current closed generic.
                if (new ReflectionHelper(pair.First.DeclaringType).IsOpenGeneric)
                {
                    currentProperty = currentTypeReflector.Type.GetProperty(currentProperty.Name);
                }
                
                string key = Guid.NewGuid().ToString();
                resolverPolicyDestination.Set<IDependencyResolverPolicy>(pair.Second.GetResolverPolicy(typeToBuild), key);
                yield return new SelectedProperty(currentProperty, key);
            }
        }
    }
}
