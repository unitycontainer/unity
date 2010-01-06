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

namespace Microsoft.Practices.Unity.Configuration.Tests.TestObjects
{
    class TestSectionExtension : SectionExtension
    {
        public static int NumberOfCalls;

        public TestSectionExtension()
        {
            NumberOfCalls = 0;
        }

        /// <summary>
        /// Add the extensions to the section via the context.
        /// </summary>
        /// <param name="context">Context object that can be used to add elements and aliases.</param>
        public override void AddExtensions(SectionExtensionContext context)
        {
            ++NumberOfCalls;

            context.AddAlias<ObjectTakingScalars>("scalarObject");
            context.AddElement<ContainerConfigElementOne>("configOne");
            context.AddElement<ContainerConfigElementTwo>("configTwo");
            context.AddElement<SeventeenValueElement>("seventeen");
        }
    }
}
