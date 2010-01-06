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
using Microsoft.Practices.Unity;

namespace Microsoft.Practices.ObjectBuilder2
{
    class FactoryDelegateBuildPlanPolicy : IBuildPlanPolicy
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
        public void BuildUp(IBuilderContext context)
        {
            if(context.Existing == null)
            {
                var currentContainer = context.NewBuildUp<IUnityContainer>();
                context.Existing = factory(currentContainer, context.BuildKey.Type, context.BuildKey.Name);

                DynamicMethodConstructorStrategy.SetPerBuildSingleton(context);
            }
        }
    }
}
