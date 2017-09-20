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
    [TestClass]
    public class ConstructorInjectionFixture
    {
        [TestMethod]
        public void ConstPropInjectionSameClassTwice()
        {
            IUnityContainer uc = new UnityContainer();
            uc.RegisterType<InjectTestClass2_Prop>()
                .RegisterType<InjectTestClass_Prop>();
            InjectTestClass2_Prop obj_test = uc.Resolve<InjectTestClass2_Prop>();
            obj_test.Propertyfirst.NameInjectTestClassProp = "First";
            obj_test.Propertysecond.NameInjectTestClassProp = "Second";

            Assert.AreEqual("Second", obj_test.Propertysecond.NameInjectTestClassProp);
            Assert.AreEqual("First", obj_test.Propertyfirst.NameInjectTestClassProp);
        }

        [TestMethod]// log Bug 
        public void TestMethodwithParamaterAsInterface()
        {
            IUnityContainer uc = new UnityContainer();
            uc.RegisterType<InjectionTestInterface, InjectionTestClass2>()
                    .RegisterType<InjectionTestInterface, InjectionTestClass2>("hi")
                       .RegisterType<InjectionTestInterface, InjectionTestClass1>()
                        .RegisterType<InjectionTestInterface, InjectionTestClass1>("hello");

            InjectionTestInterface obj3 = uc.Resolve<InjectionTestInterface>("hi");
            InjectionTestInterface obj2 = uc.Resolve<InjectionTestInterface>("hello");
        }
    }

    /// <summary>
    /// ConstructorInjectionFixture class used for property injection testing.
    /// </summary>    
    public class InjectTestClass_Prop
    {
        private string nameInjectTestClassProp = "In InjectTestClass1_Prop";

        public string NameInjectTestClassProp
        {
            get { return this.nameInjectTestClassProp; }
            set { nameInjectTestClassProp = value; }
        }
    }

    /// <summary>
    /// ConstructorInjectionFixture class used for injection.
    /// </summary>
    public class InjectTestClass2_Prop
    {
        public string Name = "In InjectTestClass2_Prop";
        private InjectTestClass_Prop objinject1;
        private InjectTestClass_Prop objinject2;

        public InjectTestClass_Prop Propertyfirst
        {
            get { return objinject1; }
            set { objinject1 = value; }
        }

        public InjectTestClass_Prop Propertysecond
        {
            get { return objinject2; }
            set { objinject2 = value; }
        }

        //InjectTestClass_Prop objinject1_created, InjectTestClass_Prop objinject2_created
        [InjectionConstructor]
        public InjectTestClass2_Prop(InjectTestClass_Prop objinject1_created, InjectTestClass_Prop objinject2_created)
        {
            objinject1 = objinject1_created;
            objinject2 = objinject2_created;
            objinject1.NameInjectTestClassProp = "First time call";
            objinject2.NameInjectTestClassProp = "Second time call";
        }
    }

    #region InjectClass

    public interface InjectionTestInterface
    {
        string Text
        {
            get;
            set;
        }
    }

    public class InjectionTestClass : InjectionTestInterface
    {
        private string name = "InjectionTestClass";
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string Text
        {
            get { return this.name; }
            set { this.name = value; }
        }
    }

    public class InjectionTestClass1 : InjectionTestInterface
    {
        private string name = "InjectionTestClass1";

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string Text
        {
            get { return this.name; }
            set { this.name = value; }
        }
    }

    public class InjectionTestClass2 : InjectionTestInterface
    {
        private string name = "InjectionTestClass2";
        private InjectionTestInterface intobj;

        public InjectionTestClass2(InjectionTestInterface intobj1)
        {
            this.intobj = intobj1;
            intobj1.Text = "Hello";
        }

        public string Text
        {
            get { return this.name; }
            set { this.name = value; }
        }
    }

    #endregion InjectClass
}
