// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Container.Register.Tests.Stubs;
using Unity.Tests;

namespace Unity.Container.Register.Tests
{
    [TestClass]
    public class UnitySeperateContainer
    {
        private IUnityContainer uc1 = new UnityContainer();

        [TestMethod]
        public void RegisterTypeWithoutNameandGet()
        {
            IUnityContainer uc = new UnityContainer().RegisterType<ITemporary, Temp>();
            object obj1 = uc.Resolve<ITemporary>();

            Assert.IsNotNull(uc);
            Assert.IsInstanceOfType(obj1, typeof(Temp));
        }

        [TestMethod]
        public void RegisterTypeWithoutNameandGetAll()
        {
            IUnityContainer uc = new UnityContainer().RegisterType<ITemporary, Temp>().RegisterType<ITemporary, Temporary>();

            List<ITemporary> temporaries = new List<ITemporary>(uc.ResolveAll<ITemporary>());

            Assert.AreEqual(0, temporaries.Count);
        }

        [TestMethod]
        public void RegisterSameType()
        {
            IUnityContainer uc = new UnityContainer()
                .RegisterType<ITemporary, Temp>("First");

            List<ITemporary> temporaries = new List<ITemporary>(uc.ResolveAll<ITemporary>());

            Assert.AreEqual(1, temporaries.Count);

            uc.RegisterType<ITemporary, Temp2>("First");
            temporaries = new List<ITemporary>(uc.ResolveAll<ITemporary>());

            Assert.AreEqual(1, temporaries.Count);
        }

        [TestMethod]
        public void RegisterTypeAndCheckOrder()
        {
            IUnityContainer uc = new UnityContainer()
                .RegisterType<ITemporary, Temp>("First")
                .RegisterType<ITemporary, Temp>()
                .RegisterType<ITemporary, Temporary>("Second");
            List<ITemporary> temporaries = new List<ITemporary>(uc.ResolveAll<ITemporary>());

            Assert.AreEqual(2, temporaries.Count);

            bool caseOne = (temporaries[0] is Temp) && (temporaries[1] is Temporary);
            bool caseTwo = (temporaries[0] is Temporary) && (temporaries[1] is Temp);

            Assert.IsTrue(caseOne || caseTwo);
        }

        [TestMethod]
        public void DuplicateRegisterType()
        {
            IUnityContainer uc = new UnityContainer()
                .RegisterType<ITemporary, Temp>("First")
                .RegisterType<MyBaseClass, MyClassDerivedBaseClass>("First")
                .RegisterType<ITemporary, Temp2>("First");

            object obj1 = uc.Resolve<ITemporary>("First");
            object obj2 = uc.Resolve<MyBaseClass>("First");

            Assert.IsInstanceOfType(obj1, typeof(Temp2));
            Assert.IsInstanceOfType(obj2, typeof(MyBaseClass));
        }

        [TestMethod]
        public void SpecialCharRegisterType()
        {
            IUnityContainer uc = new UnityContainer()
                .RegisterType<ITemporary, Temp>("My'First")
                .RegisterType<MyBaseClass, MyClassDerivedBaseClass>("First")
                .RegisterType<ITemporary, Temp2>("First");

            object obj1 = uc.Resolve<ITemporary>("My'First");
            object obj2 = uc.Resolve<MyBaseClass>("First");

            Assert.IsInstanceOfType(obj1, typeof(Temp));
            Assert.IsInstanceOfType(obj2, typeof(MyBaseClass));
        }

        [TestMethod]
        public void RegisterNamedTypesandGetThroughConfig()
        {
            IUnityContainer uc = new UnityContainer();
            uc.RegisterType<ITemporary, Temp>();
            uc.RegisterType<ITemporary, Temporary>("Temporary");

            //TODO: what does this prove?
            string str2 = ConfigurationManager.AppSettings["TypeName2"].ToString();
            ITemporary temp = uc.Resolve<ITemporary>(str2);

            Assert.IsInstanceOfType(temp, typeof(Temporary));
        }

