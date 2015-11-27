// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Configuration.Tests.ConfigFiles;
using Unity.TestSupport;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerWithOptionalDependencies
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerWithOptionalDependencies : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithOptionalDependencies()
            : base("OptionalDependency", String.Empty)
        {
        }

        [TestMethod]
        public void Then_RegisteredOptionalDependencyIsInjected()
        {
            var result = Container.Resolve<ObjectUsingLogger>("dependencyRegistered");
            Assert.IsNotNull(result.Logger);
        }

        [TestMethod]
        public void Then_UnregisteredOptionalDependencyIsNotInjected()
        {
            var result = Container.Resolve<ObjectUsingLogger>("dependencyNotRegistered");
            Assert.IsNull(result.Logger);
        }
    }
}
