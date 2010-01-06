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

using System.Linq;
using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
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
        public When_LoadingConfigWithInstances() : base("RegisteringInstances")
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

            for(int index = 0; index < expected.Length; ++index)
            {
                var instance = Section.Containers.Default.Instances[index];
                CollectionAssert.AreEqual(expected[index],
                    new string[] {instance.Name, instance.Value, instance.TypeName, instance.TypeConverterTypeName},
                    string.Format("Element at index {0} does not match", index));
            }
        }
    }
}
