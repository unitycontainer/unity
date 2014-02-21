// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder2;

namespace Unity.Tests.Extension
{
    internal class MyCustomPolicy : IBuildPlanPolicy
    {
        public bool MyCustomPolicyBool { get; set; }

        /// <summary>
        /// Creates an instance of this build plan's type, or fills
        /// in the existing type if passed in.
        /// </summary>
        /// <param name="context">Context used to build up the object.</param>
        public void BuildUp(IBuilderContext context)
        {
            if (context.Existing == null)
            {
                context.Existing = new object();
            }
        }
    }
}
