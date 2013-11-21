// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
            context.AddElement<TestInjectionMemberElement>("testInjectionMember");
            context.AddElement<SeventeenValueElement>("seventeen");
        }
    }
}
