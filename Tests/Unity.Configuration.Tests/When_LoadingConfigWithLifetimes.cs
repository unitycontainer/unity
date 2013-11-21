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
    /// Summary description for When_LoadingConfigWithLifetimes
    /// </summary>
    [TestClass]
    public class When_LoadingConfigWithLifetimes : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingConfigWithLifetimes() : base("Lifetimes")
        {
        }

        [TestMethod]
        public void Then_ILoggerHasSingletonLifetime()
        {
            var registration = Section.Containers.Default.Registrations.Where(
                r => r.TypeName == "ILogger" && r.Name == string.Empty).First();

            Assert.AreEqual("singleton", registration.Lifetime.TypeName);
        }

        [TestMethod]
        public void Then_TypeConverterInformationIsProperlyDeserialized()
        {
            var lifetime = Section.Containers.Default.Registrations
                .Where(r => r.TypeName == "ILogger" && r.Name == "reverseSession")
                .First()
                .Lifetime;

            Assert.AreEqual("session", lifetime.TypeName);
            Assert.AreEqual("backwards", lifetime.Value);
            Assert.AreEqual("reversed", lifetime.TypeConverterTypeName);
        }
    }
}
