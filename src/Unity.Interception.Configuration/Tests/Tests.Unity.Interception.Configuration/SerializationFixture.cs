// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Configuration;
using Unity.TestSupport.Configuration;

namespace Unity.InterceptionExtension.Configuration.Tests
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
