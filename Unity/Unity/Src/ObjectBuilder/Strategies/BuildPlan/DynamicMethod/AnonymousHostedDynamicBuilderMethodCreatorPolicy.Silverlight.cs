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
    /// Create a <see cref="DynamicMethod"/> object used to host the
    /// dynamically generated build plan. This class creates the
    /// dynamic method in the anonymous hosting assembly provided by
    /// the Silverlight runtime.
    /// </summary>
    public class AnonymousHostedDynamicBuilderMethodCreatorPolicy : IDynamicBuilderMethodCreatorPolicy
    {
        #region IDynamicBuilderMethodCreatorPolicy Members

        /// <summary>
        /// Create a builder method for the given type, using the given name.
        /// </summary>
        /// <param name="typeToBuild">Type that will be built by the generated method.</param>
        /// <param name="methodName">Name to give to the method.</param>
        /// <returns>A <see cref="DynamicMethod"/> object with the proper signature to use
        /// as part of a build plan.</returns>
        public DynamicMethod CreateBuilderMethod(Type typeToBuild, string methodName)
        {
            return new DynamicMethod(methodName, typeof (void), new[] {typeof (IBuilderContext)});
        }

        #endregion
    }
}
