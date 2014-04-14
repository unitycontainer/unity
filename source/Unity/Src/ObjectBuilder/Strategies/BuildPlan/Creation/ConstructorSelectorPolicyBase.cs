// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Base class that provides an implementation of <see cref="IConstructorSelectorPolicy"/>
    /// which lets you override how the parameter resolvers are created.
    /// </summary>
    public abstract class ConstructorSelectorPolicyBase<TInjectionConstructorMarkerAttribute> : IConstructorSelectorPolicy
        where TInjectionConstructorMarkerAttribute : Attribute
    {
        /// <summary>
        /// Choose the constructor to call for the given type.
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <param name="resolverPolicyDestination">The <see cref='IPolicyList'/> to add any
        /// generated resolver objects into.</param>
        /// <returns>The chosen constructor.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class")]
        public SelectedConstructor SelectConstructor(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            Guard.ArgumentNotNull(context, "context");
            Type typeToConstruct = context.BuildKey.Type;
            ConstructorInfo ctor = FindInjectionConstructor(typeToConstruct) ?? FindLongestConstructor(typeToConstruct);
            if (ctor != null)
            {
                return CreateSelectedConstructor(context, resolverPolicyDestination, ctor);
            }
            return null;
        }

        private SelectedConstructor CreateSelectedConstructor(IBuilderContext context, IPolicyList resolverPolicyDestination, ConstructorInfo ctor)
        {
            var result = new SelectedConstructor(ctor);

            foreach (ParameterInfo param in ctor.GetParameters())
            {
                result.AddParameterResolver(this.CreateResolver(param));
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

        private static ConstructorInfo FindInjectionConstructor(Type typeToConstruct)
        {
            ReflectionHelper typeToConstructReflector = new ReflectionHelper(typeToConstruct);

            ConstructorInfo[] injectionConstructors = typeToConstructReflector.InstanceConstructors
                                                        .Where(ctor => ctor.IsDefined(
                                                                                typeof(TInjectionConstructorMarkerAttribute),
                                                                                true))
                                                        .ToArray();
            switch (injectionConstructors.Length)
            {
                case 0:
                    return null;

                case 1:
                    return injectionConstructors[0];

                default:
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.MultipleInjectionConstructors,
                            typeToConstruct.GetTypeInfo().Name));
            }
        }

        private static ConstructorInfo FindLongestConstructor(Type typeToConstruct)
        {
            ReflectionHelper typeToConstructReflector = new ReflectionHelper(typeToConstruct);

            ConstructorInfo[] constructors = typeToConstructReflector.InstanceConstructors.ToArray();
            Array.Sort(constructors, new ConstructorLengthComparer());

            switch (constructors.Length)
            {
                case 0:
                    return null;

                case 1:
                    return constructors[0];

                default:
                    int paramLength = constructors[0].GetParameters().Length;
                    if (constructors[1].GetParameters().Length == paramLength)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                Resources.AmbiguousInjectionConstructor,
                                typeToConstruct.GetTypeInfo().Name,
                                paramLength));
                    }
                    return constructors[0];
            }
        }

        private class ConstructorLengthComparer : IComparer<ConstructorInfo>
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="y">The second object to compare.</param>
            /// <param name="x">The first object to compare.</param>
            /// <returns>
            /// Value Condition Less than zero is less than y. Zero equals y. Greater than zero is greater than y.
            /// </returns>
            [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class")]
            public int Compare(ConstructorInfo x, ConstructorInfo y)
            {
                Guard.ArgumentNotNull(x, "x");
                Guard.ArgumentNotNull(y, "y");

                return y.GetParameters().Length - x.GetParameters().Length;
            }
        }
    }
}
