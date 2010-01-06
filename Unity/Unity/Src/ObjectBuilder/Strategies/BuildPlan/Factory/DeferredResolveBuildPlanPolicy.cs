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
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.Unity;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Build plan for <see cref="Func{TResult}"/> that will
    /// return a func that will resolve the requested type
    /// through this container later.
    /// </summary>
    class DeferredResolveBuildPlanPolicy : IBuildPlanPolicy
    {
        public void BuildUp(IBuilderContext context)
        {
            if(context.Existing == null)
            {
                var currentContainer = context.NewBuildUp<IUnityContainer>();

                Type typeToBuild = GetTypeToBuild(context.BuildKey.Type);
                string nameToBuild = context.BuildKey.Name;

                Delegate resolveMethod;

                if(IsResolvingIEnumerable(typeToBuild))
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
            return t.GetGenericArguments()[0];
        }

        private static bool IsResolvingIEnumerable(Type typeToBuild)
        {
            return typeToBuild.IsGenericType &&
                typeToBuild.GetGenericTypeDefinition() == typeof (IEnumerable<>);
        }

        private static Delegate CreateResolver(IUnityContainer currentContainer, Type typeToBuild,
            string nameToBuild)
        {
            Type trampolineType = typeof (ResolveTrampoline<>).MakeGenericType(typeToBuild);
            Type delegateType = typeof (Func<>).MakeGenericType(typeToBuild);
            MethodInfo resolveMethod = trampolineType.GetMethod("Resolve");

            object trampoline = Activator.CreateInstance(trampolineType, currentContainer, nameToBuild);
            return Delegate.CreateDelegate(delegateType, trampoline, resolveMethod);
        }

        private static Delegate CreateResolveAllResolver(IUnityContainer currentContainer, Type enumerableType)
        {
            Type typeToBuild = GetTypeToBuild(enumerableType);

            Type trampolineType = typeof (ResolveAllTrampoline<>).MakeGenericType(typeToBuild);
            Type delegateType = typeof (Func<>).MakeGenericType(enumerableType);
            MethodInfo resolveAllMethod = trampolineType.GetMethod("ResolveAll");

            object trampoline = Activator.CreateInstance(trampolineType, currentContainer);
            return Delegate.CreateDelegate(delegateType, trampoline, resolveAllMethod);
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
