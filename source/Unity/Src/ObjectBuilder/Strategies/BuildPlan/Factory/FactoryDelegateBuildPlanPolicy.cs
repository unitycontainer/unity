// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity;

namespace ObjectBuilder2
{
    internal class FactoryDelegateBuildPlanPolicy : IBuildPlanPolicy
    {
        private readonly Func<IUnityContainer, Type, string, object> factory;

        public FactoryDelegateBuildPlanPolicy(Func<IUnityContainer, Type, string, object> factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Creates an instance of this build plan's type, or fills
        /// in the existing type if passed in.
        /// </summary>
        /// <param name="context">Context used to build up the object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class")]
        public void BuildUp(IBuilderContext context)
        {
            Unity.Utility.Guard.ArgumentNotNull(context, "context");

            if (context.Existing == null)
            {
                var currentContainer = context.NewBuildUp<IUnityContainer>();
                context.Existing = factory(currentContainer, context.BuildKey.Type, context.BuildKey.Name);

                DynamicMethodConstructorStrategy.SetPerBuildSingleton(context);
            }
        }
    }
}
