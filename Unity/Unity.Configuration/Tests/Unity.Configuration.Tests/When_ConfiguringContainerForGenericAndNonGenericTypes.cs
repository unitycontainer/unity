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
using Microsoft.Practices.Unity.Configuration.Tests.TestObjects.MyGenericTypes;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerForGenericAndNonGenericTypes
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerForGenericAndNonGenericTypes : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerForGenericAndNonGenericTypes() : base("Generics", "container1")
        {
        }

        [TestMethod]
        public void Then_CanResolveConfiguredGenericType()
        {
            var result = Container.Resolve<ItemsCollection<IItem>>();

            Assert.AreEqual(11, result.Items.Length);
            Assert.IsInstanceOfType(result.Printer, typeof(MyPrintService<IItem>));
        }
    }
}
