// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
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
            if (!destinationKey.Type.GetTypeInfo().IsGenericTypeDefinition)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                                  Resources.MustHaveOpenGenericType,
                                  destinationKey.Type.GetTypeInfo().Name));
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

            var originalTypeInfo = buildKey.Type.GetTypeInfo();
            if (originalTypeInfo.IsGenericTypeDefinition)
            {
                // No need to perform a mapping - the source type is an open generic
                return this.destinationKey;
            }

            this.GuardSameNumberOfGenericArguments(originalTypeInfo);
            Type[] genericArguments = originalTypeInfo.GenericTypeArguments;
            Type resultType = this.destinationKey.Type.MakeGenericType(genericArguments);
            return new NamedTypeBuildKey(resultType, this.destinationKey.Name);
        }

        private void GuardSameNumberOfGenericArguments(TypeInfo sourceTypeInfo)
        {
            if (sourceTypeInfo.GenericTypeArguments.Length != this.DestinationType.GetTypeInfo().GenericTypeParameters.Length)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                                  Resources.MustHaveSameNumberOfGenericArguments,
                                  sourceTypeInfo.Name, this.DestinationType.Name),
                    "sourceTypeInfo");
            }
        }

        private Type DestinationType
        {
            get { return destinationKey.Type; }
        }
    }
}
