// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.Configuration.Tests.TestObjects;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerForArrayInjection
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerForArrayInjection : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerForArrayInjection() : base("ArrayInjection", "")
        {
        }

        [TestMethod]
        public void Then_DefaultResolutionReturnsAllRegisteredLoggers()
        {
            var result = Container.Resolve<ArrayDependencyObject>("defaultInjection");

            result.Loggers.Select(l => l.GetType()).AssertContainsInAnyOrder(
                typeof (SpecialLogger), typeof (MockLogger), typeof (MockLogger));
        }

        [TestMethod]
        public void Then_SpecificElementsAreInjected()
        {
            var result = Container.Resolve<ArrayDependencyObject>("specificElements");

            result.Loggers.Select(l => l.GetType()).AssertContainsInAnyOrder(
                typeof (SpecialLogger), typeof (MockLogger));
        }

        [TestMethod]
        public void Then_CanMixResolutionAndValuesInAnArray()
        {
            var result = Container.Resolve<ArrayDependencyObject>("mixingResolvesAndValues");

            result.Strings.AssertContainsExactly("first", "Not the second", "third");
        }

        [TestMethod]
        public void Then_CanConfigureZeroLengthArrayForInjection()
        {
            var result = Container.Resolve<ArrayDependencyObject>("zeroLengthArray");

            Assert.IsNotNull(result.Strings);
            Assert.AreEqual(0, result.Strings.Length);
        }

        [TestMethod]
        public void Then_GenericArrayPropertiesAreInjected()
        {
            var result = Container.Resolve<GenericArrayPropertyDependency<string>>("defaultResolution");

            result.Stuff.AssertContainsInAnyOrder("first", "second", "third");
        }

        [TestMethod]
        public void Then_CanConfigureZeroLengthGenericArrayToBeInjected()
        {
            var result = Container.Resolve<GenericArrayPropertyDependency<string>>("explicitZeroLengthArray");

            Assert.AreEqual(0, result.Stuff.Count());
        }    
    }
}
