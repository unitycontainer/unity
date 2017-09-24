// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using System.Reflection;
using ObjectBuilder2;
using Unity.Utility;

namespace Unity.ObjectBuilder
{
    /// <summary>
    /// An implementation of <see cref="IConstructorSelectorPolicy"/> that is
    /// aware of the build keys used by the Unity container.
    /// </summary>
    public class DefaultUnityConstructorSelectorPolicy : ConstructorSelectorPolicyBase<InjectionConstructorAttribute>
    {
        /// <summary>
        /// Create a <see cref="IDependencyResolverPolicy"/> instance for the given
        /// <see cref="ParameterInfo"/>.
        /// </summary>
        /// <remarks>
        /// This implementation looks for the Unity <see cref="DependencyAttribute"/> on the
        /// parameter and uses it to create an instance of <see cref="NamedTypeDependencyResolverPolicy"/>
        /// for this parameter.</remarks>
        /// <param name="parameter">Parameter to create the resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected override IDependencyResolverPolicy CreateResolver(ParameterInfo parameter)
        {
            Guard.ArgumentNotNull(parameter, "parameter");

            // Resolve all DependencyAttributes on this parameter, if any
            var attr = parameter.GetCustomAttributes(false)?.OfType<DependencyResolutionAttribute>().FirstOrDefault();

            if (attr != null)
            {
                // Since this attribute is defined with MultipleUse = false, the compiler will
                // enforce at most one. So we don't need to check for more.
                return attr.CreateResolver(parameter.ParameterType);
            }

            // No attribute, just go back to the container for the default for that type.
            return new NamedTypeDependencyResolverPolicy(parameter.ParameterType, null);
        }
    }
}
