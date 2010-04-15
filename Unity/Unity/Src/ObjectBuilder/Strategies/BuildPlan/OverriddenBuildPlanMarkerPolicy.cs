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
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.Properties;

namespace Microsoft.Practices.ObjectBuilder2
{
    class OverriddenBuildPlanMarkerPolicy : IBuildPlanPolicy
    {
        /// <summary>
        /// Creates an instance of this build plan's type, or fills
        /// in the existing type if passed in.
        /// </summary>
        /// <param name="context">Context used to build up the object.</param>
        public void BuildUp(IBuilderContext context)
        {
            throw new InvalidOperationException(Resources.MarkerBuildPlanInvoked);
        }
    }
}
