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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public delegate void DynamicBuildPlanMethod(IBuilderContext context);

    /// <summary>
    /// 
    /// </summary>
    public class DynamicMethodBuildPlan : IBuildPlanPolicy
    {
        DynamicBuildPlanMethod buildMethod;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buildMethod"></param>
        public DynamicMethodBuildPlan(DynamicBuildPlanMethod buildMethod)
        {
            this.buildMethod = buildMethod;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void BuildUp(IBuilderContext context)
        {
            buildMethod(context);
        }
    }
}
