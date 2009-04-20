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

using System.Configuration;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Base class for configuration elements that use a <see cref="UnityTypeResolver"/>.
    /// </summary>
    public abstract class TypeResolvingConfigurationElement : ConfigurationElement, IResolvesTypeAliases
    {
        private UnityTypeResolver typeResolver;

        /// <summary>
        /// Get/set the Type Resolver
        /// </summary>
        public UnityTypeResolver TypeResolver
        {
            get { return this.typeResolver; }
            set { this.typeResolver = value; }
        }
    }
}
