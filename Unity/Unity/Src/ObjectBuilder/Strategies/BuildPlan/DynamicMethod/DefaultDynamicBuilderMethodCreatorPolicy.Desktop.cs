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
using System.Reflection.Emit;
using System.Security;
using System.Security.Permissions;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// An implementation of <see cref="IDynamicBuilderMethodCreatorPolicy"/> that will
    /// check for full trust and if we're building a class or an interface. If in full
    /// trust, attach to the class or module of the interface respectively. If in partial
    /// trust, attach to the OB2 module instead.
    /// </summary>
    public class DefaultDynamicBuilderMethodCreatorPolicy : IDynamicBuilderMethodCreatorPolicy
    {
        /// <summary>
        /// Create a builder method for the given type, using the given name.
        /// </summary>
        /// <param name="typeToBuild">Type that will be built by the generated method.</param>
        /// <param name="methodName">Name to give to the method.</param>
        /// <returns>A <see cref="DynamicMethod"/> object with the proper signature to use
        /// as part of a build plan.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification="Validation done by Guard class")]
        public DynamicMethod CreateBuilderMethod(Type typeToBuild, string methodName)
        {
            Guard.ArgumentNotNull(typeToBuild, "typeToBuild");
            Guard.ArgumentNotNullOrEmpty(methodName, "methodName");

            // Check for full trust. We can't add the method to the
            // built up type without it.

            try
            {
                PermissionSet fullTrust = new PermissionSet(PermissionState.Unrestricted);
                fullTrust.Demand();

                return CreateMethodOnModule(typeToBuild.Module, methodName);
            }
            catch (SecurityException)
            {
                // Not in full trust, add IL to this module instead.
                return CreateMethodOnModule(GetType().Module, methodName);
            }
        }

        private static DynamicMethod CreateMethodOnModule(Module module, string methodName)
        {
            return new DynamicMethod(methodName,
                typeof(void),
                new Type[] { typeof(IBuilderContext) },
                module);
        }
    }
}
