// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// Provides helper methods to get type names.
    /// </summary>
    public static class WithName
    {
        /// <summary>
        /// Returns the type name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type name.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Need to match signature Func<Type, string>")]
        public static string TypeName(Type type)
        {
            Guard.ArgumentNotNull(type, "type");

            return type.Name;
        }

        /// <summary>
        /// Returns null for the registration name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><see langword="null"/></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "type", Justification = "Need to match signature Func<Type, string>")]
        public static string Default(Type type)
        {
            return null;
        }
    }
}
