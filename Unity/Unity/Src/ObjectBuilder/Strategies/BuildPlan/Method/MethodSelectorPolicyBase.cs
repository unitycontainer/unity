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
        /// <returns>Sequence of methods to call.</returns>
        public virtual IEnumerable<SelectedMethod> SelectMethods(IBuilderContext context)
        {
            Type t = context.BuildKey.Type;
            foreach(MethodInfo method in t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
            {
                if(method.IsDefined(typeof(TMarkerAttribute), false))
                {
                    yield return CreateSelectedMethod(context, method);
                }
            }
        }

        private SelectedMethod CreateSelectedMethod(IBuilderContext context, MethodInfo method)
        {
            SelectedMethod result = new SelectedMethod(method);
            foreach(ParameterInfo parameter in method.GetParameters())
            {
                string key = Guid.NewGuid().ToString();
                IDependencyResolverPolicy resolver = CreateResolver(parameter);
                context.PersistentPolicies.Set<IDependencyResolverPolicy>(resolver, key);
                DependencyResolverTrackerPolicy.TrackKey(context.PersistentPolicies,
                    context.BuildKey,
                    key);
                result.AddParameterKey(key);
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
