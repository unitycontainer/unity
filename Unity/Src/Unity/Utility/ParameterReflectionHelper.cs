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
using System.Reflection;

namespace Microsoft.Practices.Unity.Utility
{
    /// <summary>
    /// Another reflection helper class that has extra methods
    /// for dealing with ParameterInfos.
    /// </summary>
    public class ParameterReflectionHelper : ReflectionHelper
    {
        /// <summary>
        /// Create a new instance of <see cref="ParameterReflectionHelper"/> that
        /// lets you query information about the given ParameterInfo object.
        /// </summary>
        /// <param name="parameter">Parameter to query.</param>
        public ParameterReflectionHelper(ParameterInfo parameter) :
            base(TypeFromParameterInfo(parameter))
        {
        }

        // Helper method to validate parameter so FxCop will shut up.
        private static Type TypeFromParameterInfo(ParameterInfo parameter)
        {
            Guard.ArgumentNotNull(parameter, "parameter");
            return parameter.ParameterType;
        }
    }
}
