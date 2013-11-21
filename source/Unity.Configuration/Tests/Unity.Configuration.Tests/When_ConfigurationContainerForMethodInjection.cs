// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.Configuration.Tests.TestObjects;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfigurationContainerForMethodInjection
    /// </summary>
    [TestClass]
    public class When_ConfigurationContainerForMethodInjection : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfigurationContainerForMethodInjection() : base("MethodInjection", "")
        {
        }

        [TestMethod]
        public void Then_SingleInjectionMethodIsCalledWithExpectedValues()
        {
            var result = Container.Resolve<ObjectWithInjectionMethod>("singleMethod");

            Assert.AreEqual("northwind", result.ConnectionString);
            Assert.IsInstanceOfType(result.Logger, typeof (MockLogger));
        }

        [TestMethod]
        public void Then_MultipleInjectionMethodsCalledWithExpectedValues()
        {
            var result = Container.Resolve<ObjectWithInjectionMethod>("twoMethods");

            Assert.AreEqual("northwind", result.ConnectionString);
            Assert.IsInstanceOfType(result.Logger, typeof (MockLogger));
            Assert.IsNotNull(result.MoreData);
        }

        [TestMethod]
        public void Then_CorrectFirstOverloadIsCalled()
        {
            var result = Container.Resolve<ObjectWithOverloads>("callFirstOverload");

            Assert.AreEqual(1, result.FirstOverloadCalls);
            Assert.AreEqual(0, result.SecondOverloadCalls);
        }

        [TestMethod]
        public void Then_CorrectSecondOverloadIsCalled()
        {
            var result = Container.Resolve<ObjectWithOverloads>("callSecondOverload");

            Assert.AreEqual(0, result.FirstOverloadCalls);
            Assert.AreEqual(1, result.SecondOverloadCalls);
            
        }

        [TestMethod]
        public void Then_BothOverloadsAreCalled()
        {
            var result = Container.Resolve<ObjectWithOverloads>("callBothOverloads");

            Assert.AreEqual(1, result.FirstOverloadCalls);
            Assert.AreEqual(1, result.SecondOverloadCalls);
        }

        [TestMethod]
        public void Then_FirstOverloadIsCalledTwice()
        {
            var result = Container.Resolve<ObjectWithOverloads>("callFirstOverloadTwice");

            Assert.AreEqual(2, result.FirstOverloadCalls);
            Assert.AreEqual(0, result.SecondOverloadCalls);
        }


    }
}
