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
    /// A <see cref="IMethodSelectorPolicy"/> implementation that calls the specific
    /// methods with the given parameters.
    /// </summary>
    public class SpecifiedMethodsSelectorPolicy : IMethodSelectorPolicy
    {
        private readonly List<Pair<MethodInfo, IEnumerable<InjectionParameterValue>>> methods =
            new List<Pair<MethodInfo, IEnumerable<InjectionParameterValue>>>();

        /// <summary>
        /// Add the given method and parameter collection to the list of methods
        /// that will be returned when the selector's <see cref="IMethodSelectorPolicy.SelectMethods"/>
        /// method is called.
        /// </summary>
        /// <param name="method">Method to call.</param>
        /// <param name="parameters">sequence of <see cref="InjectionParameterValue"/> objects
        /// that describe how to create the method parameter values.</param>
        public void AddMethodAndParameters(MethodInfo method, IEnumerable<InjectionParameterValue> parameters)
        {
            methods.Add(Pair.Make(method, parameters));
        }

        /// <summary>
        /// Return the sequence of methods to call while building the target object.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="resolverPolicyDestination">The <see cref='IPolicyList'/> to add any
        /// generated resolver objects into.</param>
        /// <returns>Sequence of methods to call.</returns>
        public IEnumerable<SelectedMethod> SelectMethods(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            foreach(Pair<MethodInfo, IEnumerable<InjectionParameterValue>> method in methods)
            {
                Type typeToBuild = context.BuildKey.Type;
                SelectedMethod selectedMethod;
                ReflectionHelper typeReflector = new ReflectionHelper(method.First.DeclaringType);
                MethodReflectionHelper methodReflector = new MethodReflectionHelper(method.First);
                if (!methodReflector.MethodHasOpenGenericParameters && !typeReflector.IsOpenGeneric)
                {
                    selectedMethod = new SelectedMethod(method.First);
                }
                else
                {
                    Type[] closedMethodParameterTypes =
                        methodReflector.GetClosedParameterTypes(typeToBuild.GetTypeInfo().GenericTypeArguments);
                    selectedMethod = new SelectedMethod(
                        typeToBuild.GetMethodHierarchical(method.First.Name, closedMethodParameterTypes));
                }

                SpecifiedMemberSelectorHelper.AddParameterResolvers(
                        typeToBuild,
                        resolverPolicyDestination, 
                        method.Second,
                        selectedMethod);
                yield return selectedMethod;
            }
        }
    }
}
