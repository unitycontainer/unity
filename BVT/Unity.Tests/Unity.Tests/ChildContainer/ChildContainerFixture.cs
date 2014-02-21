// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Unity.Tests.ChildContainer
{
    /// <summary>
    /// Summary description for UnityChildContainers
    /// </summary>
    [TestClass]
    public class ChildContainerFixture
    {
        [TestMethod]
        public void CreateChildUsingParentsConfiguration()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<ITemporary, Temporary>();
            IUnityContainer child = parent.CreateChildContainer();

            ITemporary temp = child.Resolve<ITemporary>();

            Assert.IsNotNull(temp);
            Assert.IsInstanceOfType(temp, typeof(Temporary));
        }

        [TestMethod]
        public void NamesRegisteredInParentAppearInChild()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<ITemporary, SpecialTemp>("test");
            IUnityContainer child = parent.CreateChildContainer();

            ITemporary temp = child.Resolve<ITemporary>("test");

            Assert.IsInstanceOfType(temp, typeof(SpecialTemp));
        }

        [TestMethod]
        public void NamesRegisteredInParentAppearInChildGetAll()
        {
            string[] numbers = { "first", "second", "third" };
            UnityContainer parent = new UnityContainer();
            parent.RegisterInstance(numbers[0], "first")
                .RegisterInstance(numbers[1], "second");

            IUnityContainer child = parent.CreateChildContainer()
                .RegisterInstance(numbers[2], "third");

            List<string> nums = new List<string>(child.ResolveAll<string>());
            CollectionAssert.AreEquivalent(numbers, nums);
        }

        [TestMethod]
        public void ChildConfigurationOverridesParentConfiguration()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<ITemporary, Temporary>();

            IUnityContainer child = parent.CreateChildContainer()
                .RegisterType<ITemporary, SpecialTemp>();

            ITemporary parentTemp = parent.Resolve<ITemporary>();
            ITemporary childTemp = child.Resolve<ITemporary>();

            Assert.IsInstanceOfType(parentTemp, typeof(Temporary));
            Assert.IsInstanceOfType(childTemp, typeof(SpecialTemp));
        }

        [TestMethod]
        public void DisposingParentDisposesChild()
        {
            UnityContainer parent = new UnityContainer();
            IUnityContainer child = parent.CreateChildContainer();

            MyDisposableObject spy = new MyDisposableObject();
            child.RegisterInstance(spy);
            parent.Dispose();

            Assert.IsTrue(spy.WasDisposed);
        }

        [TestMethod]
        public void CanDisposeChildWithoutDisposingParent()
        {
            MyDisposableObject parentSpy = new MyDisposableObject();
            MyDisposableObject childSpy = new MyDisposableObject();
            UnityContainer parent = new UnityContainer();

            parent.RegisterInstance(parentSpy);
            IUnityContainer child = parent.CreateChildContainer()
                .RegisterInstance(childSpy);
            child.Dispose();

            Assert.IsFalse(parentSpy.WasDisposed);
            Assert.IsTrue(childSpy.WasDisposed);

            childSpy.WasDisposed = false;
            parent.Dispose();

            Assert.IsTrue(parentSpy.WasDisposed);
            Assert.IsFalse(childSpy.WasDisposed);
        }

        [TestMethod]
        public void VerifyToList()
        {
            string[] numbers = { "first", "second", "third" };
            UnityContainer parent = new UnityContainer();

            parent.RegisterInstance(numbers[0], "first")
                .RegisterInstance(numbers[1], "second");
            IUnityContainer child = parent.CreateChildContainer()
                .RegisterInstance(numbers[2], "third");

            List<string> nums = new List<string>(child.ResolveAll<string>());
            CollectionAssert.AreEquivalent(numbers, nums);
        }

        [TestMethod]
        public void ChangesInParentReflectsInChild()
        {
            string[] numbers = { "first", "second", "third", "fourth" };
            UnityContainer parent = new UnityContainer();

            parent.RegisterInstance(numbers[0], "1")
                .RegisterInstance(numbers[1], "2");
            IUnityContainer child = parent.CreateChildContainer();

            List<string> childnums = new List<string>(child.ResolveAll<string>());
            List<string> parentnums = new List<string>(parent.ResolveAll<string>());

            CollectionAssert.AreEquivalent(childnums, parentnums);

            parent.RegisterInstance(numbers[3], "4"); //Register an instance in Parent but not in child

            List<string> childnums2 = new List<string>(child.ResolveAll<string>());
            List<string> parentnums2 = new List<string>(parent.ResolveAll<string>());

            CollectionAssert.AreEquivalent(childnums2, parentnums2); //Both parent child should have same instances
        }

        [TestMethod]
        public void DuplicateRegInParentAndChild()
        {
            string[] numbers = { "first", "second", "third", "fourth" };

            UnityContainer parent = new UnityContainer();
            parent.RegisterInstance(numbers[0], "1")
                .RegisterInstance(numbers[1], "2");

            IUnityContainer child = parent.CreateChildContainer();

            List<string> childnums = new List<string>(child.ResolveAll<string>());
            List<string> parentnums = new List<string>(parent.ResolveAll<string>());

            CollectionAssert.AreEquivalent(childnums, parentnums);

            parent.RegisterInstance(numbers[3], "4");
            child.RegisterInstance(numbers[3], "4");

            List<string> childnums2 = new List<string>(child.ResolveAll<string>());
            List<string> parentnums2 = new List<string>(parent.ResolveAll<string>());

            CollectionAssert.AreEquivalent(childnums2, parentnums2); //Both parent child should have same instances
        }

        [TestMethod]
        public void VerifyArgumentNullException()
        {
            string[] numbers = { "first", "second", "third" };

            UnityContainer parent = new UnityContainer();
            parent.RegisterInstance("1", numbers[0])
                .RegisterInstance("2", numbers[1]);
            IUnityContainer child = parent.CreateChildContainer()
                .RegisterInstance("3", numbers[2]);

            List<string> nums = new List<string>(child.ResolveAll<string>());

            AssertHelper.ThrowsException<ArgumentNullException>(() => Guard.ArgumentNotNull(null, String.Empty));

            CollectionAssert.AreEquivalent(numbers, nums);
        }

        [TestMethod]
        public void CreateParentChildContainersWithSameName()
        {
            IUnityContainer parent = new UnityContainer();

            parent.RegisterType<ITemporary, Temp>("First");
            parent = (UnityContainer)parent.CreateChildContainer();
            parent.RegisterType<ITemporary, Temp>("First");

            List<ITemporary> count = new List<ITemporary>(parent.ResolveAll<ITemporary>());

            Assert.AreEqual(1, count.Count);
        }

        [TestMethod]
        public void MoreChildContainers1()
        {
            UnityContainer parent = new UnityContainer();

            parent.RegisterType<ITemporary, Temp>("First");
            parent.RegisterType<ITemporary, Temp>("First");
            UnityContainer child1 = (UnityContainer)parent.CreateChildContainer();
            child1.RegisterType<ITemporary, Temp>("First");
            child1.RegisterType<ITemporary, Temp>("First");
            UnityContainer child2 = (UnityContainer)child1.CreateChildContainer();
            child2.RegisterType<ITemporary, Temp>("First");
            child2.RegisterType<ITemporary, Temp>("First");
            UnityContainer child3 = (UnityContainer)child2.CreateChildContainer();
            child3.RegisterType<ITemporary, Temp>("First");
            child3.RegisterType<ITemporary, Temp>("First");
            IUnityContainer child4 = child3.CreateChildContainer();
            child4.RegisterType<ITemporary, Temp>("First");
            ITemporary first = child4.Resolve<ITemporary>("First");

            child4.RegisterType<ITemporary, Temp>("First", new ContainerControlledLifetimeManager());
            List<ITemporary> count = new List<ITemporary>(child4.ResolveAll<ITemporary>());

            Assert.AreEqual(1, count.Count);
        }

        [TestMethod]
        public void MoreChildContainers2()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<ITemporary, Temp>("First", new HierarchicalLifetimeManager());

            UnityContainer child1 = (UnityContainer)parent.CreateChildContainer();
            child1.RegisterType<ITemporary, Temp>("First", new HierarchicalLifetimeManager());
            var result = parent.Resolve<ITemporary>("First");
            var result1 = child1.Resolve<ITemporary>("First");

            Assert.AreNotEqual<int>(result.GetHashCode(), result1.GetHashCode());

            UnityContainer child2 = (UnityContainer)child1.CreateChildContainer();
            child2.RegisterType<ITemporary, Temp>("First", new HierarchicalLifetimeManager());
            var result2 = child2.Resolve<ITemporary>("First");

            Assert.AreNotEqual<int>(result.GetHashCode(), result2.GetHashCode());
            Assert.AreNotEqual<int>(result1.GetHashCode(), result2.GetHashCode());

            List<ITemporary> count = new List<ITemporary>(child2.ResolveAll<ITemporary>());

            Assert.AreEqual(1, count.Count);
        }

        [TestMethod]
        public void GetObjectAfterDispose()
        {
            UnityContainer parent = new UnityContainer();
            parent.RegisterType<Temp>("First", new ContainerControlledLifetimeManager());

            IUnityContainer child = parent.CreateChildContainer();
            child.RegisterType<ITemporary>("First", new ContainerControlledLifetimeManager());
            parent.Dispose();
            AssertHelper.ThrowsException<ResolutionFailedException>(() => child.Resolve<ITemporary>("First"));
        }

        [TestMethod]
        public void VerifyArgumentNotNullOrEmpty()
        {
            string[] numbers = { "first", "second", "third" };

            UnityContainer parent = new UnityContainer();
            parent.RegisterInstance("1", numbers[0])
                .RegisterInstance("2", numbers[1]);
            IUnityContainer child = parent.CreateChildContainer()
                .RegisterInstance("3", numbers[2]);
            List<string> nums = new List<string>(child.ResolveAll<string>());

            AssertHelper.ThrowsException<ArgumentException>(() => Guard.ArgumentNotNullOrEmpty(String.Empty, String.Empty));
            
            CollectionAssert.AreEquivalent(numbers, nums);
        }

        [TestMethod]
        public void VerifyArgumentNotNullOrEmpty1()
        {
            string[] numbers = { "first", "second", "third" };

            UnityContainer parent = new UnityContainer();
            parent.RegisterInstance("1", numbers[0])
                .RegisterInstance("2", numbers[1]);
            IUnityContainer child = parent.CreateChildContainer()
                .RegisterInstance("3", numbers[2]);
            List<string> nums = new List<string>(child.ResolveAll<string>());

            AssertHelper.ThrowsException<ArgumentNullException>(() => Guard.ArgumentNotNullOrEmpty(null, null));

            CollectionAssert.AreEquivalent(numbers, nums);
        }

        [TestMethod]
        public void VerifyArgumentNotNullOrEmpty2()
        {
            string[] numbers = { "first", "second", "third" };

            UnityContainer parent = new UnityContainer();
            parent.RegisterInstance("1", numbers[0])
                .RegisterInstance("2", numbers[1]);
            IUnityContainer child = parent.CreateChildContainer()
                .RegisterInstance("3", numbers[2]);
            List<string> nums = new List<string>(child.ResolveAll<string>());

            Guard.ArgumentNotNullOrEmpty("first", "numbers");
            
            CollectionAssert.AreEquivalent(numbers, nums);
        }

        //bug # 3978 http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=6053
        [TestMethod]
        public void ChildParentRegisrationOverlapTest()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterInstance("str1", "string1");
            container.RegisterInstance("str2", "string2");

            IUnityContainer child = container.CreateChildContainer();

            child.RegisterInstance("str2", "string20");
            child.RegisterInstance("str3", "string30");

            var childStrList = child.ResolveAll<string>();
            var parentStrList = container.ResolveAll<string>();
            string childString = String.Empty;
            string parentString = String.Empty;

            foreach (string str in childStrList) { childString = childString + str; }
            foreach (string str1 in parentStrList) { parentString = parentString + str1; }

            Assert.AreEqual<string>("string1string20string30", childString);
            Assert.AreEqual<string>("string1string2", parentString);
        }

        public interface ITemporary
        {
        }

        public class Temp : ITemporary
        {
        }

        public class Temporary : ITemporary
        {
        }

        public class SpecialTemp : ITemporary //Second level
        {
        }

        public class MyDisposableObject : IDisposable
        {
            private bool wasDisposed = false;

            public bool WasDisposed
            {
                get { return wasDisposed; }
                set { wasDisposed = value; }
            }

            public void Dispose()
            {
                wasDisposed = true;
            }
        }
    }
}
