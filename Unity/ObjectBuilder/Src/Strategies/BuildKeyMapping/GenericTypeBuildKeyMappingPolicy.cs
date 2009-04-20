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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Practices.ObjectBuilder2.Properties;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// An implementation of <see cref="IBuildKeyMappingPolicy"/> that can map
    /// generic types.
    /// </summary>
    public class GenericTypeBuildKeyMappingPolicy : IBuildKeyMappingPolicy
    {
        private object destinationKey;

        /// <summary>
        /// Create a new <see cref="GenericTypeBuildKeyMappingPolicy"/> instance
        /// that will map generic types.
        /// </summary>
        /// <param name="destinationKey">Build key to map to. This must be or contain an open generic type.</param>
        // FxCop suppression: Validation is done by Guard class
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public GenericTypeBuildKeyMappingPolicy(object destinationKey)
        {
            if(!BuildKey.GetType(destinationKey).IsGenericTypeDefinition)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                                  Resources.MustHaveOpenGenericType,
                                  BuildKey.GetType(destinationKey).Name));
            }
            this.destinationKey = destinationKey;
        }

        /// <summary>
        /// Maps the build key.
        /// </summary>
        /// <param name="buildKey">The build key to map.</param>
        /// <returns>The new build key.</returns>
        public object Map(object buildKey)
        {
            Type originalType = BuildKey.GetType(buildKey);
            GuardSameNumberOfGenericArguments(originalType);

            Type[] genericArguments = originalType.GetGenericArguments();
            Type resultType = BuildKey.GetType(destinationKey).MakeGenericType(genericArguments);
            return BuildKey.ReplaceType(destinationKey, resultType);
        }

        private void GuardSameNumberOfGenericArguments(Type sourceType)
        {
            if(sourceType.GetGenericArguments().Length != DestinationType.GetGenericArguments().Length)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                                  Resources.MustHaveSameNumberOfGenericArguments,
                                  sourceType.Name, DestinationType.Name),
                    "sourceType");
            }
        }

        private Type DestinationType
        {
            get { return BuildKey.GetType(destinationKey); }
        }
    }
}
