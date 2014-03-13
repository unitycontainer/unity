// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Base class that provides an implementation of <see cref="IMethodSelectorPolicy"/>
    /// which lets you override how the parameter resolvers are created.
    /// </summary>
    /// <typeparam name="TMarkerAttribute">Attribute that marks methods that should
    /// be called.</typeparam>
    public abstract class MethodSelectorPolicyBase<TMarkerAttribute> : IMethodSelectorPolicy
        where TMarkerAttribute : Attribute
    {
        /// <summary>
        /// Return the sequence of methods to call while building the target object.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="resolverPolicyDestination">The <see cref='IPolicyList'/> to add any
        /// generated resolver objects into.</param>
        /// <returns>Sequence of methods to call.</returns>
        public virtual IEnumerable<SelectedMethod> SelectMethods(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            Type t = context.BuildKey.Type;
            var candidateMethods = t.GetMethodsHierarchical()
                                    .Where(m => m.IsStatic == false && m.IsPublic);

            foreach (MethodInfo method in candidateMethods)
            {
                if (method.IsDefined(typeof(TMarkerAttribute), false))
                {
                    yield return CreateSelectedMethod(context, resolverPolicyDestination, method);
                }
            }
        }

        private SelectedMethod CreateSelectedMethod(IBuilderContext context, IPolicyList resolverPolicyDestination, MethodInfo method)
        {
            var result = new SelectedMethod(method);
            foreach (ParameterInfo parameter in method.GetParameters())
            {
                result.AddParameterResolver(this.CreateResolver(parameter));
            }

            return result;
        }

        /// <summary>
        /// Create a <see cref="IDependencyResolverPolicy"/> instance for the given
        /// <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="parameter">Parameter to create the resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected abstract IDependencyResolverPolicy CreateResolver(ParameterInfo parameter);
    }
}
