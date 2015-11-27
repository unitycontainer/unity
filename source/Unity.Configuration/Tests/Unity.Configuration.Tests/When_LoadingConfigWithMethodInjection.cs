// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using System.Text;
using Unity.Configuration.Tests.ConfigFiles;
using Unity.TestSupport;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_LoadingConfigWithMethodInjection
    /// </summary>
    [TestClass]
    public class When_LoadingConfigWithMethodInjection : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingConfigWithMethodInjection()
            : base("MethodInjection")
        {
        }

        [TestMethod]
        public void Then_FirstRegistrationHasOneMethodInjection()
        {
            var registration = (from reg in section.Containers.Default.Registrations
                                where reg.TypeName == "ObjectWithInjectionMethod" && reg.Name == "singleMethod"
                                select reg).First();

            Assert.AreEqual(1, registration.InjectionMembers.Count);
            var methodRegistration = (MethodElement)registration.InjectionMembers[0];

            Assert.AreEqual("Initialize", methodRegistration.Name);
            CollectionAssertExtensions.AreEqual(new string[] { "connectionString", "logger" },
                methodRegistration.Parameters.Select(p => p.Name).ToList());
        }
    }
}
