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

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Interface defining the contract for classes that
    /// use a <see cref="UnityTypeResolver"/> to resolve
    /// type aliases.
    /// </summary>
    public interface IResolvesTypeAliases
    {
        /// <summary>
        /// Get or set the type resolver.
        /// </summary>
        UnityTypeResolver TypeResolver { get; set; }
    }
}
