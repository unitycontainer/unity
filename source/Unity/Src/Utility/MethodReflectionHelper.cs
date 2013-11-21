// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Practices.Unity.Utility
{
    /// <summary>
    /// Helper class to wrap common reflection stuff dealing with
    /// methods.
    /// </summary>
    public class MethodReflectionHelper
    {
        private readonly MethodBase method;

        /// <summary>
        /// Create a new <see cref="MethodReflectionHelper"/> instance that
        /// lets us do more reflection stuff on that method.
        /// </summary>
        /// <param name="method">The method to reflect on.</param>
        public MethodReflectionHelper(MethodBase method)
        {
            this.method = method;
        }

        /// <summary>
        /// Returns true if any of the parameters of this method
        /// are open generics.
        /// </summary>
        public bool MethodHasOpenGenericParameters
        {
            get
            {
                return GetParameterReflectors().Any(r => r.IsOpenGeneric);
            }
        }

        /// <summary>
        /// Return the <see cref="System.Type"/> of each parameter for this
        /// method.
        /// </summary>
        /// <returns>Sequence of <see cref="System.Type"/> objects, one for
        /// each parameter in order.</returns>
        public IEnumerable<Type> ParameterTypes
        {
            get
            {
                foreach (ParameterInfo param in method.GetParameters())
                {
                    yield return param.ParameterType;
                }

            }
        }

        /// <summary>
        /// Given our set of generic type arguments, 
        /// </summary>
        /// <param name="genericTypeArguments">The generic type arguments.</param>
        /// <returns>An array with closed parameter types. </returns>
        public Type[] GetClosedParameterTypes(Type[] genericTypeArguments)
        {
            return GetClosedParameterTypesSequence(genericTypeArguments).ToArray();
        }

        private IEnumerable<ParameterReflectionHelper> GetParameterReflectors()
        {
            foreach (ParameterInfo pi in method.GetParameters())
            {
                yield return new ParameterReflectionHelper(pi);
            }
        }

        private IEnumerable<Type> GetClosedParameterTypesSequence(Type[] genericTypeArguments)
        {
            foreach (ParameterReflectionHelper r in GetParameterReflectors())
            {
                yield return r.GetClosedParameterType(genericTypeArguments);
            }
        }
    }
}
