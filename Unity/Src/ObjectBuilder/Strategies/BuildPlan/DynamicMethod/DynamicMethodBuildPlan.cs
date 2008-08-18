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

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// A delegate type that defines the signature of the
    /// dynamic method created by the build plans.
    /// </summary>
    /// <param name="context"><see cref="IBuilderContext"/> used to build up the object.</param>
    delegate void DynamicBuildPlanMethod(
        IBuilderContext context);
    
    /// <summary>
    /// An implementation of <see cref="IBuildPlanPolicy"/> that runs the
    /// given delegate to execute the plan.
    /// </summary>
    class DynamicMethodBuildPlan : IBuildPlanPolicy
    {
        private DynamicBuildPlanMethod planMethod;

        public DynamicMethodBuildPlan(DynamicBuildPlanMethod planMethod)
        {
            this.planMethod = planMethod;
        }

        public void BuildUp(IBuilderContext context)
        {
            planMethod(context);
        }
    }
}
