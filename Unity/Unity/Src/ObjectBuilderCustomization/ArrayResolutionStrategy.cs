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
        private static readonly MethodInfo genericResolveArrayMethod = typeof (ArrayResolutionStrategy)
                .GetMethod("ResolveArray", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        /// <summary>
        /// Do the PreBuildUp stage of construction. This is where the actual work is performed.
        /// </summary>
        /// <param name="context">Current build context.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification="Validation done by Guard class")]
        public override void PreBuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            Type typeToBuild = BuildKey.GetType(context.BuildKey);
            if (typeToBuild.IsArray && typeToBuild.GetArrayRank() == 1)
            {
                Type elementType = typeToBuild.GetElementType();

                MethodInfo resolverMethod = genericResolveArrayMethod.MakeGenericMethod(elementType);

                ArrayResolver resolver = (ArrayResolver) Delegate.CreateDelegate(typeof (ArrayResolver), resolverMethod);

                context.Existing = resolver(context);
                context.BuildComplete = true;
            }
        }

        private static object ResolveArray<T>(IBuilderContext context)
        {
            IUnityContainer container = BuilderContext.NewBuildUp<IUnityContainer>(context);
            List<T> results = new List<T>(container.ResolveAll<T>());
            return results.ToArray();
        }
    }
}
