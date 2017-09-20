// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests.ChildContainer
{
    /// <summary>
    /// Summary description for TestChildContainerInterfaceChanges
    /// </summary>
    [TestClass]
    public class ChildContainerInterfaceChangeFixture
    {
        /// <summary>
        /// create parent and child container and then get the parent from child using the property parent.
        /// </summary>
        [TestMethod]
        public void CheckParentContOfChild()
        {
            UnityContainer uc = new UnityContainer();
            IUnityContainer ucchild = uc.CreateChildContainer();
    
            object obj = ucchild.Parent;
            
            Assert.AreSame(uc, obj);
        }

        /// <summary>
        /// Check what do we get when we ask for parent's parent container
        /// </summary>
        [TestMethod]
        public void CheckParentContOfParent()
        {
            UnityContainer uc = new UnityContainer();
            IUnityContainer ucchild = uc.CreateChildContainer();
            
            object obj = uc.Parent;
            
            Assert.IsNull(obj);
        }

        /// <summary>
        /// Check whether child inherits the configuration of the parent container or not using registertype method
        /// </summary>
        [TestMethod]
        public void ChildInheritsParentsConfiguration_RegisterTypeResolve()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<ITestContainer, TestContainer>(new ContainerControlledLifetimeManager());

            IUnityContainer child = parent.CreateChildContainer();
            ITestContainer objtest = child.Resolve<ITestContainer>();

            Assert.IsNotNull(objtest);
            Assert.IsInstanceOfType(objtest, typeof(TestContainer));
        }

        /// <summary>
        /// Check whether child inherits the configuration of the parent container or 
        /// not, using registerinstance method
        /// </summary>
        [TestMethod]
        public void ChildInheritsParentsConfiguration_RegisterInstanceResolve()
        {
            UnityContainer parent = new UnityContainer();
            ITestContainer obj = new TestContainer();
            
            parent.RegisterInstance<ITestContainer>("InParent", obj);

            IUnityContainer child = parent.CreateChildContainer();
            ITestContainer objtest = child.Resolve<ITestContainer>("InParent");

            Assert.IsNotNull(objtest);
            Assert.AreSame(objtest, obj);
        }

        /// <summary>
        /// Check whether child inherits the configuration of the parent container or 
        /// not,using registertype method and then resolveall
        /// </summary>
        [TestMethod]
        public void ChildInheritsParentsConfiguration_RegisterTypeResolveAll()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<ITestContainer, TestContainer>()
                .RegisterType<ITestContainer, TestContainer1>("first")
                .RegisterType<ITestContainer, TestContainer2>("second");

            IUnityContainer child = parent.CreateChildContainer()
                .RegisterType<ITestContainer, TestContainer3>("third");

            List<ITestContainer> list = new List<ITestContainer>(child.ResolveAll<ITestContainer>());
            
            Assert.AreEqual(3, list.Count);
        }

        /// <summary>
        /// Check whether child inherits the configuration of the parent container or 
        /// not, Using registerinstance method and then resolveall
        /// </summary>
        [TestMethod]
        public void ChildInheritsParentsConfiguration_RegisterInstanceResolveAll()
        {
            ITestContainer objdefault = new TestContainer();
            ITestContainer objfirst = new TestContainer1();
            ITestContainer objsecond = new TestContainer2();
            ITestContainer objthird = new TestContainer3();
            UnityContainer parent = new UnityContainer();
            
            parent.RegisterInstance<ITestContainer>(objdefault)
                .RegisterInstance<ITestContainer>("first", objfirst)
                .RegisterInstance<ITestContainer>("second", objsecond);

            IUnityContainer child = parent.CreateChildContainer()
                .RegisterInstance<ITestContainer>("third", objthird);

            List<ITestContainer> list = new List<ITestContainer>(child.ResolveAll<ITestContainer>());
            
            Assert.AreEqual(3, list.Count);
        }

        /// <summary>
        /// Register same type in parent and child and see the behavior
        /// </summary>
        [TestMethod]
        public void RegisterSameTypeInChildAndParentOverriden()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<ITestContainer, TestContainer>();
            IUnityContainer child = parent.CreateChildContainer()
                .RegisterType<ITestContainer, TestContainer1>();

            ITestContainer parentregister = parent.Resolve<ITestContainer>();
            ITestContainer childregister = child.Resolve<ITestContainer>();

            Assert.IsInstanceOfType(parentregister, typeof(TestContainer));
            Assert.IsInstanceOfType(childregister, typeof(TestContainer1));
        }

        /// <summary>
        /// Register type in parent and resolve using child.
        /// Change in parent and changes reflected in child.
        /// </summary>
        [TestMethod]
        public void ChangeInParentConfigurationIsReflectedInChild()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<ITestContainer, TestContainer>();
            IUnityContainer child = parent.CreateChildContainer();

            ITestContainer first = child.Resolve<ITestContainer>();
            parent.RegisterType<ITestContainer, TestContainer1>();
            ITestContainer second = child.Resolve<ITestContainer>();

            Assert.IsInstanceOfType(first, typeof(TestContainer));
            Assert.IsInstanceOfType(second, typeof(TestContainer1));
        }

        /// <summary>
        /// dispose parent container, child should get disposed.
        /// </summary>
        [TestMethod]
        public void WhenDisposingParentChildDisposes()
        {
            UnityContainer parent = new UnityContainer();
            IUnityContainer child = parent.CreateChildContainer();

            TestContainer3 obj = new TestContainer3();
            child.RegisterInstance<TestContainer3>(obj);

            parent.Dispose();
            Assert.IsTrue(obj.WasDisposed);
        }

        /// <summary>
        /// dispose child, check if parent is disposed or not.
        /// </summary>
        [TestMethod]
        public void ParentNotDisposedWhenChildDisposed()
        {
            UnityContainer parent = new UnityContainer();
            IUnityContainer child = parent.CreateChildContainer();
            TestContainer obj1 = new TestContainer();
            TestContainer3 obj3 = new TestContainer3();
            parent.RegisterInstance<TestContainer>(obj1);
            child.RegisterInstance<TestContainer3>(obj3);

            child.Dispose();
            //parent not getting disposed
            Assert.IsFalse(obj1.WasDisposed);

            //child getting disposed.
            Assert.IsTrue(obj3.WasDisposed);
        }

        [TestMethod]
        public void ChainOfContainers()
        {
            UnityContainer parent = new UnityContainer();
            IUnityContainer child = parent.CreateChildContainer();
            IUnityContainer child2 = child.CreateChildContainer();
            IUnityContainer child3 = child2.CreateChildContainer();

            TestContainer obj1 = new TestContainer();

            parent.RegisterInstance<TestContainer>("InParent", obj1);
            child.RegisterInstance<TestContainer>("InChild", obj1);
            child2.RegisterInstance<TestContainer>("InChild2", obj1);
            child3.RegisterInstance<TestContainer>("InChild3", obj1);

            object objresolve = child3.Resolve<TestContainer>("InParent");
            object objresolve1 = parent.Resolve<TestContainer>("InChild3");

            Assert.AreSame(obj1, objresolve);
            
            child.Dispose();
            
            //parent not getting disposed
            Assert.IsTrue(obj1.WasDisposed);
        }

        /// <summary>
        /// Null not allowed for Lifetime in registerinstance. 
        /// </summary>
        [TestMethod]
        public void CheckNullHandledInRegisterInstance()
        {
            UnityContainer parent = new UnityContainer();
            TestContainer obj = new TestContainer();

            AssertHelper.ThrowsException<ArgumentNullException>(() => parent.RegisterInstance<TestContainer>(obj, null));
        }
    }
}