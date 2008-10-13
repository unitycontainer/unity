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

using System.Collections.Generic;
using System.Configuration;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    [TestClass]
    public class InjectionConfigurationFixture : ConfigurationFixtureBase
    {
        private const string configFileName = "ConfiguringInjectionConstructor";

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void MockDatabaseIsConfiguredToCallDefaultConstructor()
        {
            MockDatabase db = ResolveConfiguredObject<MockDatabase>("defaultConstructor");

            Assert.IsTrue(string.IsNullOrEmpty(db.ConnectionString));
            Assert.IsTrue(db.DefaultConstructorCalled);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void MockDatabaseIsInitializedWithConnectionString()
        {
            MockDatabase db = ResolveConfiguredObject<MockDatabase>("oneParameterConstructor");

            Assert.IsFalse(db.DefaultConstructorCalled);
            Assert.AreEqual("Northwind", db.ConnectionString);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void CanConfigureDependencyParametersForConstructor()
        {
            ObjectWithOneConstructorDependency obj =
                ResolveConfiguredObject<ObjectWithOneConstructorDependency>(
                    "oneDependencyParameterConstructor");

            Assert.IsNotNull(obj.Logger);
            Assert.IsInstanceOfType(obj.Logger, typeof(SpecialLogger));
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void CanConfigureMultipleConstructorParameters()
        {
            ObjectWithTwoConstructorParameters obj =
                ResolveConfiguredObject<ObjectWithTwoConstructorParameters>(
                    "twoConstructorParameters");

            Assert.IsNotNull(obj.ConnectionString);
            Assert.IsNotNull(obj.Logger);

            Assert.AreEqual("AdventureWorks", obj.ConnectionString);
            Assert.IsInstanceOfType(obj.Logger, typeof(MockLogger));

        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void CanConfigurePropertyInjection()
        {
            ObjectUsingLogger obj = ResolveConfiguredObject<ObjectUsingLogger>("injectionProperty");

            Assert.IsNotNull(obj.Logger);
            Assert.IsInstanceOfType(obj.Logger, typeof(MockLogger));
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void CanInjectMultipleProperties()
        {
            ObjectWithTwoProperties obj =
                ResolveConfiguredObject<ObjectWithTwoProperties>("multipleProperties");

            Assert.IsInstanceOfType(obj.Obj1, typeof(SpecialLogger));
            Assert.AreEqual("Hello", obj.Obj2);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void InjectionMethodsAreCalled()
        {
            ObjectWithInjectionMethod obj =
                ResolveConfiguredObject<ObjectWithInjectionMethod>("method");

            Assert.AreEqual(obj.ConnectionString, "contoso");
            Assert.IsInstanceOfType(obj.Logger, typeof(MockLogger));
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void CanInjectSameMethodSeveralTimes()
        {
            ObjectWithOverloadedInjectionMethod obj =
                ResolveConfiguredObject<ObjectWithOverloadedInjectionMethod>("method-multi");

            Assert.AreEqual(3, obj.initializationParameters.Count);
            Assert.AreEqual("contoso", obj.initializationParameters[0]);
            Assert.AreEqual("northwind", obj.initializationParameters[1]);
            Assert.AreEqual("AdventureWorks", obj.initializationParameters[2]);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void CanInjectOverloads()
        {
            ObjectWithOverloadedInjectionMethod obj =
                ResolveConfiguredObject<ObjectWithOverloadedInjectionMethod>("method-overload");

            Assert.AreEqual(4, obj.initializationParameters.Count);
            Assert.AreEqual("contoso", (string)obj.initializationParameters[0]);
            Assert.AreEqual(14, (int)obj.initializationParameters[1]);
            Assert.AreEqual("AdventureWorks", (string)obj.initializationParameters[2]);
            Assert.AreEqual(42, (int)obj.initializationParameters[3]);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void CanConfigureInjectionForNamedInstances()
        {
            MockDatabase db =
                ResolveConfiguredObject<MockDatabase>("injectNamed", "Northwind");

            Assert.AreEqual("Northwind", db.ConnectionString);
        }

        [TestMethod]
        [DeploymentItem("ConfiguringInjectionConstructor.config")]
        public void ConfiguringNamedInjectionDoesntAffectDefault()
        {
            IUnityContainer container = GetConfiguredContainer("injectNamed");
            MockDatabase defaultDb = container.Resolve<MockDatabase>();
            MockDatabase nwDb = container.Resolve<MockDatabase>("Northwind");

            Assert.AreEqual("contoso", defaultDb.ConnectionString);
            Assert.AreEqual("Northwind", nwDb.ConnectionString);
        }

        protected override string ConfigFileName
        {
            get { return configFileName; }
        }
    }

    public class ObjectWithOverloadedInjectionMethod
    {
        public List<object> initializationParameters = new List<object>();

        public void Initialize(string connectionString)
        {
            initializationParameters.Add(connectionString);
        }

        public void Initialize(int value)
        {
            initializationParameters.Add(value);
        }
    }
}
