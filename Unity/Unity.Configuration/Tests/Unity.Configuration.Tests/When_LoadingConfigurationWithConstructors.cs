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
using Microsoft.Practices.Unity.TestSupport;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_LoadingConfigurationWithConstructors
    /// </summary>
    [TestClass]
    public class When_LoadingConfigurationWithConstructors : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingConfigurationWithConstructors() : base("RegistrationWithConstructors")
        {
        }

        private ContainerElement container;
        private RegisterElement firstRegistration;
        private RegisterElement secondRegistration;
        private RegisterElement thirdRegistration;

        protected override void Arrange()
        {
            base.Arrange();
            container = Section.Containers.Default;
            firstRegistration = container.Registrations[0];
            secondRegistration = container.Registrations[1];
            thirdRegistration = container.Registrations[2];
        }

        [TestMethod]
        public void Then_FirstRegistrationHasOneInjectionMember()
        {
            Assert.AreEqual(1, firstRegistration.InjectionMembers.Count);
        }

        [TestMethod]
        public void Then_FirstRegistrationHasConstructorMember()
        {
            Assert.IsInstanceOfType(firstRegistration.InjectionMembers[0], typeof(ConstructorElement));
        }

        [TestMethod]
        public void Then_FirstRegistrationConstructorHasExpectedParameters()
        {
            var constructorElement = (ConstructorElement)firstRegistration.InjectionMembers[0];

            constructorElement.Parameters.Select(p => p.Name).AssertContainsExactly("foo", "bar", "baz");
        }

        [TestMethod]
        public void Then_SecondRegistrationHasNoInjectionMembers()
        {
            Assert.AreEqual(0, secondRegistration.InjectionMembers.Count);
        }

        [TestMethod]
        public void Then_ThirdRegistrationHasZeroArgConstructor()
        {
            Assert.AreEqual(0,
                ((ConstructorElement)thirdRegistration.InjectionMembers[0]).Parameters.Count);
        }
    }
}
