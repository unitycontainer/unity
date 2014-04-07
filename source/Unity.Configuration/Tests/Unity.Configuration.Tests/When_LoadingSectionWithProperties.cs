// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_LoadingSectionWithProperties
    /// </summary>
    [TestClass]
    public class When_LoadingSectionWithProperties : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingSectionWithProperties()
            : base("InjectingProperties")
        {
        }

        [TestMethod]
        public void Then_RegistrationHasOnePropertyElement()
        {
            var registration = (from reg in Section.Containers.Default.Registrations
                                where reg.TypeName == "ObjectWithTwoProperties" && reg.Name == "singleProperty"
                                select reg).First();

            Assert.AreEqual(1, registration.InjectionMembers.Count);
            Assert.IsInstanceOfType(registration.InjectionMembers[0], typeof(PropertyElement));
        }

        [TestMethod]
        public void Then_RegistrationHasTwoPropertyElements()
        {
            var registration = (from reg in Section.Containers.Default.Registrations
                                where reg.TypeName == "ObjectWithTwoProperties" && reg.Name == "twoProperties"
                                select reg).First();

            Assert.AreEqual(2, registration.InjectionMembers.Count);
            Assert.IsTrue(registration.InjectionMembers.All(im => im is PropertyElement));
        }

        [TestMethod]
        public void Then_PropertyNamesAreProperlyDeserialized()
        {
            var registration = (from reg in Section.Containers.Default.Registrations
                                where reg.TypeName == "ObjectWithTwoProperties" && reg.Name == "twoProperties"
                                select reg).First();

            CollectionAssertExtensions.AreEqual(new string[] { "Obj1", "Obj2" },
                registration.InjectionMembers.OfType<PropertyElement>().Select(pe => pe.Name).ToList());
        }
    }
}
