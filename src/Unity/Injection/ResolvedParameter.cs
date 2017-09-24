// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using ObjectBuilder2;
using Unity.ObjectBuilder;
using Unity.Utility;
using Guard = Unity.Utility.Guard;

namespace Unity
{
    /// <summary>
    /// A class that stores a name and type, and generates a 
    /// resolver object that resolves the parameter via the
    /// container.
    /// </summary>
    public class ResolvedParameter : TypedInjectionValue
    {
        private readonly string name;

        /// <summary>
        /// Construct a new <see cref="ResolvedParameter"/> that
        /// resolves to the given type.
        /// </summary>
        /// <param name="parameterType">Type of this parameter.</param>
        public ResolvedParameter(Type parameterType)
            : this(parameterType, null)
        {
        }

        /// <summary>
        /// Construct a new <see cref="ResolvedParameter"/> that
        /// resolves the given type and name.
        /// </summary>
        /// <param name="parameterType">Type of this parameter.</param>
        /// <param name="name">Name to use when resolving parameter.</param>
        public ResolvedParameter(Type parameterType, string name)
            : base(parameterType)
        {
            this.name = name;
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
            Guard.ArgumentNotNull(typeToBuild, "typeToBuild");

            var parameterReflector = new ReflectionHelper(ParameterType);

            if (parameterReflector.IsGenericArray)
            {
                return this.CreateGenericArrayResolverPolicy(typeToBuild, parameterReflector);
            }

            if (parameterReflector.IsOpenGeneric || parameterReflector.Type.IsGenericParameter)
            {
                return this.CreateGenericResolverPolicy(typeToBuild, parameterReflector);
            }

            return this.CreateResolverPolicy(parameterReflector.Type);
        }

        private IDependencyResolverPolicy CreateResolverPolicy(Type typeToResolve)
        {
            return new NamedTypeDependencyResolverPolicy(typeToResolve, this.name);
        }

        private IDependencyResolverPolicy CreateGenericResolverPolicy(Type typeToBuild, ReflectionHelper parameterReflector)
        {
            return new NamedTypeDependencyResolverPolicy(
                parameterReflector.GetClosedParameterType(typeToBuild.GetTypeInfo().GenericTypeArguments),
                this.name);
        }

        private IDependencyResolverPolicy CreateGenericArrayResolverPolicy(Type typeToBuild, ReflectionHelper parameterReflector)
        {
            Type arrayType = parameterReflector.GetClosedParameterType(typeToBuild.GetTypeInfo().GenericTypeArguments);
            return new NamedTypeDependencyResolverPolicy(arrayType, this.name);
        }
    }

    /// <summary>
    /// A generic version of <see cref="ResolvedParameter"/> for convenience
    /// when creating them by hand.
    /// </summary>
    /// <typeparam name="TParameter">Type of the parameter</typeparam>
    public class ResolvedParameter<TParameter> : ResolvedParameter
    {
        /// <summary>
        /// Create a new <see cref="ResolvedParameter{TParameter}"/> for the given
        /// generic type and the default name.
        /// </summary>
        public ResolvedParameter()
            : base(typeof(TParameter))
        {
        }

        /// <summary>
        /// Create a new <see cref="ResolvedParameter{TParameter}"/> for the given
        /// generic type and name.
        /// </summary>
        /// <param name="name">Name to use to resolve this parameter.</param>
        public ResolvedParameter(string name)
            : base(typeof(TParameter), name)
        {
        }
    }
}