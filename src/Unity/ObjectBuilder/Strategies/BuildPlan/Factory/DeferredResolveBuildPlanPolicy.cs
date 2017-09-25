// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Unity;
using Unity.Utility;

namespace ObjectBuilder2
{
    /// <summary>
    /// Build plan for <see cref="Func{TResult}"/> that will return a Func that will resolve the requested type
    /// through this container later.
    /// </summary>
    internal class DeferredResolveBuildPlanPolicy : IBuildPlanPolicy
    {
        public void BuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            if (context.Existing == null)
            {
                var currentContainer = context.Container ?? context.NewBuildUp<IUnityContainer>();

                Type typeToBuild = GetTypeToBuild(context.BuildKey.Type);
                string nameToBuild = context.BuildKey.Name;

                Delegate resolveMethod;

                if (IsResolvingIEnumerable(typeToBuild))
                {
                    resolveMethod = CreateResolveAllResolver(currentContainer, typeToBuild);
                }
                else
                {
                    resolveMethod = CreateResolver(currentContainer, typeToBuild, nameToBuild);
                }

                context.Existing = resolveMethod;

                DynamicMethodConstructorStrategy.SetPerBuildSingleton(context);
            }
        }

        private static Type GetTypeToBuild(Type t)
        {
            return t.GetTypeInfo().GenericTypeArguments[0];
        }

        private static bool IsResolvingIEnumerable(Type typeToBuild)
        {
            return typeToBuild.GetTypeInfo().IsGenericType &&
                typeToBuild.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static Delegate CreateResolver(IUnityContainer currentContainer, Type typeToBuild,
            string nameToBuild)
        {
            Type trampolineType = typeof(ResolveTrampoline<>).MakeGenericType(typeToBuild);
            Type delegateType = typeof(Func<>).MakeGenericType(typeToBuild);
            MethodInfo resolveMethod = trampolineType.GetTypeInfo().GetDeclaredMethod("Resolve");

            object trampoline = Activator.CreateInstance(trampolineType, currentContainer, nameToBuild);
            return resolveMethod.CreateDelegate(delegateType, trampoline);
        }

        private static Delegate CreateResolveAllResolver(IUnityContainer currentContainer, Type enumerableType)
        {
            Type typeToBuild = GetTypeToBuild(enumerableType);

            Type trampolineType = typeof(ResolveAllTrampoline<>).MakeGenericType(typeToBuild);
            Type delegateType = typeof(Func<>).MakeGenericType(enumerableType);
            MethodInfo resolveAllMethod = trampolineType.GetTypeInfo().GetDeclaredMethod("ResolveAll");

            object trampoline = Activator.CreateInstance(trampolineType, currentContainer);
            return resolveAllMethod.CreateDelegate(delegateType, trampoline);
        }

        private class ResolveTrampoline<TItem>
        {
            private readonly IUnityContainer container;
            private readonly string name;

            public ResolveTrampoline(IUnityContainer container, string name)
            {
                this.container = container;
                this.name = name;
            }

            public TItem Resolve()
            {
                return container.Resolve<TItem>(name);
            }
        }

        private class ResolveAllTrampoline<TItem>
        {
            private readonly IUnityContainer container;

            public ResolveAllTrampoline(IUnityContainer container)
            {
                this.container = container;
            }

            public IEnumerable<TItem> ResolveAll()
            {
                return container.ResolveAll<TItem>();
            }
        }
    }
}
