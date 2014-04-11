// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// A <see cref="InjectionParameterValue"/> that can be passed to
    /// <see cref="IUnityContainer.RegisterType"/> to configure a
    /// parameter or property as an optional dependency.
    /// </summary>
    public class OptionalParameter : TypedInjectionValue
    {
        private readonly string name;

        /// <summary>
        /// Construct a new <see cref="OptionalParameter"/> object that
        /// specifies the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type of the dependency.</param>
        public OptionalParameter(Type type)
            : this(type, null)
        {
        }

        /// <summary>
        /// Construct a new <see cref="OptionalParameter"/> object that
        /// specifies the given <paramref name="type"/> and <paramref name="name"/>.
        /// </summary>
        /// <param name="type">Type of the dependency.</param>
        /// <param name="name">Name for the dependency.</param>
        public OptionalParameter(Type type, string name)
            : base(type)
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

            Type typeToResolve = parameterReflector.Type;
            if (parameterReflector.IsOpenGeneric)
            {
                typeToResolve = parameterReflector.GetClosedParameterType(typeToBuild.GenericTypeArguments);
            }

            return new OptionalDependencyResolverPolicy(typeToResolve, this.name);
        }
    }

    /// <summary>
    /// A generic version of <see cref="OptionalParameter"></see> that lets you
    /// specify the type of the dependency using generics syntax.
    /// </summary>
    /// <typeparam name="T">Type of the dependency.</typeparam>
    public class OptionalParameter<T> : OptionalParameter
    {
        /// <summary>
        /// Construct a new <see cref="OptionalParameter{T}"/>.
        /// </summary>
        public OptionalParameter()
            : base(typeof(T))
        {
        }

        /// <summary>
        /// Construct a new <see cref="OptionalParameter{T}"/> with the given
        /// <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of the dependency.</param>
        public OptionalParameter(string name)
            : base(typeof(T), name)
        {
        }
    }
}
