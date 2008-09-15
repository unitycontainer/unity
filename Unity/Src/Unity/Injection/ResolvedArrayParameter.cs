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
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.Unity.Properties;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// A class that stores a type, and generates a 
    /// resolver object that resolves all the named instances or the
    /// type registered in a container.
    /// </summary>
    public class ResolvedArrayParameter : TypedInjectionValue
    {
        private readonly Type elementType;
        private readonly List<InjectionParameterValue> elementValues = new List<InjectionParameterValue>();

        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter"/> that
        /// resolves to the given element type.
        /// </summary>
        /// <param name="elementType">The type of elements to resolve.</param>
        public ResolvedArrayParameter(Type elementType)
            : this(GetArrayType(elementType), elementType)
        {
        }

        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter"/> that
        /// resolves to the given array and element types.
        /// </summary>
        /// <param name="arrayParameterType">The type for the array of elements to resolve.</param>
        /// <param name="elementType">The type of elements to resolve.</param>
        protected ResolvedArrayParameter(Type arrayParameterType, Type elementType)
            : base(arrayParameterType)
        {
            Guard.ArgumentNotNull(elementType, "elementType");

            this.elementType = elementType;
        }

        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter"/> that
        /// resolves to the given element type and collection of element values.
        /// </summary>
        /// <param name="elementType">The type of elements to resolve.</param>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        public ResolvedArrayParameter(Type elementType, params object[] elementValues)
            : this(GetArrayType(elementType), elementType, elementValues)
        {
        }

        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter"/> that
        /// resolves to the given array and element types and collection of element values.
        /// </summary>
        /// <param name="arrayParameterType">The type for the array of elements to resolve.</param>
        /// <param name="elementType">The type of elements to resolve.</param>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        protected ResolvedArrayParameter(Type arrayParameterType, Type elementType, params object[] elementValues)
            : this(arrayParameterType, elementType)
        {
            Guard.ArgumentNotNull(elementValues, "elementValues");

            this.elementValues.AddRange(ToParameters(elementValues));
            foreach (InjectionParameterValue pv in this.elementValues)
            {
                if (!pv.MatchesType(elementType))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.TypesAreNotAssignable,
                            elementType,
                            pv.ParameterTypeName));
                }
            }
        }

        /// <summary>
        /// Return a <see cref="IDependencyResolverPolicy"/> instance that will
        /// return this types value for the parameter.
        /// </summary>
        /// <param name="typeToBuild">Type that contains the member that needs this parameter. Used
        /// to resolve open generic parameters.</param>
        /// <returns>The <see cref="IDependencyResolverPolicy"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done via Guard class")]
        public override IDependencyResolverPolicy GetResolverPolicy(Type typeToBuild)
        {
            Guard.ArgumentNotNull(typeToBuild, "typeToBuild");

            List<IDependencyResolverPolicy> resolverPolicies = new List<IDependencyResolverPolicy>();
            foreach (InjectionParameterValue pv in elementValues)
            {
                resolverPolicies.Add(pv.GetResolverPolicy(elementType));
            }
            return new ResolvedArrayWithElementsResolverPolicy(elementType, resolverPolicies.ToArray());
        }

        private static Type GetArrayType(Type elementType)
        {
            Guard.ArgumentNotNull(elementType, "elementType");

            return elementType.MakeArrayType();
        }
    }

    /// <summary>
    /// A generic version of <see cref="ResolvedArrayParameter"/> for convenience
    /// when creating them by hand.
    /// </summary>
    /// <typeparam name="TElement">Type of the elements for the array of the parameter.</typeparam>
    public class ResolvedArrayParameter<TElement> : ResolvedArrayParameter
    {
        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter{TElement}"/> that
        /// resolves to the given element generic type.
        /// </summary>
        public ResolvedArrayParameter()
            : base(typeof(TElement[]), typeof(TElement))
        { }

        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter{TElement}"/> that
        /// resolves to the given element generic type with the given element values.
        /// </summary>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        public ResolvedArrayParameter(params object[] elementValues)
            : base(typeof(TElement[]), typeof(TElement), elementValues)
        {
        }
    }
}
