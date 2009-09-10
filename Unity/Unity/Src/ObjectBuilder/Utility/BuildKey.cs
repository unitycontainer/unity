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
using System.Globalization;
using Microsoft.Practices.Unity.Properties;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Utility methods for dealing with arbitrary build key objects.
    /// </summary>
    public static class BuildKey
    {
        /// <summary>
        /// Gets the <see cref="Type"/> of object to build from the build key.
        /// </summary>
        /// <param name="buildKey">The build key to get the <see cref="Type"/>.</param>
        /// <returns>The <see cref="Type"/> of object to build from the key.</returns>
        public static Type GetType(object buildKey)
        {
            Type result;

            if (!TryGetType(buildKey, out result))
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.CannotExtractTypeFromBuildKey,
                        buildKey),
                    "buildKey");

            return result;
        }

        /// <summary>
        /// Tries to get the <see cref="Type"/> from <paramref name="buildKey"/>.
        /// </summary>
        /// <param name="buildKey">The build key to get the <see cref="Type"/>.</param>
        /// <param name="type">When this method returns, contains the <see cref="Type"/> contained in 
        /// <paramref name="buildKey"/>, if the extraction succeeded, or <see langword="null"/> if the extration failed.
        /// </param>
        /// <returns>true if the <see cref="Type"/> was found; otherwise, false.</returns>
        public static bool TryGetType(object buildKey, out Type type)
        {
            type = buildKey as Type;

            if (type == null)
            {
                IBuildKey basedBuildKey = buildKey as IBuildKey;
                if (basedBuildKey != null)
                    type = basedBuildKey.Type;
            }

            return type != null;
        }

        /// <summary>
        /// Given a <paramref name="buildKey"/>, return a new build key
        /// which is the same as the original, except that the type has
        /// been replaced with <paramref name="newType"/>.
        /// </summary>
        /// <param name="buildKey">original build key</param>
        /// <param name="newType">New type to put in new build key.</param>
        /// <returns>The new build key.</returns>
        public static object ReplaceType(object buildKey, Type newType)
        {
            Type typeKey = buildKey as Type;
            if (typeKey != null)
            {
                return newType;
            }

            IBuildKey originalKey = buildKey as IBuildKey;
            if (originalKey != null)
            {
                return originalKey.ReplaceType(newType);
            }

            throw new ArgumentException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.CannotExtractTypeFromBuildKey,
                    buildKey),
                "buildKey");
        }
    }
}
