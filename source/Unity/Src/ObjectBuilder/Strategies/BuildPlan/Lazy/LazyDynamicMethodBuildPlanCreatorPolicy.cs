// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// An <see cref="IBuildPlanCreatorPolicy"/> implementation
    /// that constructs a build plan for creating <see cref="Lazy{T}"/> objects.
    /// </summary>
    public class LazyDynamicMethodBuildPlanCreatorPolicy : IBuildPlanCreatorPolicy
    {
        private static readonly MethodInfo BuildResolveLazyMethod = StaticReflection.GetMethodInfo(() => LazyDynamicMethodBuildPlanCreatorPolicy.BuildResolveLazy<object>(null)).GetGenericMethodDefinition();
        private static readonly MethodInfo BuildResolveAllLazyMethod = StaticReflection.GetMethodInfo(() => LazyDynamicMethodBuildPlanCreatorPolicy.BuildResolveAllLazy<object>(null)).GetGenericMethodDefinition();

        /// <summary>
        /// Creates a build plan using the given context and build key.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="buildKey">Current build key.</param>
        /// <returns>
        /// The build plan.
        /// </returns>
        public IBuildPlanPolicy CreatePlan(IBuilderContext context, NamedTypeBuildKey buildKey)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(buildKey, "buildKey");

            return new DynamicMethodBuildPlan(CreateBuildPlanMethod(buildKey.Type));
        }

        private static DynamicBuildPlanMethod CreateBuildPlanMethod(Type lazyType)
        {
            var itemType = lazyType.GetTypeInfo().GenericTypeArguments[0];

            MethodInfo lazyMethod;

            if (itemType.GetTypeInfo().IsGenericType && itemType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                lazyMethod = BuildResolveAllLazyMethod.MakeGenericMethod(itemType.GetTypeInfo().GenericTypeArguments[0]);
            }
            else
            {
                lazyMethod = BuildResolveLazyMethod.MakeGenericMethod(itemType);
            }

            return (DynamicBuildPlanMethod)lazyMethod.CreateDelegate(typeof(DynamicBuildPlanMethod));
        }

        private static void BuildResolveLazy<T>(IBuilderContext context)
        {
            if (context.Existing == null)
            {
                var name = context.BuildKey.Name;
                var container = context.NewBuildUp<IUnityContainer>();
                context.Existing = new Lazy<T>(() => container.Resolve<T>(name));
            }

            // match the behavior of DynamicMethodConstructorStrategy
            DynamicMethodConstructorStrategy.SetPerBuildSingleton(context);
        }

        private static void BuildResolveAllLazy<T>(IBuilderContext context)
        {
            if (context.Existing == null)
            {
                var container = context.NewBuildUp<IUnityContainer>();
                context.Existing = new Lazy<IEnumerable<T>>(() => container.ResolveAll<T>());
            }

            // match the behavior of DynamicMethodConstructorStrategy
            DynamicMethodConstructorStrategy.SetPerBuildSingleton(context);
        }
    }
}
