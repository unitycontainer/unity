// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using Unity.Tests.TestDoubles;

namespace Unity.Tests.Lifetime
{
    /// <summary>
    /// Summary description for MyTest
    /// </summary>
    [TestClass]
    public class LifetimeFixture
    {
        /// <summary>
        /// Registering a type twice with SetSingleton method. once with default and second with name.
        /// </summary>
        [TestMethod]
        public void CheckSetSingletonDoneTwice()
        {
            IUnityContainer uc = new UnityContainer();
            
            uc.RegisterType<A>(new ContainerControlledLifetimeManager())
                .RegisterType<A>("hello", new ContainerControlledLifetimeManager());
            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
            
            Assert.AreNotSame(obj, obj1);
        }

        /// <summary>
        /// Registering a type twice with SetSingleton method. once with default and second with name.
        /// </summary>
        [TestMethod]
        public void CheckRegisterInstanceDoneTwice()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            uc.RegisterInstance<A>(aInstance).RegisterInstance<A>("hello", aInstance);
            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
            
            Assert.AreSame(obj, aInstance);
            Assert.AreSame(obj1, aInstance);
            Assert.AreSame(obj, obj1);
        }

        /// <summary>
        /// Registering a type as singleton and handling its lifetime. Using SetLifetime method.
        /// </summary>
        [TestMethod]
        public void SetLifetimeTwiceWithLifetimeHandle()
        {
            IUnityContainer uc = new UnityContainer();
            
            uc.RegisterType<A>(new ContainerControlledLifetimeManager())
                .RegisterType<A>("hello", new ExternallyControlledLifetimeManager());
            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
            
            Assert.AreNotSame(obj, obj1);
        }

        /// <summary>
        /// SetSingleton class A. Then register instance of class A twice. once by default second by name.
        /// </summary>
        [TestMethod]
        public void SetSingletonRegisterInstanceTwice()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            uc.RegisterInstance<A>(aInstance).RegisterInstance<A>("hello", aInstance);
            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
            
