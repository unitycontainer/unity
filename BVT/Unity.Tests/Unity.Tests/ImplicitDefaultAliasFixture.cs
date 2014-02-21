// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests
{
    /// <summary>
    /// Summary description for TestImplicitDefaultRegistrations
    /// </summary>
    [TestClass]
    public class ImplicitDefaultAliasFixture : ConfigurationFixtureBase
    {
        public const string ConfigFileName = @"ConfigFiles\TestImplicitDefaultAlias.config";
        public ImplicitDefaultAliasFixture()
            : base(ConfigFileName)
        {
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ImplicitAliasesRegistrationsRetrieved()
        {
            IUnityContainer container = GetContainer();

            var test = container.Resolve<TestTypeToInjectWithValues>();
            
            Assert.AreEqual<int>(123123, test.Test1);
            Assert.AreEqual<string>("testValue", test.Test);
            Assert.AreEqual<float>((float)1.123123, test.Test2);
        }
    }

    public class TestTypeToInjectWithValues
    {
        public TestTypeToInjectWithValues(string test, int test1, float test2)
        {
            Test = test;
            Test1 = test1;
            Test2 = test2;
        }

        public int Test1 { get; set; }
        public string Test { get; set; }
        public float Test2 { get; set; }
    }
}