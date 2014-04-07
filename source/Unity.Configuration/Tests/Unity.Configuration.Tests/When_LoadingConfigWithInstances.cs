// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_LoadingConfigWithInstances
    /// </summary>
    [TestClass]
    public class When_LoadingConfigWithInstances : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingConfigWithInstances()
            : base("RegisteringInstances")
        {
        }

        [TestMethod]
        public void Then_ContainerHasExpectedInstancesElements()
        {
            Assert.AreEqual(4, Section.Containers.Default.Instances.Count);
        }

        [TestMethod]
        public void Then_InstancesHaveExpectedContents()
        {
            var expected = new[]
                {
                    // Name, Value, Type, TypeConverter
                    new[] {"", "AdventureWorks", "", ""},
                    new[] {"", "42", "System.Int32", ""},
                    new[] {"negated", "23", "int", "negator"},
                    new[] {"forward", "23", "int", ""}
                };

            for (int index = 0; index < expected.Length; ++index)
            {
                var instance = Section.Containers.Default.Instances[index];
                CollectionAssertExtensions.AreEqual(
                    expected[index],
                    new string[] { instance.Name, instance.Value, instance.TypeName, instance.TypeConverterTypeName },
                    string.Format("Element at index {0} does not match", index));
            }
        }
    }
}
