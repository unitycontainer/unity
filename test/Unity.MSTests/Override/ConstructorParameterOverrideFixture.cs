// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System.Configuration;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests.Override
{
    /// <summary>
    /// Summary description for ConstructorParameterOverrideFixture
    /// </summary>
    [TestClass]
    public class ConstructorParameterOverrideFixture : ConfigurationFixtureBase
    {
        private const string ConfigFileName = @"ConfigFiles\ConstructionParameterOverrideFixture.config";
        private const string ContainerName = "ConstructorOverrideTest";

        public ConstructorParameterOverrideFixture() : base(ConfigFileName)
        {
        }

        [TestMethod]
        public void WhenInjectingConstructorWithOverrideAsDifferentPolymorphicType()
        {
            IInterfaceForTypesToInject defaultInjectionParameter = new TypeToInject1(-1);
            IInterfaceForTypesToInject overrideInjectionParameter = new TypeToInject2(9999);
            var container = new UnityContainer()
                .RegisterType<SubjectType1ToInject>(new InjectionConstructor(defaultInjectionParameter));

            var result =
                container.Resolve<SubjectType1ToInject>(new ParameterOverride("injectedObject", overrideInjectionParameter));

            Assert.IsInstanceOfType(result.InjectedObject, typeof(TypeToInject2));
            Assert.AreEqual<int>(9999, result.InjectedObject.Value);
        }

        [TestMethod]
        public void WhenInjectingConstructorWithOverrideAsDifferentPolymorphicTypesAcrossCalls()
        {
            IInterfaceForTypesToInject defaultInjectionParameter = new TypeToInject1(-1);
            IInterfaceForTypesToInject overrideInjectionParameter = new TypeToInject2(9999);
            var container = new UnityContainer()
                .RegisterType<SubjectType1ToInject>(new InjectionConstructor(defaultInjectionParameter));

            var result1 =
                container.Resolve<SubjectType1ToInject>(new ParameterOverride("injectedObject", overrideInjectionParameter));
            var result2 =
                container.Resolve<SubjectType1ToInject>();

            Assert.IsInstanceOfType(result1.InjectedObject, typeof(TypeToInject2));
            Assert.IsInstanceOfType(result2.InjectedObject, typeof(TypeToInject1));
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void NullCheckForParameterOverrideExists()
        {
            IInterfaceForTypesToInject defaultInjectionParameter = new TypeToInject1(-1);

            var container = new UnityContainer()
                .RegisterType<SubjectType1ToInject>("test", new InjectionConstructor(defaultInjectionParameter));

            var result1 =
                container.Resolve<SubjectType1ToInject>("test", null);
        }

        [TestMethod]
        public void CanOverrideMultipleTimes()
        {
            IInterfaceForTypesToInject defaultParameter = new TypeToInject1(9999);

            IInterfaceForTypesToInject overrideInjectionParameter1 = new TypeToInject2(9999);
            IInterfaceForTypesToInject overrideInjectionParameter2 = new TypeToInject3(8888);
            var container = new UnityContainer()
                .RegisterType<SubjectType1ToInject>(new InjectionConstructor(defaultParameter));

            var result1 =
                container.Resolve<SubjectType1ToInject>(new ParameterOverride("injectedObject", overrideInjectionParameter1));

            var result2 =
                container.Resolve<SubjectType1ToInject>(new ParameterOverride("injectedObject", overrideInjectionParameter2));
            var result3 =
                container.Resolve<SubjectType1ToInject>();

            Assert.IsInstanceOfType(result1.InjectedObject, typeof(TypeToInject2));
            Assert.IsInstanceOfType(result2.InjectedObject, typeof(TypeToInject3));
        }

        [TestMethod]
        public void WhenInjectingConstructorWithOverrideAndNoDefault()
        {
            IInterfaceForTypesToInject overrideInjectionParameter1 = new TypeToInject1(9999);
            IInterfaceForTypesToInject overrideInjectionParameter2 = new TypeToInject2(8888);
            var container = new UnityContainer()
                .RegisterType<SubjectType1ToInject>();

            var result1 =
                container.Resolve<SubjectType1ToInject>(new ParameterOverride("injectedObject", overrideInjectionParameter1));

            var result2 =
                container.Resolve<SubjectType1ToInject>(new ParameterOverride("injectedObject", overrideInjectionParameter2));

            Assert.IsInstanceOfType(result1.InjectedObject, typeof(TypeToInject1));
            Assert.IsInstanceOfType(result2.InjectedObject, typeof(TypeToInject2));
        }

        

       

        [TestMethod]
        public void WhenInjectingConstructorWithOverrideWithMaxArgConstructorAndDefault()
        {
            var container = new UnityContainer()
                .RegisterType<SubjectType1ToInject>(new InjectionConstructor(1, "abc"));

            var result =
                container.Resolve<SubjectType1ToInject>(new ParameterOverride("x", 123), new ParameterOverride("y", "Test"));

            Assert.AreEqual<int>(result.X, 123);
            Assert.AreEqual<string>(result.Y, "Test");
        }

        [TestMethod]
        public void WhenInjectingConstructorWithOverridesCollection()
        {
            var container = new UnityContainer()
                .RegisterType<SubjectType1ToInject>(new InjectionConstructor(1, "abc"));
         
            ParameterOverrides overrides = new ParameterOverrides();
            overrides.Add("y", "string");
            overrides.Add("x", 123);

            var result = container.Resolve<SubjectType1ToInject>(overrides);

            Assert.AreEqual<int>(result.X, 123);
            Assert.AreEqual<string>(result.Y, "string");
        }

        [TestMethod]
        public void WhenInjectingConstructorWithContainerControllerLifeTime()
        {
            IInterfaceForTypesToInject defaultParameter = new TypeToInject1(9999);
            IInterfaceForTypesToInject overrideInjectionParameter = new TypeToInject2(8888);
            IInterfaceForTypesToInject overrideInjectionParameter1 = new TypeToInject3(7777);
         
            var container = new UnityContainer()
                .RegisterType<SubjectType1ToInject>(new ContainerControlledLifetimeManager(), new InjectionConstructor(defaultParameter));

            var result1 =
                container.Resolve<SubjectType1ToInject>(new ParameterOverride("injectedObject", overrideInjectionParameter));
            var result2 =
                container.Resolve<SubjectType1ToInject>(new ParameterOverride("injectedObject", overrideInjectionParameter1));
            var result3 =
                container.Resolve<SubjectType1ToInject>();

            Assert.IsInstanceOfType(result1.InjectedObject, typeof(TypeToInject2));
            Assert.IsInstanceOfType(result2.InjectedObject, typeof(TypeToInject2));
            Assert.IsInstanceOfType(result2.InjectedObject, typeof(TypeToInject2));
        }

        [TestMethod]
        public void WhenUsingOverrideForKeyedRegisteration()
        {
            var container = new UnityContainer()
                .RegisterType<SubjectType1ToInject>("Default", new InjectionConstructor(12, "default"));

            var result1 =
                container.Resolve<SubjectType1ToInject>("Default", new ParameterOverride("x", 123), new ParameterOverride("y", "overrideDefault"));
            var result2 =
                container.Resolve<SubjectType1ToInject>("Default");
            Assert.AreEqual<string>(result1.Y, "overrideDefault");
            Assert.AreEqual<string>(result2.Y, "default");
        }

        

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void CanOverrideConstructorDependencyInConfiguration()
        {
            var overriddenTypeToInject = new TypeToInject3(-9999);
        
            IUnityContainer container = GetContainer(ContainerName);
            
            var injectedObjectWithDefault = container.Resolve<SubjectType1ToInject>("TestConstructorOverrideDefaultInConfiguration");
            var overridenInjectedObject = container.Resolve<SubjectType1ToInject>("TestConstructorOverrideDefaultInConfiguration", new ParameterOverride("injectedObject", overriddenTypeToInject));

            Assert.IsInstanceOfType(overridenInjectedObject.InjectedObject, typeof(TypeToInject3));
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void OverrideReturnedOnlyOnceWhenDefaultInConfiguration()
        {
            var overriddenTypeToInject = new TypeToInject3(-9999);

            IUnityContainer container = GetContainer(ContainerName);

            var overridenInjectedObject = container.Resolve<SubjectType1ToInject>("TestConstructorOverrideDefaultInConfiguration", new ParameterOverride("injectedObject", overriddenTypeToInject));
            var injectedObjectWithDefault = container.Resolve<SubjectType1ToInject>("TestConstructorOverrideDefaultInConfiguration");
            
            Assert.IsInstanceOfType(overridenInjectedObject.InjectedObject, typeof(TypeToInject3));
            Assert.IsInstanceOfType(injectedObjectWithDefault.InjectedObject, typeof(TypeToInject1));
        }

        
        [TestMethod]
        public void WhenOverridingContainerHavingMultiplePolymorphicTypes()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType<ISubjectTypeToInject, SubjectType1ToInject>("SubjectType1ToInject", new InjectionConstructor(1, "DefaultValue"));
            container.RegisterType<ISubjectTypeToInject, SubjectType2ToInject>("SubjectType2ToInject", new InjectionConstructor(2, "DefaultValue"));
            container.RegisterType<ISubjectTypeToInject, SubjectType3ToInject>("SubjectType3ToInject", new InjectionConstructor(3, "DefaultValue"));
            
            ParameterOverrides overrides = new ParameterOverrides();
            overrides.Add("x", 4);
            overrides.Add("y", "overriddenValue");

            System.Collections.Generic.IEnumerable<ISubjectTypeToInject> overriddenList1 = container.ResolveAll<ISubjectTypeToInject>(overrides);
            System.Collections.Generic.IEnumerable<ISubjectTypeToInject> defaultList = container.ResolveAll<ISubjectTypeToInject>();
            
            int count = 0;
            foreach (var item in overriddenList1)
            {
                ++count;
                Assert.AreEqual<int>(4, item.X);
                Assert.AreEqual<string>("overriddenValue", item.Y);
            }
            
            Assert.AreEqual<int>(3, count);
            
            count = 0;
            foreach (var item in defaultList)
            {
                ++count;

                Assert.AreEqual<string>("DefaultValue", item.Y);
            }
            
            Assert.AreEqual<int>(3, count);
        }

        [TestMethod]
        public void WhenOverridingWithMultiplePolymorphicTypesAndSomeDoNotHaveMatchingConstructor()
        {
            IInterfaceForTypesToInject defaultInjectedObject = new TypeToInject1(123);
            IInterfaceForTypesToInject overrideInjectedObject = new TypeToInject2(-9999);
            
            IUnityContainer container = new UnityContainer();
            
            container.RegisterType<ISubjectTypeToInject, SubjectType1ToInject>("SubjectType1ToInject", new InjectionConstructor(defaultInjectedObject));
            container.RegisterType<ISubjectTypeToInject, SubjectType2ToInject>("SubjectType2ToInject", new InjectionConstructor(defaultInjectedObject));
            container.RegisterType<ISubjectTypeToInject, SubjectType3ToInject>("SubjectType3ToInject", new InjectionConstructor(1, "abc"));

            System.Collections.Generic.IEnumerable<ISubjectTypeToInject> overriddenList = container.ResolveAll<ISubjectTypeToInject>(new ParameterOverride("injectedObject", overrideInjectedObject));
            System.Collections.Generic.IEnumerable<ISubjectTypeToInject> defaultList = container.ResolveAll<ISubjectTypeToInject>();

            System.Collections.Generic.List<ISubjectTypeToInject> myOverridenList = new System.Collections.Generic.List<ISubjectTypeToInject>(overriddenList);

            int injectedCount = 0;
            int count = 0;
            foreach (var item in myOverridenList)
            {
                count++;
                if (item.InjectedObject != null && item.InjectedObject.Value == -9999)
                {
                    injectedCount++;
                }
            }

            Assert.AreEqual<int>(2, injectedCount);
            Assert.AreEqual<int>(3, count);
        }

        private Configuration OpenConfigFile(string fileName)
        {
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = fileName;
            return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
        }
    }
}