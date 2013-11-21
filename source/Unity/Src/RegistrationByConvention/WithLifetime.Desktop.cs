// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
