// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests.Generics
{
    [TestClass]
    public class GenericParameterConfigurationFixture : ConfigurationFixtureBase
    {
        private const string ConfigFileName = @"ConfigFiles\GenericParameterConfigurationTests.config";

        public GenericParameterConfigurationFixture()
            : base(ConfigFileName)
        { }

        /// <summary>
        /// Verifies that the default unity ctor is used.
        /// </summary>
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void VerifyCanCreateAGenericTypeThroughTheDefaultCtor()
        {
            IUnityContainer container = GetContainer("VerifyCanCreateAGenericTypeThroughTheDefaultCtor");

            IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> obj;
            obj = container.Resolve<IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.PropT1);
            Assert.IsNotNull(obj.PropT2);
            Assert.IsNull(obj.PropT3);
            Assert.IsNull(obj.PropT4);
        }

        /// <summary>
        /// Verifies that a specific generic ctor can be used
        /// </summary>
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void VerifyCanCreateAGenericTypeThroughASpecifiedGenericConstructor1()
        {
            IUnityContainer container = GetContainer("VerifyCanCreateAGenericTypeThroughASpecifiedGenericConstructor1");

            IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> obj;
            obj = container.Resolve<IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.PropT1);
            Assert.IsNull(obj.PropT2);
            Assert.IsNull(obj.PropT3);
            Assert.IsNull(obj.PropT4);
        }

        /// <summary>
        /// Verifies that a specific generic ctor can be used
        /// </summary>
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void VerifyCanCreateAGenericTypeThroughASpecifiedGenericConstructor2()
        {
            IUnityContainer container = GetContainer("VerifyCanCreateAGenericTypeThroughASpecifiedGenericConstructor2");

            IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> obj;
            obj = container.Resolve<IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(obj);
            Assert.IsNull(obj.PropT1);
            Assert.IsNotNull(obj.PropT2);
            Assert.IsNull(obj.PropT3);
            Assert.IsNull(obj.PropT4);
        }

        /// <summary>
        /// Verifies that a specific generic ctor can be used
        /// </summary>
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void VerifyCanCreateAGenericTypeThroughASpecifiedGenericConstructor3()
        {
            IUnityContainer container = GetContainer("VerifyCanCreateAGenericTypeThroughASpecifiedGenericConstructor3");

            IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> obj;
            obj = container.Resolve<IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.PropT1);
            Assert.IsNotNull(obj.PropT2);
            Assert.IsNull(obj.PropT3);
            Assert.IsNull(obj.PropT4);
        }

        /// <summary>
        /// Verifies that a specific generic ctor can be used
        /// </summary>
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void VerifyCanCreateAGenericTypeFailsThroughASpecifiedGenericConstructorWithInvalidParameterOrder()
        {
            IUnityContainer container = GetContainer("VerifyCanCreateAGenericTypeFailsThroughASpecifiedGenericConstructorWithInvalidParameterOrder");

            IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> obj;
            obj = container.Resolve<IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.PropT1);
            Assert.IsNotNull(obj.PropT2);
            Assert.IsNull(obj.PropT3);
            Assert.IsNull(obj.PropT4);
        }

        /// <summary>
        /// Verifies that a specific generic ctor can be used
        /// </summary>
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void VerifyCanCreateAGenericTypeFailsThroughASpecifiedGenericConstructorWithInvalidGenericTypeName()
        {
            IUnityContainer container = GetContainer("VerifyCanCreateAGenericTypeFailsThroughASpecifiedGenericConstructorWithInvalidGenericTypeName");

            IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> obj;
            obj = container.Resolve<IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.PropT1);
            Assert.IsNotNull(obj.PropT2);
            Assert.IsNull(obj.PropT3);
            Assert.IsNull(obj.PropT4);
        }

        /// <summary>
        /// Verifies that a property can be set
        /// </summary>
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void VerifyCanCreateAGenericTypeWithAConfiguredDependencyProperty1()
        {
            IUnityContainer container = GetContainer("VerifyCanCreateAGenericTypeWithAConfiguredDependencyProperty1");

            IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> obj;
            obj = container.Resolve<IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(obj);
            Assert.IsNull(obj.PropT1);
            Assert.IsNull(obj.PropT2);
            Assert.IsNotNull(obj.PropT3);
            Assert.IsNull(obj.PropT4);
        }

        /// <summary>
        /// Verifies that a property can be set
        /// </summary>
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void VerifyCanCreateAGenericTypeWithAConfiguredDependencyProperty2()
        {
            IUnityContainer container = GetContainer("VerifyCanCreateAGenericTypeWithAConfiguredDependencyProperty2");

            IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> obj;
            obj = container.Resolve<IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(obj);
            Assert.IsNull(obj.PropT1);
            Assert.IsNull(obj.PropT2);
            Assert.IsNotNull(obj.PropT3);
            Assert.IsNotNull(obj.PropT4);
        }

        /// <summary>
        /// Verifies that a injection method can be called
        /// </summary>
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void VerifyCanCreateAGenericTypeWithAInjectionMethod1()
        {
            IUnityContainer container = GetContainer("VerifyCanCreateAGenericTypeWithAInjectionMethod1");

            IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> obj;
            obj = container.Resolve<IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(obj);
            Assert.IsNull(obj.PropT1);
            Assert.IsNull(obj.PropT2);
            Assert.IsNotNull(obj.PropT3);
            Assert.IsNotNull(obj.PropT4);
        }

        /// <summary>
        /// Verifies that a injection method can be called
        /// </summary>
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void VerifyCanCreateAGenericTypeFailsWithAInjectionMethodAndInvalidParamterOrder()
        {
            IUnityContainer container = GetContainer("VerifyCanCreateAGenericTypeFailsWithAInjectionMethodAndInvalidParamterOrder");

            IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> obj;
            obj = container.Resolve<IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(obj);
            Assert.IsNull(obj.PropT1);
            Assert.IsNull(obj.PropT2);
            Assert.IsNotNull(obj.PropT3);
            Assert.IsNotNull(obj.PropT4);
        }

        /// <summary>
        /// Verifies that a injection method can be called
        /// </summary>
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void VerifyCanCreateAGenericTypeWithAInjectionMethod2()
        {
            IUnityContainer container = GetContainer("VerifyCanCreateAGenericTypeWithAInjectionMethod2");

            IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> obj;
            obj = container.Resolve<IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(obj);
            Assert.IsNull(obj.PropT1);
            Assert.IsNull(obj.PropT2);
            Assert.IsNull(obj.PropT3);
            Assert.IsNotNull(obj.PropT4);
        }

        /// <summary>
        /// Verifies that a injection method can be called
        /// </summary>
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void VerifyCanCreateAGenericTypeWithAInjectionMethod3()
        {
            IUnityContainer container = GetContainer("VerifyCanCreateAGenericTypeWithAInjectionMethod3");

            IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> obj;
            obj = container.Resolve<IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.PropT1);
            Assert.IsNull(obj.PropT2);
            Assert.IsNull(obj.PropT3);
            Assert.IsNotNull(obj.PropT4);
        }

        /// <summary>
        /// Verifies that a injection method can be called
        /// </summary>
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void VerifyCanCreateAGenericTypeWithAInjectionMethod3_Closed()
        {
            IUnityContainer container = GetContainer("VerifyCanCreateAGenericTypeWithAInjectionMethod3_Closed");

            IHaveManyGenericTypesClosed obj;
            obj = container.Resolve<IHaveManyGenericTypesClosed>();

            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.PropT1);
            Assert.IsNull(obj.PropT2);
            Assert.IsNull(obj.PropT3);
            Assert.IsNotNull(obj.PropT4);
        }

        /// <summary>
        /// Verifies that a injection method can be called
        /// </summary>
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void VerifyCanCreateAGenericTypeWithAInjectionMethodButWrongParameterName()
        {
            IUnityContainer container = GetContainer("VerifyCanCreateAGenericTypeFailsWithAInjectionMethodButWrongParameterName");

            IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> obj;
            obj = container.Resolve<IHaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void Bug18556_ConstDependecyElement()
        {
            IUnityContainer container = GetContainer("ConstDependecyElement");

            ClassWithConstMethodandProperty<IGenLogger> dep =
                container.Resolve<ClassWithConstMethodandProperty<IGenLogger>>();

            Assert.IsNotNull(dep.Value);
            Assert.IsInstanceOfType(dep.Value, typeof(GenMockLogger));
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void Bug18556_PropertyDependecyElement()
        {
            IUnityContainer container = GetContainer("PropertyDependecyElement");

            ClassWithConstMethodandProperty<IGenLogger> dep = 
                container.Resolve<ClassWithConstMethodandProperty<IGenLogger>>();

            Assert.IsNotNull(dep.Value);
            Assert.IsInstanceOfType(dep.Value, typeof(GenMockLogger));

            string str = dep.Value.ToString();
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void Bug18556_PropertyNoNullDependecyElement()
        {
            IUnityContainer container = GetContainer("PropertyNoNullDependecyElement");

            ClassWithConstMethodandProperty<IGenLogger> dep =
                container.Resolve<ClassWithConstMethodandProperty<IGenLogger>>();

            Assert.IsNotNull(dep.Value);
            Assert.IsInstanceOfType(dep.Value, typeof(GenSpecialLogger));
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void Bug18556_PropertyNoNullDependecyElementThrowException()
        {
            IUnityContainer container = GetContainer("PropertyNoNullDependecyElementThrowException");

            ClassWithConstMethodandProperty<IGenLogger> dep = 
                container.Resolve<ClassWithConstMethodandProperty<IGenLogger>>();
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void Bug18556_MethodDependecyElement()
        {
            IUnityContainer container = GetContainer("MethodDependecyElement");

            ClassWithConstMethodandProperty<IGenLogger> dep = 
                container.Resolve<ClassWithConstMethodandProperty<IGenLogger>>();

            Assert.IsNotNull(dep.Value);
            Assert.IsInstanceOfType(dep.Value, typeof(GenSpecialLogger));

            string str = dep.Value.ToString();
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void Bug18556_MethodNoNullDependecyElement()
        {
            IUnityContainer container = GetContainer("MethodNoNullDependecyElement");

            ClassWithConstMethodandProperty<IGenLogger> dep = 
                container.Resolve<ClassWithConstMethodandProperty<IGenLogger>>();

            Assert.IsNotNull(dep.Value);
            Assert.IsInstanceOfType(dep.Value, typeof(GenSpecialLogger));
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Bug18556_MethodNoNullDependecyElementThrowException()
        {
            IUnityContainer container = GetContainer("MethodNoNullDependecyElementThrowException");

            ClassWithConstMethodandProperty<IGenLogger> dep = 
                container.Resolve<ClassWithConstMethodandProperty<IGenLogger>>();
        }

        /// <summary>
        /// Value through code
        /// </summary>
        [TestMethod]
        public void VerifyCanCreateAGenericTypeWithValueElementWithoutParameterTypeThroughCode()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType<IHaveAGenericType<int>, HaveAGenericType<int>>(new InjectionConstructor(1));
            IHaveAGenericType<int> obj = container.Resolve<IHaveAGenericType<int>>();
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void VerifyCanCreateAGenericTypeWithValueElementWithParameterType()
        {
            IUnityContainer container = GetContainer("VerifyValueElementParameterType");

            IHaveAGenericType<int> obj = container.Resolve<IHaveAGenericType<int>>();
        }

        /// <summary>
        /// Value element without type specified
        /// </summary>
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void VerifyCanCreateAGenericTypeWithValueElementWithoutParameterType()
        {
            IUnityContainer container = GetContainer("VerifyValueElementWithoutParameterType");

            IHaveAGenericType<int> obj = container.Resolve<IHaveAGenericType<int>>();
        }
    }
}