        [TestMethod]
        public void GetInterfaceUsingNonGenericMethods()
        {
            IUnityContainer uc = new UnityContainer()
                .RegisterType(typeof(ITemporary), typeof(Temp), "First")
                .RegisterType(typeof(ITemporary), typeof(Temporary));

            ITemporary temp = uc.Resolve(typeof(ITemporary), "First") as ITemporary;
            ITemporary temporary = uc.Resolve(typeof(ITemporary)) as ITemporary;

            Assert.IsNotNull(temp);
            Assert.IsNotNull(temporary);

            Assert.IsInstanceOfType(temp, typeof(Temp));
            Assert.IsInstanceOfType(temporary, typeof(Temporary));
        }

        [TestMethod]
        public void GetObjectsTypesasDependencyKeys()
        {
            IUnityContainer uc = new UnityContainer()
                .RegisterType<MyBaseClass, MyClassDerivedBaseClass>();

            MyBaseClass mclass = uc.Resolve<MyBaseClass>();
            MyClassDerivedBaseClass mder = uc.Resolve<MyClassDerivedBaseClass>();

            Assert.IsInstanceOfType(mclass, typeof(MyBaseClass));
            Assert.IsInstanceOfType(mder, typeof(MyBaseClass));
        }

        [TestMethod]
        public void GetObjectsUsingNonGenericMethods()
        {
            IUnityContainer uc = new UnityContainer()
                .RegisterType(typeof(MyBaseClass), typeof(MyClassDerivedBaseClass));

            object mclass = uc.Resolve(typeof(MyBaseClass));

            Assert.IsInstanceOfType(mclass, typeof(MyBaseClass));
        }

        [TestMethod]
        public void CheckMethodInjectionWorks()
        {
            IUnityContainer uc = new UnityContainer().RegisterInstance<string>("hello");

            MyMethodDependencyClass obj1 = uc.Resolve<MyMethodDependencyClass>();

            Assert.AreSame("hello", obj1.MyObj);
        }

        public void WorkerThreadFunction()
        {
            int i;
            i = 0;
            while (i < 10000)
            {
                uc1.RegisterType<ITemporary, Temp>(i.ToString());

                i += 1;
            }
        }

        [TestMethod]
        public void SingletonsAreSame()
        {
            IUnityContainer uc = new UnityContainer()
                    .RegisterType<object>(new ContainerControlledLifetimeManager());
            object obj1 = uc.Resolve<object>();
            object obj2 = uc.Resolve<object>();

            Assert.AreSame(obj1, obj2);
            Assert.IsInstanceOfType(obj1.GetType(), typeof(object));
        }

        [TestMethod]
        public void SingletonInterfaceGetClass()
        {
            IUnityContainer uc = new UnityContainer()
                            .RegisterType<ITemporary, Temp>()
                            .RegisterType<ITemporary, Temp>()
                            .RegisterType<object, Temp>();

            object obj1 = uc.Resolve<Temp>();
            List<ITemporary> temporaries = new List<ITemporary>(uc.ResolveAll<ITemporary>());

            Assert.AreEqual(0, temporaries.Count);
        }

        [TestMethod]
        public void NamedUnnamedSingletonareNotSame()
        {
            IUnityContainer uc = new UnityContainer()
                    .RegisterType<object>(new ContainerControlledLifetimeManager())
                    .RegisterType<object>("MyObject", new ContainerControlledLifetimeManager())
                    .RegisterType<object>("MyObject1", new ContainerControlledLifetimeManager());

            object obj1 = uc.Resolve<object>();
            object obj2 = uc.Resolve<object>("MyObject");

            Assert.AreNotSame(obj1, obj2);
        }

        [TestMethod]
        public void SingletonsWithoutRegistering()
        {
            IUnityContainer uc = new UnityContainer()
                    .RegisterType<MyFirstSingetonclass>(new ContainerControlledLifetimeManager());

            MyFirstSingetonclass firstObj = uc.Resolve<MyFirstSingetonclass>();
            MyFirstSingetonclass secondObj = uc.Resolve<MyFirstSingetonclass>();

            Assert.AreSame(firstObj, secondObj);
        }

