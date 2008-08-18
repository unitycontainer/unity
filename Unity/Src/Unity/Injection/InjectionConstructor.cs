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
using System.Globalization;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.Unity.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// A class that holds the collection of information
    /// for a constructor, so that the container can
    /// be configured to call this constructor.
    /// </summary>
    public class InjectionConstructor : InjectionMember
    {
        private readonly List<InjectionParameterValue> parameterValues;

        /// <summary>
        /// Create a new instance of <see cref="InjectionConstructor"/> that looks
        /// for a constructor with the given set of parameters.
        /// </summary>
        /// <param name="parameterValues">The values for the parameters, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        public InjectionConstructor(params object[] parameterValues)
        {
            this.parameterValues = Sequence.ToList(InjectionParameterValue.ToParameters(parameterValues));
        }

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="typeToCreate">Type to register.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies(Type typeToCreate, string name, IPolicyList policies)
        {
            ConstructorInfo ctor = FindConstructor(typeToCreate);
            policies.Set<IConstructorSelectorPolicy>(
                new SpecifiedConstructorSelectorPolicy(ctor, parameterValues.ToArray()),
                new NamedTypeBuildKey(typeToCreate, name));
        }

        private ConstructorInfo FindConstructor(Type typeToCreate)
        {
            ParameterMatcher matcher = new ParameterMatcher(parameterValues);
            foreach(ConstructorInfo ctor in typeToCreate.GetConstructors())
            {
                if(matcher.Matches(ctor.GetParameters()))
                {
                    return ctor;
                }
            }

            string signature = Sequence.ToString(parameterValues,
                ", ",
                delegate(InjectionParameterValue parameter) { return parameter.ParameterTypeName; });

            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Resources.NoSuchConstructor,
                    typeToCreate.FullName,
                    signature));
        }
    }
}
