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
using System.ComponentModel;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Configuration element class wrapping the lifetime element
    /// inside a type element.
    /// </summary>
    public class UnityLifetimeElement : InstanceDescriptionConfigurationElement
    {
        /// <summary>
        /// Create the lifetime manager instance configured in this section.
        /// </summary>
        /// <returns>The lifetime manager configured.</returns>
        public LifetimeManager CreateLifetimeManager()
        {
            return CreateInstance<LifetimeManager>();
        }
    }
}
