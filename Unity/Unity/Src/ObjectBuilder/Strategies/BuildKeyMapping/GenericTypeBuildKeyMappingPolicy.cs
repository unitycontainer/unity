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
using Microsoft.Practices.Unity.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// An implementation of <see cref="IBuildKeyMappingPolicy"/> that can map
    /// generic types.
    /// </summary>
    public class GenericTypeBuildKeyMappingPolicy : IBuildKeyMappingPolicy
    {
        private readonly NamedTypeBuildKey destinationKey;

        /// <summary>
        /// Create a new <see cref="GenericTypeBuildKeyMappingPolicy"/> instance
        /// that will map generic types.
        /// </summary>
        /// <param name="destinationKey">Build key to map to. This must be or contain an open generic type.</param>
        // FxCop suppression: Validation is done by Guard class
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public GenericTypeBuildKeyMappingPolicy(NamedTypeBuildKey destinationKey)
        {
            Guard.ArgumentNotNull(destinationKey, "destinationKey");
            if(!destinationKey.Type.IsGenericTypeDefinition)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                                  Resources.MustHaveOpenGenericType,
                                  destinationKey.Type.Name));
            }
            this.destinationKey = destinationKey;
        }

        /// <summary>
        /// Maps the build key.
        /// </summary>
        /// <param name="buildKey">The build key to map.</param>
        /// <param name="context">Current build context. Used for contextual information
        /// if writing a more sophisticated mapping.</param>
        /// <returns>The new build key.</returns>
        public NamedTypeBuildKey Map(NamedTypeBuildKey buildKey, IBuilderContext context)
        {
            Guard.ArgumentNotNull(buildKey, "buildKey");
            Type originalType = buildKey.Type;
            GuardSameNumberOfGenericArguments(originalType);

            Type[] genericArguments = originalType.GetGenericArguments();
            Type resultType = destinationKey.Type.MakeGenericType(genericArguments);
            return new NamedTypeBuildKey(resultType, destinationKey.Name);
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
            get { return destinationKey.Type; }
        }
    }
}
