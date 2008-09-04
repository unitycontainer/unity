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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// An implementation of <see cref="IDependencyResolverPolicy"/> that resolves to
    /// all the named instances registered on an <see cref="IUnityContainer"/>.
    /// </summary>
    /// <see cref="IUnityContainer.ResolveAll"/>
    public class ResolvedArrayResolverPolicy : IDependencyResolverPolicy
    {
        private delegate object Resolver(IBuilderContext context);
        private readonly Resolver resolver;

        /// <summary>
        /// Create an instance of <see cref="ResolvedArrayResolverPolicy"/>
        /// with the given type.
        /// </summary>
        /// <param name="typeToResolve">The type.</param>
        public ResolvedArrayResolverPolicy(Type typeToResolve)
        {
            Guard.ArgumentNotNull(typeToResolve, "typeToResolve");

            MethodInfo resolverMethodInfo
                = typeof(ResolvedArrayResolverPolicy)
                    .GetMethod("DoResolve", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)
                        .MakeGenericMethod(typeToResolve);

            this.resolver = (Resolver)Delegate.CreateDelegate(typeof(Resolver), resolverMethodInfo);
        }

        /// <summary>
        /// Resolve the value for a dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>An array with all the named instances registered in the container available in <paramref name="context"/>.
        /// </returns>
        // FxCop suppression: Validation is done by Guard class
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public object Resolve(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            return this.resolver(context);
        }

        private static object DoResolve<T>(IBuilderContext context)
        {
            // either an instance will be available or an exception will be thrown, so the value cannot be null
            IUnityContainer container = ResolveContainer(context);

            List<T> values = new List<T>();
            values.AddRange(container.ResolveAll<T>());

            return values.ToArray();
        }

        private static IUnityContainer ResolveContainer(IBuilderContext context)
        {
            NamedTypeBuildKey containerKey = NamedTypeBuildKey.Make<IUnityContainer>();
            IBuilderContext clone = context.CloneForNewBuild(containerKey, null);
            clone.Strategies.ExecuteBuildUp(clone);
            return (IUnityContainer)clone.Existing;
        }
    }
}