        [TestMethod]
        public void RegisterSingletons()
        {
            IUnityContainer uc = new UnityContainer()
                    .RegisterType<IMySingeltonInterface, MyFirstSingetonclass>(new ContainerControlledLifetimeManager());

            IMySingeltonInterface firstObj = uc.Resolve<IMySingeltonInterface>();
            IMySingeltonInterface secondObj = uc.Resolve<IMySingeltonInterface>();

            Assert.AreSame(firstObj, secondObj);
        }

        [TestMethod]
        public void RegisterTwoNonNamedSingletonsOverwriteFirst()
        {
            IUnityContainer uc = new UnityContainer()
                .RegisterType<IMySingeltonInterface, MyFirstSingetonclass>(
                new ContainerControlledLifetimeManager())
                .RegisterType<IMySingeltonInterface, MySecondSingetonclass>(
                new ContainerControlledLifetimeManager());

            object firstObj = uc.Resolve<IMySingeltonInterface>();

            Assert.IsInstanceOfType(firstObj, typeof(MySecondSingetonclass));
        }

        [TestMethod]
        public void RegisterNamedNonNamedSingletons()
        {
            IUnityContainer uc = new UnityContainer()
                    .RegisterType<IMySingeltonInterface, MyFirstSingetonclass>("First", new ContainerControlledLifetimeManager())
                    .RegisterType<IMySingeltonInterface, MySecondSingetonclass>(new ContainerControlledLifetimeManager());

            object firstObj = uc.Resolve<IMySingeltonInterface>();

            Assert.IsInstanceOfType(firstObj, typeof(MySecondSingetonclass));
        }

        [TestMethod]
        public void RegisterClassWithConstError()
        {
            IUnityContainer uc = new UnityContainer();

            uc.RegisterType<MyTypeSecond, MyTypeSecond>("Second");
            MyTypeSecond myTestSecond = uc.Resolve<MyTypeSecond>("Wrong");
        }

        [TestMethod]
        public void RegisterInstanceSingletonsChckwithScott()
        {
            MyFirstSingetonclass first = new MyFirstSingetonclass();

            IUnityContainer uc = new UnityContainer()
                    .RegisterInstance<IMySingeltonInterface>("first", first)
                    .RegisterType<IMySingeltonInterface, MySecondSingetonclass>(new ContainerControlledLifetimeManager());

            object firstObj = uc.Resolve<IMySingeltonInterface>();
            object secondObj = uc.Resolve<IMySingeltonInterface>();

            Assert.IsInstanceOfType(firstObj, typeof(MySecondSingetonclass));
        }

        [TestMethod]
        public void RegisterClassWithTypo()
        {
            IUnityContainer uc = new UnityContainer();

            uc.RegisterType<MyTypeSecond, MyTypeSecond>("Typo");
            MyTypeSecond secObj = uc.Resolve<MyTypeSecond>("Typo");
            secObj.MyObj = "Property Val";
            MyTypeSecond myTestSecond = uc.Resolve<MyTypeSecond>("typo");

            Assert.IsNotNull(myTestSecond);
        }

        [TestMethod]
        public void RegisterResolveDefaultTypeWithLifeTime()
        {
            UnityContainer uc = new UnityContainer();
            uc.RegisterType<Temp>(new ContainerControlledLifetimeManager());
            Temp temp = uc.Resolve<Temp>();
        }

        [TestMethod]
        [DeploymentItem(@"ConfigFiles\ContainerRegisterFixture.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        public void ConfigurationUnityTest()
        {
            IUnityContainer container = ConfigurationFixtureBase.GetContainer(@"ConfigFiles\ContainerRegisterFixture.config", "containerOne");

            Temp objTemp = container.Resolve<Temp>("MyInstance1");
            Temp objTemp1 = container.Resolve<Temp>("MyInstance2");

            Assert.IsInstanceOfType(objTemp, typeof(Temp));
        }

        [TestMethod]
        public void GenericConstructorTest()
        {
            UnityContainer uc = new UnityContainer();
            uc.RegisterType<TempGeneric<object>>("ss");
            TempGeneric<object> obj = uc.Resolve<TempGeneric<object>>("ss");

            Assert.IsNotNull(obj);
        }
    }
}