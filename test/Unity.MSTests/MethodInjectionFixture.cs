// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections;

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
    /// Summary description for TestMethodInjection
    /// </summary>
    [TestClass]
    public class MethodInjectionFixture
    {
        /// <summary>
        /// Creating unity container and trying to resolve class TestVirtual which has method injection
        /// on unitycontainer.
        /// </summary>
        [TestMethod]
        public void TestMethodInjectionAndResolveInIt()
        {
            IUnityContainer uc = new UnityContainer();
            try
            {
                TestVirtual objvirtual = uc.Resolve<TestVirtual>();
            }
            catch (Exception ex)
            {
                Assert.Fail("Bug#16494: " + ex.Message);
            }
        }

        [TestMethod]
        public void HappyMethodInjection()
        {
            UnityContainer uc = new UnityContainer();
            uc.RegisterType<IMyMethodInterface, MyMethodTest>()
                 .RegisterType<MyMethodTest, MyMethodTest1>();

            MyMethodTest2 mytestobj = uc.Resolve<MyMethodTest2>();
            mytestobj.Myobj.Strs = "I am Initialized";
            IMyMethodInterface newobj = mytestobj.MyInterface;
            MyMethodTest newobjmethod = mytestobj.Myobj;
        }

        [TestMethod]
        public void SpecialCharacterTest()
        {
            IUnityContainer uc = new UnityContainer();
            uc.RegisterType<MyMethodTest>("@Foo!@");

            MyMethodTest objspecial = uc.Resolve<MyMethodTest>("@Foo!@");
            Assert.IsNotNull(objspecial);
        }

        [TestMethod]
        public void TestNormalParameter()
        {
            UnityContainer uc = new UnityContainer();
            uc.RegisterInstance(typeof(string), "bar");

            ClassWithNormalParameters mytestobj = uc.Resolve<ClassWithNormalParameters>();
        }

        [TestMethod]
        public void TestParamsList()
        {
            object[] myarray = new object[2];

            UnityContainer uc = new UnityContainer();
            uc.RegisterInstance(typeof(string), "bar");
            uc.RegisterInstance(typeof(object[]), myarray);

            ClassWithParmsList mytestobj = uc.Resolve<ClassWithParmsList>();
        }

        [TestMethod]
        public void TestParamArray()
        {
            object[] myarray = new object[2];

            UnityContainer uc = new UnityContainer();
            uc.RegisterInstance(typeof(string), "bar");
            uc.RegisterInstance(typeof(object[]), myarray);

            ClassWithParmArray mytestobj = uc.Resolve<ClassWithParmArray>();
        }

        [TestMethod]
        public void TestRefParameter()
        {
            UnityContainer uc = new UnityContainer();
            uc.RegisterInstance(typeof(string), "bar");

            AssertHelper.ThrowsException<ResolutionFailedException>(() => uc.Resolve<ClassWithRefParameters>());
        }

        [TestMethod]
        public void TestOutParameter()
        {
            UnityContainer uc = new UnityContainer();
            uc.RegisterInstance(typeof(string), "bar");

            AssertHelper.ThrowsException<ResolutionFailedException>(() => uc.Resolve<ClassWithOutParameters>());
        }

        [TestMethod]
        public void TestGenericMethod()
        {
            UnityContainer uc = new UnityContainer();
            uc.RegisterInstance(typeof(string), "bar");

            AssertHelper.ThrowsException<ResolutionFailedException>(() => uc.Resolve<ClassWithGenericMethod>());
        }

        [TestMethod]
        public void TestGenericClass()
        {
            UnityContainer uc = new UnityContainer();
            uc.RegisterInstance(typeof(string), "bar");

            ClassWithGenericParameter<string> mytestobj = uc.Resolve<ClassWithGenericParameter<string>>();
        }
    }

            public class ClassWithParmArray
        {
            [InjectionMethod]
            public void Foo(string parameter, object[] myob)
            {
                parameter = "foo";
            }
        }

        public class ClassWithParmsList
        {
            [InjectionMethod]
            public void Foo(string parameter, params object[] arg)
            {
                parameter = "foo";
            }
        }

        public class ClassWithNormalParameters
        {
            [InjectionMethod]
            public void Foo(string parameter)
            {
                parameter = "foo";
            }
        }

        public class ClassWithRefParameters
        {
            [InjectionMethod]
            public void Foo(ref string parameter)
            {
                parameter = "foo";
            }
        }

        public class ClassWithOutParameters
        {
            [InjectionMethod]
            public void Foo(out string parameter)
            {
                parameter = "foo";
            }
        }

        public class ClassWithGenericParameter<T> where T : IEnumerable
        {
            [InjectionMethod]
            public void Foo(T parameter)
            {
                parameter = default(T);
            }
        }

        public class ClassWithGenericMethod
        {
            [InjectionMethod]
            public void Foo<TIgnored>(string parameter)
            {
                parameter = "foo";
            }
        }

    public interface IMyMethodInterface
    { }

    public class MyMethodTest : IMyMethodInterface
    {
        public string Strs = "Method Inject Test";
    }

    public class MyMethodTest1 : MyMethodTest
    {
        public string Strstest1 = "Method Inject Test 2";
    }

    public class MyMethodTest2
    {
        public MyMethodTest Myobj;
        public IMyMethodInterface MyInterface;

        [InjectionMethod]
        public void Initialize(MyMethodTest obj, IMyMethodInterface objint)
        {
            Myobj = obj;
            MyInterface = objint;
        }
    }

    public class MyMethodTestCircular
    {
        public string Name = "In MyMethodTestCircular";
        private MyMethodTestCircular objcheck;

        [InjectionMethod]
        public void Initialize(MyMethodTestCircular obj)
        {
            objcheck = obj;
        }
    }

    public class MyMethodTestvirtual
    {
    }

    public class TestVirtual
    {
        [InjectionMethod]
        public MyMethodTestvirtual Method1(UnityContainer obj)
        {
            MyMethodTestvirtual obimp = obj.Resolve<MyMethodTestvirtual>("hello");
            return obimp;
        }

        public TestVirtual()
        {
        }
    }
}