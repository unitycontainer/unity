// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using ObjectBuilder2;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.ObjectBuilder;

namespace Unity.Tests.TestObjects
{
    internal class MyUnityContainerSelectorPolicy : ConstructorSelectorPolicyBase<InjectionConstructorAttribute>
    {
        /// <summary>
        /// Create a <see cref="IDependencyResolverPolicy"/> instance for the given
        /// <see cref="ParameterInfo"/>.
        /// </summary>
        /// <remarks>
        /// This implementation looks for the Unity <see cref="DependencyAttribute"/> on the
        /// parameter and uses it to create an instance of <see cref="NamedTypeDependencyResolverPolicy"/>
        /// for this parameter.</remarks>
        /// <param name="param">Parameter to create the resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected override IDependencyResolverPolicy CreateResolver(ParameterInfo param)
        {
            // Resolve all DependencyAttributes on this parameter, if any
            List<DependencyResolutionAttribute> attrs = new List<DependencyResolutionAttribute>(
                from item in param.GetCustomAttributes(false)
                where item is DependencyResolutionAttribute
                select (DependencyResolutionAttribute)item);

            if (attrs.Count > 0)
            {
                // Since this attribute is defined with MultipleUse = false, the compiler will
                // enforce at most one. So we don't need to check for more.
                return attrs[0].CreateResolver(param.ParameterType);
            }

            // No attribute, just go back to the container for the default for that type.
            return new NamedTypeDependencyResolverPolicy(param.ParameterType, null);
        }
    }      
}
