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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.Unity.Utility;
using Microsoft.Practices.Unity.Properties;
using Guard = Microsoft.Practices.Unity.Utility.Guard;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// A <see cref="InjectionParameterValue"/> that lets you specify that
    /// an array containing the registered instances of a generic type parameter 
    /// should be resolved.
    /// </summary>
    public class GenericResolvedArrayParameter : InjectionParameterValue
    {
        private readonly string genericParameterName;
        private readonly List<InjectionParameterValue> elementValues = new List<InjectionParameterValue>();

        /// <summary>
        /// Create a new <see cref="GenericResolvedArrayParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        public GenericResolvedArrayParameter(string genericParameterName, params object[] elementValues)
        {
            Guard.ArgumentNotNull(genericParameterName, "genericParameterName");

            this.genericParameterName = genericParameterName;
            this.elementValues.AddRange(ToParameters(elementValues));
        }

        /// <summary>
        /// Name for the type represented by this <see cref="InjectionParameterValue"/>.
        /// This may be an actual type name or a generic argument name.
        /// </summary>
        public override string ParameterTypeName
        {
            get { return genericParameterName + "[]"; }
        }

        /// <summary>
        /// Test to see if this parameter value has a matching type for the given type.
        /// </summary>
        /// <param name="t">Type to check.</param>
        /// <returns>True if this parameter value is compatible with type <paramref name="t"/>,
        /// false if not.</returns>
        /// <remarks>A type is considered compatible if it is an array type of rank one
        /// and its element type is a generic type parameter with a name matching this generic
        /// parameter name configured for the receiver.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public override bool MatchesType(Type t)
        {
            Guard.ArgumentNotNull(t, "t");

            if (!t.IsArray || t.GetArrayRank() != 1)
                return false;

            Type elementType = t.GetElementType();
            return elementType.IsGenericParameter && elementType.Name == genericParameterName;
        }

        /// <summary>
        /// Return a <see cref="IDependencyResolverPolicy"/> instance that will
        /// return this types value for the parameter.
        /// </summary>
        /// <param name="typeToBuild">Type that contains the member that needs this parameter. Used
        /// to resolve open generic parameters.</param>
        /// <returns>The <see cref="IDependencyResolverPolicy"/>.</returns>
        public override IDependencyResolverPolicy GetResolverPolicy(Type typeToBuild)
        {
            GuardTypeToBuildIsGeneric(typeToBuild);
            GuardTypeToBuildHasMatchingGenericParameter(typeToBuild);

            Type typeToResolve = new ReflectionHelper(typeToBuild).GetNamedGenericParameter(genericParameterName);

            if (this.elementValues.Count == 0)
            {
                return new NamedTypeDependencyResolverPolicy(typeToResolve.MakeArrayType(), null);
            }
            else
            {
                List<IDependencyResolverPolicy> resolverPolicies = new List<IDependencyResolverPolicy>();
                foreach (InjectionParameterValue pv in elementValues)
                {
                    resolverPolicies.Add(pv.GetResolverPolicy(typeToBuild));
                }
                return new ResolvedArrayWithElementsResolverPolicy(typeToResolve, resolverPolicies.ToArray());
            }
        }

        private void GuardTypeToBuildIsGeneric(Type typeToBuild)
        {
            if (!typeToBuild.IsGenericType)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.NotAGenericType,
                        typeToBuild.Name,
                        genericParameterName));
            }
        }

        private void GuardTypeToBuildHasMatchingGenericParameter(Type typeToBuild)
        {
            foreach (Type genericParam in typeToBuild.GetGenericTypeDefinition().GetGenericArguments())
            {
                if (genericParam.Name == genericParameterName)
                {
                    return;
                }
            }

            throw new InvalidOperationException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.NoMatchingGenericArgument,
                    typeToBuild.Name,
                    genericParameterName));
        }
    }
}
