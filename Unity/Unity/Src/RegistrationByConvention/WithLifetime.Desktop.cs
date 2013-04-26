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

namespace Microsoft.Practices.Unity
{
    partial class WithLifetime
    {
        /// <summary>
        /// Returns a <see cref="PerThreadLifetimeManager"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A per thread lifetime manager.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "type", Justification = "Need to match signature Func<Type, string>")]
        public static LifetimeManager PerThread(Type type)
        {
            return new PerThreadLifetimeManager();
        }
    }
}
