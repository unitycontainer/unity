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
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> class that lets you
    /// override a named parameter passed to a constructor.
    /// </summary>
    public class ParameterOverride : ResolverOverride
    {
        private readonly Type typeToConstruct;
        private readonly string parameterName;
        private readonly object parameterValue;

        /// <summary>
        /// Construct a new <see cref="ParameterOverride"/> object that will
        /// override the given named constructor parameter, and pass the given
        /// value.
        /// </summary>
        /// <param name="typeToConstruct">Type the constructor is on.</param>
        /// <param name="parameterName">Name of the constructor parameter.</param>
        /// <param name="parameterValue">Value to pass for the constructor.</param>
        public ParameterOverride(Type typeToConstruct, string parameterName, object parameterValue)
        {
            this.typeToConstruct = typeToConstruct;
            this.parameterName = parameterName;
            this.parameterValue = parameterValue;
        }

        /// <summary>
        /// Return a <see cref="IDependencyResolverPolicy"/> that can be used to give a value
        /// for the given desired dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="dependencyType">Type of dependency desired.</param>
        /// <returns>a <see cref="IDependencyResolverPolicy"/> object if this override applies, null if not.</returns>
        public override IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            var currentOperation = context.CurrentOperation as ConstructorArgumentResolveOperation;

            if (currentOperation != null &&
                currentOperation.TypeBeingConstructed == typeToConstruct &&
                currentOperation.ParameterName == parameterName)
            {
                return new LiteralValueDependencyResolverPolicy(parameterValue);
            }

            return null;
        }
    }

    /// <summary>
    /// A generic version of <see cref="ParameterOverride"/> that lets you specify
    /// the type the constructor is on as a generic parameter.
    /// </summary>
    /// <typeparam name="T">Type the constructor is on.</typeparam>
    public class ParameterOverride<T> : ParameterOverride
    {
        /// <summary>
        /// Construct a new <see cref="ParameterOverride"/> object that will
        /// override the given named constructor parameter, and pass the given
        /// value.
        /// </summary>
        /// <param name="parameterName">Name of the constructor parameter.</param>
        /// <param name="parameterValue">Value to pass for the constructor.</param>
        public ParameterOverride(string parameterName, object parameterValue)
            : base(typeof(T), parameterName, parameterValue)
        {
            
        }
    }
}
