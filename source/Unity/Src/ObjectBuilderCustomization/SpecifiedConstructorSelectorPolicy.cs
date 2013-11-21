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
    /// An implementation of <see cref="IConstructorSelectorPolicy"/> that selects
    /// the given constructor and creates the appropriate resolvers to call it with
    /// the specified parameters.
    /// </summary>
    public class SpecifiedConstructorSelectorPolicy : IConstructorSelectorPolicy
    {
        private readonly ConstructorInfo ctor;
        private readonly MethodReflectionHelper ctorReflector;
        private readonly InjectionParameterValue[] parameterValues;

        /// <summary>
        /// Create an instance of <see cref="SpecifiedConstructorSelectorPolicy"/> that
        /// will return the given constructor, being passed the given injection values
        /// as parameters.
        /// </summary>
        /// <param name="ctor">The constructor to call.</param>
        /// <param name="parameterValues">Set of <see cref="InjectionParameterValue"/> objects
        /// that describes how to obtain the values for the constructor parameters.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "ctor")]
        public SpecifiedConstructorSelectorPolicy(ConstructorInfo ctor, InjectionParameterValue[] parameterValues)
        {
            this.ctor = ctor;
            ctorReflector = new MethodReflectionHelper(ctor);
            this.parameterValues = parameterValues;
        }

        /// <summary>
        /// Choose the constructor to call for the given type.
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <param name="resolverPolicyDestination">The <see cref='IPolicyList'/> to add any
        /// generated resolver objects into.</param>
        /// <returns>The chosen constructor.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
             Justification = "Validation done by Guard class")]
        public SelectedConstructor SelectConstructor(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(context, "context");

            SelectedConstructor result;
            Type typeToBuild = context.BuildKey.Type;

            ReflectionHelper typeReflector = new ReflectionHelper(ctor.DeclaringType);
            if (!ctorReflector.MethodHasOpenGenericParameters && !typeReflector.IsOpenGeneric)
            {
                result = new SelectedConstructor(ctor);
            }
            else
            {
                Type[] closedCtorParameterTypes =
                    ctorReflector.GetClosedParameterTypes(typeToBuild.GenericTypeArguments);
                result = new SelectedConstructor(typeToBuild.GetConstructor(closedCtorParameterTypes));
            }

            SpecifiedMemberSelectorHelper.AddParameterResolvers(typeToBuild, resolverPolicyDestination, parameterValues, result);
            return result;
        }
    }
}