            Assert.AreSame(obj, obj1);
        }

        /// <summary>
        /// SetLifetime class A. Then use Get method to get the instances, once without name, second with name.
        /// </summary>
        [TestMethod]
        public void SetLifetimeGetTwice()
        {
            IUnityContainer uc = new UnityContainer();

            uc.RegisterType<A>(new ContainerControlledLifetimeManager());
            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
         
            Assert.AreNotSame(obj, obj1);
        }

        /// <summary>
        /// SetSingleton class A. Then register instance of class A twice. once by default second by name. 
        /// Then SetLifetime once default and then by name.
        /// </summary>
        [TestMethod]
        public void SetSingletonRegisterInstanceTwiceSetLifetimeTwice()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            uc.RegisterInstance<A>(aInstance)
                .RegisterInstance<A>("hello", aInstance)
                .RegisterType<A>(new ContainerControlledLifetimeManager())
                .RegisterType<A>("hello1", new ContainerControlledLifetimeManager());
            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello1");
            
            Assert.AreNotSame(obj, obj1);
        }

        /// <summary>
        /// SetSingleton class A. Then register instance of class A once by default second by name and
        /// again register instance by another name with lifetime control as false.
        /// Then SetLifetime once default and then by name.
        /// </summary>
        [TestMethod]
        public void SetSingletonNoNameRegisterInstanceDiffNames()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            uc.RegisterInstance<A>(aInstance)
                .RegisterInstance<A>("hello", aInstance)
                .RegisterInstance<A>("hi", aInstance, new ExternallyControlledLifetimeManager());

            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
            A obj2 = uc.Resolve<A>("hi");

            Assert.AreSame(obj, obj1);
            Assert.AreSame(obj1, obj2);
        }

        /// <summary>
        /// SetLifetime class A. Then register instance of class A once by default second by name and
        /// again register instance by another name with lifetime control as false.
        /// Then SetLifetime once default and then by name.
        /// </summary>
        [TestMethod]
        public void SetLifetimeNoNameRegisterInstanceDiffNames()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            uc.RegisterType<A>(new ContainerControlledLifetimeManager())
                .RegisterInstance<A>(aInstance)
                .RegisterInstance<A>("hello", aInstance)
                .RegisterInstance<A>("hi", aInstance, new ExternallyControlledLifetimeManager());

            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
            A obj2 = uc.Resolve<A>("hi");
            
            Assert.AreSame(obj, obj1);
            Assert.AreSame(obj1, obj2);
        }

        /// <summary>
        /// SetSingleton class A with name. Then register instance of class A once by default second by name and
        /// again register instance by another name with lifetime control as false.
        /// Then SetLifetime once default and then by name.
        /// </summary>
        [TestMethod]
        public void SetSingletonWithNameRegisterInstanceDiffNames()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            uc.RegisterType<A>("set", new ContainerControlledLifetimeManager())
                .RegisterInstance<A>(aInstance)
                .RegisterInstance<A>("hello", aInstance)
                .RegisterInstance<A>("hi", aInstance, new ExternallyControlledLifetimeManager());

            A obj = uc.Resolve<A>("set");
            A obj1 = uc.Resolve<A>("hello");
            A obj2 = uc.Resolve<A>("hi");
            
            Assert.AreNotSame(obj, obj1);
            Assert.AreSame(obj1, obj2);
            Assert.AreSame(aInstance, obj1);
        }

        /// <summary>
        /// SetLifetime class A with name. Then register instance of class A once by default second by name and
        /// again register instance by another name with lifetime control as false.
        /// Then SetLifetime once default and then by name.
        /// </summary>
        [TestMethod]
        public void SetLifetimeWithNameRegisterInstanceDiffNames()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            uc.RegisterType<A>("set", new ContainerControlledLifetimeManager())
                .RegisterInstance<A>(aInstance)
                .RegisterInstance<A>("hello", aInstance)
                .RegisterInstance<A>("hi", aInstance, new ExternallyControlledLifetimeManager());

            A obj = uc.Resolve<A>("set");
            A obj1 = uc.Resolve<A>("hello");
            A obj2 = uc.Resolve<A>("hi");
            
            Assert.AreNotSame(obj, obj1);
            Assert.AreSame(aInstance, obj1);
            Assert.AreSame(obj1, obj2);
        }

        /// <summary>
        /// SetSingleton class A. Then register instance of class A once by default second by name and
        /// lifetime as true. Then again register instance by another name with lifetime control as true
        /// then register.
        /// Then SetLifetime once default and then by name.
        /// </summary>
        [TestMethod]
        public void SetSingletonClassARegisterInstanceOfAandBWithSameName()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            B bInstance = new B();
            uc.RegisterType<A>(new ContainerControlledLifetimeManager())
                .RegisterInstance<A>(aInstance)
                .RegisterInstance<A>("hello", aInstance)
                .RegisterInstance<B>("hi", bInstance)
                .RegisterInstance<B>("hello", bInstance, new ExternallyControlledLifetimeManager());

            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
            B obj2 = uc.Resolve<B>("hello");
            B obj3 = uc.Resolve<B>("hi");
            
            Assert.AreSame(obj, obj1);
            Assert.AreNotSame(obj, obj2);
            Assert.AreNotSame(obj1, obj2);
            Assert.AreSame(obj2, obj3);
        }

        /// <summary>defect
        /// SetSingleton class A with name. then register instance of A twice. Once by name, second by default.       
        /// </summary>
        [TestMethod]
        public void SetSingletonByNameRegisterInstanceOnit()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            uc.RegisterType<A>("SetA", new ContainerControlledLifetimeManager())
                .RegisterInstance<A>(aInstance)
                .RegisterInstance<A>("hello", aInstance);

            A obj = uc.Resolve<A>("SetA");
            A obj1 = uc.Resolve<A>();
            A obj2 = uc.Resolve<A>("hello");
            
            Assert.AreSame(obj1, obj2);
            Assert.AreNotSame(obj, obj2);
        }

        /// <summary>
        /// Use SetLifetime twice, once with parameter, and without parameter
        /// </summary>
        [TestMethod]
        public void TestSetLifetime()
        {
            IUnityContainer uc = new UnityContainer();

            uc.RegisterType<A>(new ContainerControlledLifetimeManager())
               .RegisterType<A>("hello", new ContainerControlledLifetimeManager());

            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
            
            Assert.AreNotSame(obj, obj1);
        }

        /// <summary>
        /// Register class A as singleton then use RegisterInstance to register instance 
        /// of class A.
        /// </summary>
        [TestMethod]
        public void SetSingletonDefaultNameRegisterInstance()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            uc.RegisterType<A>(new ContainerControlledLifetimeManager())
                .RegisterType<A>("SetA", new ContainerControlledLifetimeManager())
                .RegisterInstance<A>(aInstance)
                .RegisterInstance<A>("hello", aInstance)
                .RegisterInstance<A>("hello", aInstance, new ExternallyControlledLifetimeManager());

            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("SetA");
            A obj2 = uc.Resolve<A>("hello");

            Assert.AreNotSame(obj, obj1);
            Assert.AreSame(obj, obj2);
        }

        /// <summary>
        /// Registering a type in both parent as well as child. Now trying to Resolve from both
        /// check if same or diff instances are returned.
        /// </summary>
        [TestMethod]
        public void RegisterWithParentAndChild()
        {
            //create unity container
            UnityContainer parentuc = new UnityContainer();

            //register type UnityTestClass
            parentuc.RegisterType<UnityTestClass>(new ContainerControlledLifetimeManager());

            UnityTestClass mytestparent = parentuc.Resolve<UnityTestClass>();
            mytestparent.Name = "Hello World";
            IUnityContainer childuc = parentuc.CreateChildContainer();
            childuc.RegisterType<UnityTestClass>(new ContainerControlledLifetimeManager());

            UnityTestClass mytestchild = childuc.Resolve<UnityTestClass>();

            Assert.AreNotSame(mytestparent.Name, mytestchild.Name);
        }

        /// <summary>
        /// Verify Lifetime managers. When registered using externally controlled and freed, new instance is 
        /// returned when again resolve is done.
        /// </summary>
        [TestMethod]
        public void UseExternallyControlledLifetime()
        {
            IUnityContainer parentuc = new UnityContainer();

            parentuc.RegisterType<UnityTestClass>(new ExternallyControlledLifetimeManager());

            UnityTestClass parentinstance = parentuc.Resolve<UnityTestClass>();
            parentinstance.Name = "Hello World Ob1";
            parentinstance = null;
            GC.Collect();
            UnityTestClass parentinstance1 = parentuc.Resolve<UnityTestClass>();

            Assert.AreSame("Hello", parentinstance1.Name);
        }

        /// <summary>
        /// Verify Lifetime managers. When registered using externally controlled. Should return me with new 
        /// instance every time when asked by Resolve.
        /// Bug ID : 16372
        /// </summary>
        [TestMethod]
        public void UseExternallyControlledLifetimeResolve()
        {
            IUnityContainer parentuc = new UnityContainer();
            parentuc.RegisterType<UnityTestClass>(new ExternallyControlledLifetimeManager());

            UnityTestClass parentinstance = parentuc.Resolve<UnityTestClass>();
            parentinstance.Name = "Hello World Ob1";

            UnityTestClass parentinstance1 = parentuc.Resolve<UnityTestClass>();

            Assert.AreSame(parentinstance.Name, parentinstance1.Name);
        }

        /// <summary>
        /// Verify Lifetime managers. When registered using container controlled and freed, even then
        /// same instance is returned when asked for Resolve.
        /// </summary>
        [TestMethod]
        public void UseContainerControlledLifetime()
        {
            UnityTestClass obj1 = new UnityTestClass();

            obj1.Name = "InstanceObj";

            UnityContainer parentuc = new UnityContainer();
            parentuc.RegisterType<UnityTestClass>(new ContainerControlledLifetimeManager());

            UnityTestClass parentinstance = parentuc.Resolve<UnityTestClass>();
            parentinstance.Name = "Hello World Ob1";
            parentinstance = null;
            GC.Collect();

            UnityTestClass parentinstance1 = parentuc.Resolve<UnityTestClass>();

            Assert.AreSame("Hello World Ob1", parentinstance1.Name);
        }

        /// <summary>
        /// The Resolve method returns the object registered with the named mapping, 
        /// or raises an exception if there is no mapping that matches the specified name. Testing this scenario
        /// Bug ID : 16371
        /// </summary>
        [TestMethod]
        public void TestResolveWithName()
        {
            IUnityContainer uc = new UnityContainer();

            UnityTestClass obj = uc.Resolve<UnityTestClass>("Hello");

            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void TestEmpty()
        {
            UnityContainer uc1 = new UnityContainer();

            uc1.RegisterType<ATest>(new ContainerControlledLifetimeManager());
            uc1.RegisterType<ATest>(String.Empty, new ContainerControlledLifetimeManager());
            uc1.RegisterType<ATest>(null, new ContainerControlledLifetimeManager());

            ATest a = uc1.Resolve<ATest>();
            ATest b = uc1.Resolve<ATest>(String.Empty);
            ATest c = uc1.Resolve<ATest>((string)null);

            Assert.AreEqual(a, b);
            Assert.AreEqual(b, c);
            Assert.AreEqual(a, c);
        }

        [TestMethod]
        public void RegisterInstanceLifetimeManagerNullTest()
        {
            ATTest obj1 = new ATTest();
            obj1.Strtest = "obj1";

            UnityContainer uc = new UnityContainer();
            AssertHelper.ThrowsException<ArgumentNullException>(() => uc.RegisterInstance("obj2", obj1, null));
        }

        [TestMethod]
        public void Test_TaskID_16777()
        {
            UnityContainer uc = new UnityContainer();

            uc.RemoveAllExtensions();
            uc.AddExtension(new MyExtension());
            uc.RegisterType<UnityTestClass>(new ContainerControlledLifetimeManager());

            UnityTestClass mytestparent = uc.Resolve<UnityTestClass>();
        }
    }
}