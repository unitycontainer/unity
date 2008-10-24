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

using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// This extension installs the default strategies and policies into the container
    /// to implement the standard behavior of the Unity container.
    /// </summary>
    public partial class UnityDefaultStrategiesExtension
    {
        /// <summary>
        /// Add the correct <see cref="IDynamicBuilderMethodCreatorPolicy"/> to the policy
        /// set. This version adds the appropriate policy for running on the desktop CLR.
        /// </summary>
        protected void SetDynamicBuilderMethodCreatorPolicy()
        {
            Context.Policies.SetDefault<IDynamicBuilderMethodCreatorPolicy>(
                new DefaultDynamicBuilderMethodCreatorPolicy());
        }
    }
}
