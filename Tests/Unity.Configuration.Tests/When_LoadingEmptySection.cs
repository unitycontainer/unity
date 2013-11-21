// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_LoadingEmptySection
    /// </summary>
    [TestClass]
    public class When_LoadingEmptySection : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingEmptySection() : base("EmptySection")
        {
            
        }

        [TestMethod]
        public void Then_SectionIsPresent()
        {
            Assert.IsNotNull(Section);
        }
    }
}
