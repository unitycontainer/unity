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
using System.Globalization;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.Properties;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// This attribute is used to mark properties and parameters as targets for array injection.
    /// </summary>
    /// <remarks>
    /// Discrete array elements cannot be specified using attributes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class DependencyArrayAttribute : DependencyResolutionAttribute
    {
        /// <summary>
        /// Create an instance of <see cref="IDependencyResolverPolicy"/> that
        /// will be used to get the value for the member this attribute is
        /// applied to.
        /// </summary>
        /// <param name="typeToResolve">Type of parameter or property that
        /// this attribute is decoration.</param>
        /// <returns>The resolver object.</returns>
        /// <throws><see cref="ArgumentException"/> when <paramref name="typeToResolve"/> is not 
        /// an array type with rank one.</throws>
        public override IDependencyResolverPolicy CreateResolver(Type typeToResolve)
        {
            if (!typeToResolve.IsArray || typeToResolve.GetArrayRank() > 1)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.NotAnArrayTypeWithRankOne,
                        typeToResolve.Name),
                    "typeToResolve");
            }

            // guaranteed to be not null: this is an array type with rank 1
            Type elementType = typeToResolve.GetElementType();

            return new ResolvedArrayResolverPolicy(elementType);
        }
    }
}
