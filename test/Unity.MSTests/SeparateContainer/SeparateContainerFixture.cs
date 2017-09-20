// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Unity.Tests.SeparateContainer
{
    [TestClass]
    public class SeperateContainerFixture
    {
        [TestMethod]
        public void GetObject()
        {
            UnityContainer uc = new UnityContainer();
            object obj = uc.Resolve<object>();

            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void RecursiveDependencies()
        {
            IUnityContainer uc = new UnityContainer();
            object obj1 = uc.Resolve<MyDependency>();

            Assert.IsNotNull(obj1);
            Assert.IsInstanceOfType(obj1, typeof(MyDependency));
        }

        [TestMethod]
        public void CheckPropertyInjectionWorks()
        {
            IUnityContainer uc = new UnityContainer();
            MySetterInjectionClass obj1 = uc.Resolve<MySetterInjectionClass>();
            
            Assert.IsNotNull(obj1);
            Assert.IsNull(obj1.MyObj);
            Assert.IsInstanceOfType(obj1, typeof(MySetterInjectionClass));
        }

        [TestMethod]
        public void CheckPropertyDependencyInjectionWorks()
        {
            IUnityContainer uc = new UnityContainer();
            MySetterDependencyClass obj1 = uc.Resolve<MySetterDependencyClass>();

            Assert.IsNotNull(obj1);
            Assert.IsNotNull(obj1.MyObj);
            Assert.IsInstanceOfType(obj1, typeof(MySetterDependencyClass));
        }

        [TestMethod]
        public void Check2PropertyDependencyInjectionWorks()
        {
            IUnityContainer uc = new UnityContainer();
            My2PropertyDependencyClass obj1 = uc.Resolve<My2PropertyDependencyClass>();
            
            Assert.IsNotNull(obj1);
            Assert.IsNotNull(obj1.MyFirstObj);
            Assert.IsNotNull(obj1.MySecondObj);
            Assert.IsInstanceOfType(obj1, typeof(My2PropertyDependencyClass));
        }

        [TestMethod]
        public void Check2PropertyDependencyBuildUpWorks()
        {
            UnityContainer uc = new UnityContainer();
            My2PropertyDependencyClass obj1 = new My2PropertyDependencyClass();

            Assert.IsNotNull(obj1);
            Assert.IsNull(obj1.MyFirstObj);
            Assert.IsNull(obj1.MySecondObj);
            
            uc.BuildUp(obj1);
            
            Assert.IsNotNull(obj1.MyFirstObj);
            Assert.IsNotNull(obj1.MySecondObj);
        }

        [TestMethod]
        public void CheckMultipleDependencyNonDependencyInjectionWorks()
        {
            UnityContainer uc = new UnityContainer();
            MySetterDependencyNonDependencyClass obj1 = uc.Resolve<MySetterDependencyNonDependencyClass>();
            
            Assert.IsNotNull(obj1);
            Assert.IsNotNull(obj1.MyObj);
            Assert.IsNull(obj1.MyAnotherObj);
            Assert.IsInstanceOfType(obj1, typeof(MySetterDependencyNonDependencyClass));
        }

        [TestMethod]
        public void TwoInstancesAreNotSame()
        {
            UnityContainer uc = new UnityContainer();
            object obj1 = uc.Resolve<object>();
            object obj2 = uc.Resolve<object>();

            Assert.AreNotSame(obj1, obj2);
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
        public void NamedUnnamedSingletonareNotSame()
        {
            IUnityContainer uc = new UnityContainer()
                .RegisterType<object>(new ContainerControlledLifetimeManager())
                .RegisterType<object>("MyObject", new ContainerControlledLifetimeManager());

            object obj1 = uc.Resolve<object>();
            object obj2 = uc.Resolve<object>("MyObject");
            
            Assert.AreNotSame(obj1, obj2);
        }
    }

    public class MyDependency
    {
        private object myDepObj;

        public MyDependency(object obj)
        {
            this.myDepObj = obj;
        }
    }

    public class MyDependency1
    {
        private object myDepObj;

        public MyDependency1(object obj)
        {
            this.myDepObj = obj;
        }

        [InjectionConstructor]
        public MyDependency1(string str)
        {
            this.myDepObj = str;
        }
    }

    public class MultipleConstructors
    {
        public MultipleConstructors()
        {
            System.Diagnostics.Debug.WriteLine("Default Empty constructor");
        }
        
        public MultipleConstructors(object obj)
        {
            System.Diagnostics.Debug.WriteLine("object constructor");
        }
    }

    public class MySetterInjectionClass
    {
        public object MyObj { get; set; }
    }

    public class MySetterDependencyClass
    {
        [Dependency]
        public object MyObj { get; set; }
    }

    public class MySetterDependencyNonDependencyClass
    {
        [Dependency]
        public object MyObj { get; set; }

        public object MyAnotherObj { get; set; }
    }

    public class My2PropertyDependencyClass
    {
        [Dependency]
        public object MyFirstObj { get; set; }

        [Dependency]
        public object MySecondObj { get; set; }
    }

    public class MyMethodDependencyClass
    {
        private string myObj;

        [InjectionMethod]
        public void Initialize(string obj)
        {
            myObj = obj;
        }

        public object MyObj
        {
            get { return myObj; }
        }
    }

    internal interface IMySingeltonInterface
    {
    }

    internal class MyFirstSingetonclass : IMySingeltonInterface
    {
    }

    internal class MySecondSingetonclass : IMySingeltonInterface
    {
    }

    internal interface IMyClass
    {
    }

    internal class MyBaseClass : IMyClass
    {
    }

    internal class MyClassDerivedBaseClass : MyBaseClass
    {
    }

    internal class MyDisposeClass : IDisposable
    {
        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    internal interface IMyInterface
    {
    }

    internal interface ITemporary
    {
    }
    
    public class Temp : ITemporary
    {
    }

    internal class Temporary : ITemporary
    {
    }
}