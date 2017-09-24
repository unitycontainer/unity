// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using ObjectBuilder2;
using Unity.Properties;
using Unity.Utility;

namespace Unity
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
            this.elementValues.AddRange(InjectionParameterValue.ToParameters(elementValues));
        }

        /// <summary>
        /// Name for the type represented by this <see cref="InjectionParameterValue"/>.
        /// This may be an actual type name or a generic argument name.
        /// </summary>
        public override string ParameterTypeName
        {
            get { return this.genericParameterName + "[]"; }
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
        public override bool MatchesType(Type t)
        {
            Guard.ArgumentNotNull(t, "t");

            if (!t.IsArray || t.GetArrayRank() != 1)
            {
                return false;
            }

            Type elementType = t.GetElementType();
            return elementType.GetTypeInfo().IsGenericParameter && elementType.GetTypeInfo().Name == this.genericParameterName;
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
            this.GuardTypeToBuildIsGeneric(typeToBuild);
            this.GuardTypeToBuildHasMatchingGenericParameter(typeToBuild);

            Type typeToResolve = new ReflectionHelper(typeToBuild).GetNamedGenericParameter(this.genericParameterName);

            var resolverPolicies = new List<IDependencyResolverPolicy>();
            foreach (InjectionParameterValue pv in this.elementValues)
            {
                resolverPolicies.Add(pv.GetResolverPolicy(typeToBuild));
            }
            return new ResolvedArrayWithElementsResolverPolicy(typeToResolve, resolverPolicies.ToArray());
        }

        private void GuardTypeToBuildIsGeneric(Type typeToBuild)
        {
            if (!typeToBuild.GetTypeInfo().IsGenericType)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.NotAGenericType,
                        typeToBuild.GetTypeInfo().Name,
                        this.genericParameterName));
            }
        }

        private void GuardTypeToBuildHasMatchingGenericParameter(Type typeToBuild)
        {
            foreach (Type genericParam in typeToBuild.GetGenericTypeDefinition().GetTypeInfo().GenericTypeParameters)
            {
                if (genericParam.GetTypeInfo().Name == this.genericParameterName)
                {
                    return;
                }
            }

            throw new InvalidOperationException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.NoMatchingGenericArgument,
                    typeToBuild.GetTypeInfo().Name,
                    this.genericParameterName));
        }
    }
}
