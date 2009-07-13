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
using System.Reflection.Emit;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// This interface defines a policy that manages creation of the dynamic methods
    /// used by the ObjectBuilder code generation. This way, we can replace the details
    /// of how the dynamic method is created to handle differences in CLR (like Silverlight
    /// vs desktop) or security policies.
    /// </summary>
    public interface IDynamicBuilderMethodCreatorPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Create a builder method for the given type, using the given name.
        /// </summary>
        /// <param name="typeToBuild">Type that will be built by the generated method.</param>
        /// <param name="methodName">Name to give to the method.</param>
        /// <returns>A <see cref="DynamicMethod"/> object with the proper signature to use
        /// as part of a build plan.</returns>
        DynamicMethod CreateBuilderMethod(Type typeToBuild, string methodName);
    }
}
