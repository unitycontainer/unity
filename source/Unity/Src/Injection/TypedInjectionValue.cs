// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// A base class for implementing <see cref="InjectionParameterValue"/> classes
    /// that deal in explicit types.
    /// </summary>
    public abstract class TypedInjectionValue : InjectionParameterValue
    {
        private readonly ReflectionHelper parameterReflector;

        /// <summary>
        /// Create a new <see cref="TypedInjectionValue"/> that exposes
        /// information about the given <paramref name="parameterType"/>.
        /// </summary>
        /// <param name="parameterType">Type of the parameter.</param>
        protected TypedInjectionValue(Type parameterType)
        {
            this.parameterReflector = new ReflectionHelper(parameterType);
        }

        /// <summary>
        /// The type of parameter this object represents.
        /// </summary>
        public virtual Type ParameterType
        {
            get { return this.parameterReflector.Type; }
        }

        /// <summary>
        /// Name for the type represented by this <see cref="InjectionParameterValue"/>.
        /// This may be an actual type name or a generic argument name.
        /// </summary>
        public override string ParameterTypeName
        {
            get { return this.parameterReflector.Type.GetTypeInfo().Name; }
        }

        /// <summary>
        /// Test to see if this parameter value has a matching type for the given type.
        /// </summary>
        /// <param name="t">Type to check.</param>
        /// <returns>True if this parameter value is compatible with type <paramref name="t"/>,
        /// false if not.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class.")]
        public override bool MatchesType(Type t)
        {
            Guard.ArgumentNotNull(t, "t");
            ReflectionHelper candidateReflector = new ReflectionHelper(t);
            if (candidateReflector.IsOpenGeneric && this.parameterReflector.IsOpenGeneric)
            {
                return candidateReflector.Type.GetGenericTypeDefinition() ==
                       this.parameterReflector.Type.GetGenericTypeDefinition();
            }

            return t.GetTypeInfo().IsAssignableFrom(this.parameterReflector.Type.GetTypeInfo());
        }
    }
}
