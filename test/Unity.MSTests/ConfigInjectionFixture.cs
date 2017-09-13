// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Tests.TestObjects;

namespace Unity.Tests
{
    [TestClass]
    public class ConfigInjectionFixture : ConfigurationFixtureBase
    {
        private const string ConfigFileName = @"ConfigFiles\ConfigInjectionFile.config";

        public ConfigInjectionFixture()
            : base(ConfigFileName)
        { }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        [ExpectedException(typeof(ArgumentException), "container not found")]
        public void ConfigInject_WrongContainerName()
        {
            MockDatabase db = ResolveConfiguredObject<MockDatabase>("NotExists");
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_MockDatabaseIsConfiguredToCallDefaultConstructor()
        {
            MockDatabase db = ResolveConfiguredObject<MockDatabase>("defaultConstructor");

            Assert.IsTrue(string.IsNullOrEmpty(db.ConnectionString));
            Assert.IsTrue(db.DefaultConstructorCalled);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_MockDatabaseIsConfiguredToCallOneIntParameterConstructor()
        {
            MockDatabase db = ResolveConfiguredObject<MockDatabase>("oneIntParameterConstructor");

            Assert.AreEqual(db.SomeNumber, 101);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_CanConfigureInjectionForNamedInstances()
        {
            MockDatabase db =
                ResolveConfiguredObject<MockDatabase>("injectNamed", "Northwind");

            Assert.AreEqual("Northwind", db.ConnectionString);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_ConfiguringNamedInjectionDoesntAffectDefault()
        {
            IUnityContainer container = GetContainer("injectNamed");
            MockDatabase defaultDb = container.Resolve<MockDatabase>();
            MockDatabase nwDb = container.Resolve<MockDatabase>("Northwind");

            Assert.AreEqual("contoso", defaultDb.ConnectionString);
            Assert.AreEqual("Northwind", nwDb.ConnectionString);
        }

        [TestMethod]
        public void ConfigInject_CodePlexBugFix()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            IUnityContainer container = new UnityContainer()
                .Configure<InjectedMembers>()
                    .ConfigureInjectionFor<TestCodePlexBug>(
                        new InjectionConstructor())
                .Container;

            TestCodePlexBug myObject1 = container.Resolve<TestCodePlexBug>();
            Assert.IsTrue(myObject1.DefaultConstructorCalled);
            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor<TestCodePlexBug>(
                new InjectionConstructor("BugFix"));

            TestCodePlexBug myObject = container.Resolve<TestCodePlexBug>();

            Assert.AreEqual("BugFix", myObject.MyString);
            Assert.IsFalse(myObject.DefaultConstructorCalled);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_MockDatabaseIsInitializedWithTwoParameters()
        {
            MockDatabase db = ResolveConfiguredObject<MockDatabase>("stringIntConstructor");

            Assert.IsFalse(db.DefaultConstructorCalled);
            Assert.AreEqual("Northwind", db.ConnectionString);
            Assert.AreEqual(101, db.SomeNumber);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_MockDatabaseIsInitializedWithOneParameter()
        {
            MockDatabase db = ResolveConfiguredObject<MockDatabase>("oneParameterConstructor");

            Assert.IsFalse(db.DefaultConstructorCalled);
            Assert.AreEqual("Northwind", db.ConnectionString);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_CanConfigureDependencyParametersForConstructor()
        {
            ObjectWithOneConstructorDependency obj =
                ResolveConfiguredObject<ObjectWithOneConstructorDependency>(
                    "oneDependencyParameterConstructor");

            Assert.IsNotNull(obj.Logger);
            Assert.IsInstanceOfType(obj.Logger, typeof(SpecialLogger));
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_oneDependencyParameterConstructorForNamedInstance()
        {
            ObjectWithOneConstructorDependency obj =
                ResolveConfiguredObject<ObjectWithOneConstructorDependency>(
                    "oneDependencyParameterConstructorForNamedInstance", "MyInstance");

            Assert.IsNotNull(obj.Logger);
            Assert.IsInstanceOfType(obj.Logger, typeof(SpecialLogger));
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_oneDependencyParameterConstructorForNamedInstance1()
        {
            ObjectWithOneConstructorDependency obj =
                ResolveConfiguredObject<ObjectWithOneConstructorDependency>(
                    "oneDependencyParameterConstructorForNamedInstance", "MyInstance");

            Assert.IsNotNull(obj.Logger);
            Assert.IsInstanceOfType(obj.Logger, typeof(SpecialLogger));
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_CanConfigureMultipleConstructorParameters()
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
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_CanConfigureTwoSameDependentConstructorParameters()
        {
            ObjectWithTwoConstructorParameters obj =
                ResolveConfiguredObject<ObjectWithTwoConstructorParameters>(
                    "twoSameDependencyConstructorParameters");

            Assert.IsNotNull(obj.Logger1);
            Assert.IsNotNull(obj.Logger2);
            Assert.IsInstanceOfType(obj.Logger1, typeof(MockLogger));
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_CanConfigureOnePropertyAndOneMethod()
        {
            ObjectWithInjectionMethod obj =
                ResolveConfiguredObject<ObjectWithInjectionMethod>(
                    "ObjectWithInjectionMethodContainer");

            Assert.IsNotNull(obj.Service);
            Assert.IsInstanceOfType(obj.Service, typeof(EmailService));
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_CanConfigureTwoDifferentDependentConstructorParameters()
        {
            ObjectWithTwoConstructorParameters obj =
                ResolveConfiguredObject<ObjectWithTwoConstructorParameters>(
                    "twoDifferentDependentConstructorParameters");

            Assert.IsNotNull(obj.Service);
            Assert.IsNotNull(obj.Logger);

            Assert.IsInstanceOfType(obj.Service, typeof(IService));
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_CanConfigurePropertyInjection()
        {
            ObjectUsingLogger obj = ResolveConfiguredObject<ObjectUsingLogger>("injectionProperty");

            Assert.IsNotNull(obj.Logger);
            Assert.IsInstanceOfType(obj.Logger, typeof(MockLogger));
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_CanConfigureOneIntPropertyInjection()
        {
            ObjectUsingLogger obj = ResolveConfiguredObject<ObjectUsingLogger>("OneIntProperty");

            Assert.IsNotNull(obj.Age);
            Assert.IsInstanceOfType(obj.Age, typeof(int));
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_CanInjectMultipleProperties()
        {
            ObjectWithTwoProperties obj =
                ResolveConfiguredObject<ObjectWithTwoProperties>("multipleProperties");

            Assert.IsInstanceOfType(obj.Obj1, typeof(SpecialLogger));
            Assert.AreEqual("Hello", obj.Obj2);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_CanInjectTwoProperties()
        {
            ObjectWithTwoProperties obj =
                ResolveConfiguredObject<ObjectWithTwoProperties>("TwoProperties");

            Assert.IsInstanceOfType(obj.Obj1, typeof(SpecialLogger));
            Assert.IsInstanceOfType(obj.Obj2, typeof(MockLogger));
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_InjectionMethodsAreCalled()
        {
            ObjectWithInjectionMethod obj =
                ResolveConfiguredObject<ObjectWithInjectionMethod>("method");

            Assert.AreEqual(obj.ConnectionString, "contoso");
            Assert.IsInstanceOfType(obj.Logger, typeof(MockLogger));
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_InjectionMethodWithTwoSameParameters()
        {
            ObjectWithInjectionMethod obj =
                ResolveConfiguredObject<ObjectWithInjectionMethod>("methodwithTwoSameInjectionParameters");

            Assert.IsInstanceOfType(obj.Logger1, typeof(MockLogger));
            Assert.IsInstanceOfType(obj.Logger2, typeof(SpecialLogger));
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void ConfigInject_InjectionMethodWithTwoDifferentParameters()
        {
            ObjectWithInjectionMethod obj =
                ResolveConfiguredObject<ObjectWithInjectionMethod>("methodwithTwoDifferentInjectionParameters");

            Assert.IsInstanceOfType(obj.Service, typeof(IService));
            Assert.IsInstanceOfType(obj.Logger, typeof(SpecialLogger));
        }

        private TObj ResolveConfiguredObject<TObj>(string containerName)
        {
            IUnityContainer container = GetContainer(containerName);
            return container.Resolve<TObj>();
        }

        private TObj ResolveConfiguredObject<TObj>(string containerName, string name)
        {
            IUnityContainer container = GetContainer(containerName);
            return container.Resolve<TObj>(name);
        }
    }
}
