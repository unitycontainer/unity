// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


namespace Unity.Tests
{
    /// <summary>
    /// Test Interface.
    /// </summary>
    public interface InjectTestInterface
    {
    }

    /// <summary>
    /// Test class used for injection.
    /// </summary>
    public class InjectTestClass : InjectTestInterface
    {
        private string name = "In InjectTestClass";

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
    }

    /// <summary>
    /// Test class used for injection.
    /// </summary>
    public class InjectTestClass1
    {
        private string name = "In InjectTestClass1";

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public InjectTestClass1(InjectTestClass intobj1)
        {
            intobj1.Name = name;
        }
    }

    /// <summary>
    /// Test class used for injection.
    /// </summary>
    public class InjectTestClass2
    {
        private string name = "In InjectTestClass2";
        private InjectTestClass2 objcircular;

        [InjectionConstructor]
        public InjectTestClass2(InjectTestClass1 objinject1)
        {
            objinject1.Name = name;
        }

        [Dependency]
        public InjectTestClass2 Text
        {
            get { return objcircular; }
            set { objcircular = value; }
        }
    }

    /// <summary>
    /// To check happy flow scenario for property injection
    /// </summary>
    public class TestProperty
    {
    }

    public class TestPropertyClass
    {
        private TestProperty objprop;

        [Dependency]
        public TestProperty ObjProp
        {
            get { return objprop; }
            set { objprop = value; }
        }
    }

    /// <summary>
    /// classes to check property and constructor injection at the same time. i.e.dependency on same type.
    /// </summary>
    public class TestInjectionProp
    {
        public String Strs = "check";
    }

    public class PropertyTestInjection1
    {
        private TestInjectionProp objtest;

        [Dependency]
        public TestInjectionProp PropertyCheck
        {
            get { return objtest; }
            set { objtest = value; }
        }

        public PropertyTestInjection1(TestInjectionProp objtest1)
        {
            objtest1.Strs = "set from constructor";
            objtest = objtest1;
        }
    }

    /// <summary>
    /// Summary description for TestInjection
    /// </summary>
    [TestClass]
    public class TestInjection
    {
        [TestMethod]
        public void PropertyInjectionUseNew_Test()
        {
            InjectTestClass objTest = new InjectTestClass();
            InjectTestClass1 objTest1 = new InjectTestClass1(objTest);
            InjectTestClass2 objTest2 = new InjectTestClass2(objTest1);
            InjectTestClass2 obj1 = objTest2.Text;
        }

        /// <summary>
        /// Check property injection
        /// </summary>
        [TestMethod]
        public void HappyPropertyInjection_Test()
        {
            UnityContainer container = new UnityContainer();
            TestPropertyClass objprop = container.Resolve<TestPropertyClass>();
            TestProperty objtest = objprop.ObjProp;
        }

        /// <summary>
        /// Have implemented - Constructor injection and property injection both.
        /// Once in constructor injection the dependent class gets instantiated, it should not initialize 
        /// the same dependent class again in property injection. Due to this the previously set values are lost
        /// and get initialized again.
        /// Bug ID : 16459
        /// </summary>
        [TestMethod]
        public void ConstPropInjectionSameClass()
        {
            IUnityContainer uc = new UnityContainer();

            uc.RegisterType<TestInjectionProp>(new ContainerControlledLifetimeManager());
            uc.RegisterType<PropertyTestInjection1>(new ContainerControlledLifetimeManager());
            PropertyTestInjection1 objtest1 = uc.Resolve<PropertyTestInjection1>();

            Assert.AreEqual("set from constructor", objtest1.PropertyCheck.Strs);
        }
    }
}