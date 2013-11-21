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
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.TestSupport.Configuration;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
{
    /// <summary>
    /// Base class for test fixtures that are testing serialization of the
    /// configuration sections.
    /// </summary>
    public abstract class SerializationFixture
    {
        protected UnityConfigurationSection SerializeAndLoadConfig(string filename, Action<ContainerElement> containerInitializer)
        {
            var serializer = new ConfigSerializer(filename);
            var section = new UnityConfigurationSection();
            section.SectionExtensions.Add(new SectionExtensionElement()
            {
                TypeName = typeof(InterceptionConfigurationExtension).AssemblyQualifiedName
            });

            var container = new ContainerElement();
            section.Containers.Add(container);

            containerInitializer(container);

            serializer.Save("unity", section);

            return (UnityConfigurationSection)serializer.Load().GetSection("unity");
        }
    }
}
