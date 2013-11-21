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

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.Configuration.Tests.TestObjects;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerToResolveGenerics
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerToResolveGenerics : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerToResolveGenerics() : base("InjectingGenerics", "")
        {
        }

        [TestMethod]
        public void Then_GenericParameterAsStringIsProperlySubstituted()
        {
            Container.RegisterType(typeof (GenericObjectWithConstructorDependency<>), "manual",
                new InjectionConstructor(new GenericParameter("T")));
            var manualResult = Container.Resolve<GenericObjectWithConstructorDependency<string>>("manual");

            var resultForString = Container.Resolve<GenericObjectWithConstructorDependency<string>>("basic");
            Assert.AreEqual(Container.Resolve<string>(), resultForString.Value);
        }

        [TestMethod]
        public void Then_GenericParameterAsIntIsProperlySubstituted()
        {
            var resultForInt = Container.Resolve<GenericObjectWithConstructorDependency<int>>("basic");
            Assert.AreEqual(Container.Resolve<int>(), resultForInt.Value);
        }


    }
}
