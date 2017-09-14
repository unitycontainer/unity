// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Configuration.Tests.ConfigFiles;
using Unity.TestSupport;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerForPropertyInjection
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerForPropertyInjection : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerForPropertyInjection()
            : base("InjectingProperties", String.Empty)
        {
        }

        [TestMethod]
        public void Then_InjectedPropertyIsResolvedAccordingToConfiguration()
        {
            var expected = Container.Resolve<object>("special");
            var result = Container.Resolve<ObjectWithTwoProperties>("singleProperty");

            Assert.AreSame(expected, result.Obj1);
        }

        [TestMethod]
        public void Then_InjectedPropertyIsResolvedAccordingToConfigurationUsingAttributes()
        {
            var expected = Container.Resolve<object>("special");
            var result = Container.Resolve<ObjectWithTwoProperties>("twoProperties");

            Assert.AreSame(expected, result.Obj1);
        }

        [TestMethod]
        public void Then_InjectedPropertyIsProperType()
        {
            var result = Container.Resolve<ObjectWithTwoProperties>("injectingDifferentType");

            Assert.IsInstanceOfType(result.Obj1, typeof(SpecialLogger));
        }

        [TestMethod]
        public void Then_MultiplePropertiesGetInjected()
        {
            var expected = Container.Resolve<object>("special");
            var result = Container.Resolve<ObjectWithTwoProperties>("injectingDifferentType");

            Assert.IsInstanceOfType(result.Obj1, typeof(SpecialLogger));
            Assert.AreSame(expected, result.Obj2);
        }
    }
}
