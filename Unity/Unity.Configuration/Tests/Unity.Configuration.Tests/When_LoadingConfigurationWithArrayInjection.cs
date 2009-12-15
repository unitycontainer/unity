using System.Linq;
using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_LoadingConfigurationWithArrayInjection
    /// </summary>
    [TestClass]
    public class When_LoadingConfigurationWithArrayInjection : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingConfigurationWithArrayInjection() : base("ArrayInjection")
        {
        }

        [TestMethod]
        public void Then_ArrayPropertyHasArrayElementAsValue()
        {
            var prop = GetArrayPropertyElement("specificElements");

            Assert.IsInstanceOfType(prop.Value, typeof (ArrayElement));
        }

        [TestMethod]
        public void Then_ArrayPropertyHasTwoValuesThatWillBeInjected()
        {
            var prop = GetArrayPropertyElement("specificElements");
            var arrayValue = (ArrayElement) prop.Value;

            Assert.AreEqual(2, arrayValue.Values.Count);
        }

        [TestMethod]
        public void Then_ArrayPropertyValuesAreAllDependencies()
        {
            var prop = GetArrayPropertyElement("specificElements");
            var arrayValue = (ArrayElement) prop.Value;

            Assert.IsTrue(arrayValue.Values.All(v => v is DependencyElement));
        }

        [TestMethod]
        public void Then_ArrayPropertyValuesHaveExpectedNames()
        {
            var prop = GetArrayPropertyElement("specificElements");
            var arrayValue = (ArrayElement)prop.Value;

            CollectionAssert.AreEqual(new[] {"main", "special"},
                arrayValue.Values.Cast<DependencyElement>().Select(e => e.Name).ToList());
        }

        private PropertyElement GetArrayPropertyElement(string registrationName)
        {
            var registration = Section.Containers.Default.Registrations
                .Where(r => r.TypeName == "ArrayDependencyObject" && r.Name == registrationName)
                .First();

            return registration.InjectionMembers.OfType<PropertyElement>()
                .Where(pe => pe.Name == "Loggers")
                .First();
            
        }

    }
}
