// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using Unity.Configuration.Tests.ConfigFiles;
using Unity.TestSupport;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_LoadingConfigurationWithArrayInjection
    /// </summary>
    [TestClass]
    public class When_LoadingConfigurationWithArrayInjection : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingConfigurationWithArrayInjection()
            : base("ArrayInjection")
        {
        }

        [TestMethod]
        public void Then_ArrayPropertyHasArrayElementAsValue()
        {
            var prop = this.GetArrayPropertyElement("specificElements");

            Assert.IsInstanceOfType(prop.Value, typeof(ArrayElement));
        }

        [TestMethod]
        public void Then_ArrayPropertyHasTwoValuesThatWillBeInjected()
        {
            var prop = this.GetArrayPropertyElement("specificElements");
            var arrayValue = (ArrayElement)prop.Value;

            Assert.AreEqual(2, arrayValue.Values.Count);
        }

        [TestMethod]
        public void Then_ArrayPropertyValuesAreAllDependencies()
        {
            var prop = this.GetArrayPropertyElement("specificElements");
            var arrayValue = (ArrayElement)prop.Value;

            Assert.IsTrue(arrayValue.Values.All(v => v is DependencyElement));
        }

        [TestMethod]
        public void Then_ArrayPropertyValuesHaveExpectedNames()
        {
            var prop = this.GetArrayPropertyElement("specificElements");
            var arrayValue = (ArrayElement)prop.Value;

            CollectionAssertExtensions.AreEqual(new[] { "main", "special" },
                arrayValue.Values.Cast<DependencyElement>().Select(e => e.Name).ToList());
        }

        private PropertyElement GetArrayPropertyElement(string registrationName)
        {
            var registration = section.Containers.Default.Registrations
                .Where(r => r.TypeName == "ArrayDependencyObject" && r.Name == registrationName)
                .First();

            return registration.InjectionMembers.OfType<PropertyElement>()
                .Where(pe => pe.Name == "Loggers")
                .First();
        }
    }
}
