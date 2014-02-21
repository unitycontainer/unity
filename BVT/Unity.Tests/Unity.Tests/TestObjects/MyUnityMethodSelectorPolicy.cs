// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using System.Collections.Generic;

namespace Unity.Tests.TestObjects
{
    internal class MyUnityMethodSelectorPolicy : MethodSelectorPolicyBase<InjectionMethodAttribute>
    {
        /// <summary>
        /// Create a <see cref="IDependencyResolverPolicy"/> instance for the given
        /// <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="parameter">Parameter to create the resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected override IDependencyResolverPolicy CreateResolver(ParameterInfo parameter)
        {
            List<DependencyResolutionAttribute> attributes = new List<DependencyResolutionAttribute>(
                from item in parameter.GetCustomAttributes(false)
                where item is DependencyResolutionAttribute
                select (DependencyResolutionAttribute)item);

            if (attributes.Count > 0)
            {
                return attributes[0].CreateResolver(parameter.ParameterType);
            }

            return new NamedTypeDependencyResolverPolicy(parameter.ParameterType, null);
        }

        public override IEnumerable<SelectedMethod> SelectMethods(IBuilderContext context, IPolicyList list)
        {
            return base.SelectMethods(context, list);
        }
    }
}
