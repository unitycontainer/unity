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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for OptionalDependencyConfigurationFixture
    /// </summary>
    [TestClass]
    public class OptionalDependencyConfigurationFixture : ConfigurationFixtureBase
    {
        protected override string ConfigFileName
        {
            get { return "OptionalDependencies"; }
        }

        [TestMethod]
        [DeploymentItem("OptionalDependencies.config")]
        public void DefaultConfigurationWorks()
        {
            IUnityContainer container = GetConfiguredContainer("defaultConstructor");
            var result = container.Resolve<GuineaPig>();

            Assert.IsNull(result.Pig);
        }

        [TestMethod]
        [DeploymentItem("OptionalDependencies.config")]
        public void OptionalConstructorDependencyInjectsNull()
        {
            IUnityContainer container = GetConfiguredContainer("optionalConstructorParameter");
            var result = container.Resolve<GuineaPig>();

            Assert.IsNull(result.Pig);
            Assert.IsTrue(result.ConstructorWithArgsWasCalled);
        }

        [TestMethod]
        [DeploymentItem("OptionalDependencies.config")]
        public void OptionalPropertyInjectionInjectsNull()
        {
            IUnityContainer container = GetConfiguredContainer("optionalProperty");
            var result = container.Resolve<GuineaPig>();

            Assert.IsNull(result.Pig);
            Assert.IsFalse(result.ConstructorWithArgsWasCalled);
        }

        [TestMethod]
        [DeploymentItem("OptionalDependencies.config")]
        public void OptionalPropertyInjectsValueWhenAvailable()
        {
            IGuineaPig expected = new Mock<IGuineaPig>().Object;
            IUnityContainer container = GetConfiguredContainer("optionalProperty");
            container.RegisterInstance(expected);

            var result = container.Resolve<GuineaPig>();

            Assert.AreSame(expected, result.Pig);
        }

        public class GuineaPig
        {
            public bool ConstructorWithArgsWasCalled = false;

            public GuineaPig()
            {

            }

            public GuineaPig(IGuineaPig pig)
            {
                Pig = pig;
                ConstructorWithArgsWasCalled = true;
            }

            public IGuineaPig Pig
            {
                get;
                set;
            }

            public void SetPig(IGuineaPig pig)
            {
                Pig = pig;
            }
        }

        public interface IGuineaPig
        {
        }

        public class GuineaPigImpl : IGuineaPig
        {
        }
    }
}
