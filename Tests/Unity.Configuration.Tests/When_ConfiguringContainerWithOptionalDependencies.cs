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

using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerWithOptionalDependencies
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerWithOptionalDependencies : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithOptionalDependencies() : base("OptionalDependency", "")
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
