// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_LoadingConfigurationWithConstructors
    /// </summary>
    [TestClass]
    public class When_LoadingConfigurationWithConstructors : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingConfigurationWithConstructors()
            : base("RegistrationWithConstructors")
        {
        }

        private ContainerElement container;
        private RegisterElement firstRegistration;
        private RegisterElement secondRegistration;
        private RegisterElement thirdRegistration;

        protected override void Arrange()
        {
            base.Arrange();
            this.container = this.section.Containers.Default;
            this.firstRegistration = this.container.Registrations[0];
            this.secondRegistration = this.container.Registrations[1];
            this.thirdRegistration = this.container.Registrations[2];
        }

        [TestMethod]
        public void Then_FirstRegistrationHasOneInjectionMember()
        {
            Assert.AreEqual(1, this.firstRegistration.InjectionMembers.Count);
        }

        [TestMethod]
        public void Then_FirstRegistrationHasConstructorMember()
        {
            Assert.IsInstanceOfType(this.firstRegistration.InjectionMembers[0], typeof(ConstructorElement));
        }

        [TestMethod]
        public void Then_FirstRegistrationConstructorHasExpectedParameters()
        {
            var constructorElement = (ConstructorElement)this.firstRegistration.InjectionMembers[0];

            constructorElement.Parameters.Select(p => p.Name).AssertContainsExactly("one", "two", "three");
        }

        [TestMethod]
        public void Then_SecondRegistrationHasNoInjectionMembers()
        {
            Assert.AreEqual(0, this.secondRegistration.InjectionMembers.Count);
        }

        [TestMethod]
        public void Then_ThirdRegistrationHasZeroArgConstructor()
        {
            Assert.AreEqual(0,
                ((ConstructorElement)this.thirdRegistration.InjectionMembers[0]).Parameters.Count);
        }
    }
}
