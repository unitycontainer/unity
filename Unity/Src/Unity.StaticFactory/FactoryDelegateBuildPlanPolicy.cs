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

using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.StaticFactory
{
    internal delegate object FactoryBuildPlanDelegate();

    class FactoryDelegateBuildPlanPolicy : IBuildPlanPolicy
    {
        private FactoryBuildPlanDelegate factory;

        public FactoryDelegateBuildPlanPolicy(FactoryBuildPlanDelegate factory)
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
                context.Existing = factory();
            }
        }
    }
}
