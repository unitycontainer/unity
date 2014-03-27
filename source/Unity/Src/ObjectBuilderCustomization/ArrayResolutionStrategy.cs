// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Guard = Microsoft.Practices.Unity.Utility.Guard;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// This strategy implements the logic that will call container.ResolveAll
    /// when an array parameter is detected.
    /// </summary>
    public class ArrayResolutionStrategy : BuilderStrategy
    {
        private delegate object ArrayResolver(IBuilderContext context);

        private static readonly MethodInfo genericResolveArrayMethod = typeof(ArrayResolutionStrategy)
                .GetTypeInfo().DeclaredMethods.First(m => m.Name == "ResolveArray" && m.IsPublic == false && m.IsStatic);

        /// <summary>
        /// Do the PreBuildUp stage of construction. This is where the actual work is performed.
        /// </summary>
        /// <param name="context">Current build context.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public override void PreBuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            Type typeToBuild = context.BuildKey.Type;
            if (typeToBuild.IsArray && typeToBuild.GetArrayRank() == 1)
            {
                Type elementType = typeToBuild.GetElementType();

                MethodInfo resolverMethod = genericResolveArrayMethod.MakeGenericMethod(elementType);

                ArrayResolver resolver = (ArrayResolver)resolverMethod.CreateDelegate(typeof(ArrayResolver));

                context.Existing = resolver(context);
                context.BuildComplete = true;
            }
        }

        private static object ResolveArray<T>(IBuilderContext context)
        {
            var registeredNamesPolicy = context.Policies.Get<IRegisteredNamesPolicy>(null);
            if (registeredNamesPolicy != null)
            {
                var registeredNames = registeredNamesPolicy.GetRegisteredNames(typeof(T));
                if (typeof(T).GetTypeInfo().IsGenericType)
                {
                    registeredNames = registeredNames.Concat(registeredNamesPolicy.GetRegisteredNames(typeof(T).GetGenericTypeDefinition()));
                }
                registeredNames = registeredNames.Distinct();

                return registeredNames.Select(n => context.NewBuildUp<T>(n)).ToArray();
            }

            return new T[0];
        }
    }
}
