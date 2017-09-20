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

namespace Unity.Tests
{
    [TestClass]
    public class ContainerBasicFixture
    {
        [TestMethod]
        public void ResolveAllOnlyReturnsInterfaceRegistrations()
        {
            ITest iTest;

            ATest objA = new ATest();
            BTest objB = new BTest();
            CTest objC = new CTest();

            objA.Strtest = "Hi";

            UnityContainer uc1 = new UnityContainer();

            uc1.RegisterType<ITest, ATest>();

            iTest = objA;
            uc1.RegisterInstance("ATest", iTest);
            iTest = objB;
            uc1.RegisterInstance("BTest", iTest);
            iTest = objC;
            uc1.RegisterInstance("CTest", iTest);

            List<ATest> list = new List<ATest>(uc1.ResolveAll<ATest>());
            List<ITest> list3 = new List<ITest>(uc1.ResolveAll<ITest>());

            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(3, list3.Count);
        }

        [TestMethod]
        public void NamedMappedInterfaceInstanceRegistrationCanBeResolved()
        {
            ITest iTest;
            ATest objA = null;

            UnityContainer uc1 = new UnityContainer();

            iTest = objA;
            uc1.RegisterInstance<ITest>("ATest", new ATest());
            iTest = (ITest)uc1.Resolve(typeof(ITest), "ATest");

            Assert.IsNotNull(iTest);
        }

        [TestMethod]
        public void UnnamedMappingThrowsResolutionFailedException()
        {
            //The inner exception says {"The type ITest does not have an accessible constructor."} where as it should
            //specify that the named registration for ITest does not exist

            //There is an unnamed mapping 

            ATest objA = null;

            UnityContainer uc1 = new UnityContainer();

            uc1.RegisterType<ITest, ATest>();
            AssertHelper.ThrowsException<ResolutionFailedException>(() => objA = (ATest)uc1.Resolve<ITest>("ATest"));
        }

        [TestMethod]
        public void WhenInstanceIsRegisteredAsSingletonEnsureItIsNotGarbageCollected()
        {
            ITest iTest;
            BTest objB = new BTest();

            UnityContainer uc1 = new UnityContainer();

            uc1.RegisterType<ITest, ATest>("ATest");
            iTest = objB;

            uc1.RegisterInstance<ITest>("BTest", iTest);

            iTest = (ITest)uc1.Resolve(typeof(ITest), "BTest");
            Assert.IsNotNull(iTest);

            iTest = null;

            GC.Collect();

            iTest = (ITest)uc1.Resolve(typeof(ITest), "BTest");
            
            Assert.IsNotNull(iTest);

            iTest = (ITest)uc1.Resolve(typeof(ITest), "ATest");

            Assert.IsNotNull(iTest);
        }

        [TestMethod]
        public void ResovleCollection()
        {
            UnityContainer uc1 = new UnityContainer();

            uc1.RegisterType<ITestColl, BTestColl>("BTest");
            BTestColl b = (BTestColl)uc1.Resolve<BTestColl>();
        }

        [TestMethod]
        public void CollectionWhenConstructorParameterNotRegisteredThrowsResolutionFailedException()
        {
            UnityContainer uc1 = new UnityContainer();

            uc1.RegisterType<ITestColl, CTestColl>("CTest");
            AssertHelper.ThrowsException<ResolutionFailedException>(() => uc1.Resolve<CTestColl>());
        }

        [TestMethod]
        public void WhenResolvePrimitiveThrowsResolutionFailedException()
        {
            //Primitive type not supported
            UnityContainer uc1 = new UnityContainer();

            uc1.RegisterType<int>("i");
            AssertHelper.ThrowsException<ResolutionFailedException>(() => uc1.Resolve<int>("i"));
        }

        [TestMethod]
        public void ResolveListAtest()
        {
            //List of class  type not supported
            UnityContainer uc1 = new UnityContainer();

            uc1.RegisterType<List<ATest>>("List");
            AssertHelper.ThrowsException<ResolutionFailedException>(() => uc1.Resolve<List<ATest>>("List"));
        }

        [TestMethod]
        public void ResolveArrayOfAtest()
        {
            UnityContainer uc1 = new UnityContainer();

            uc1.RegisterType<ATest[]>("Array");
            ATest[] arr = (ATest[])uc1.Resolve<ATest[]>("Array");
        }

        #region Basic Parameterized Constructor

        [TestMethod]
        public void ResolveListAtestAsParameterToConstructor()
        {
            //Constructor of List of class as parameter not supported
            UnityContainer uc1 = new UnityContainer();

            uc1.RegisterType<ListOfClassParameter>("List");
            AssertHelper.ThrowsException<ResolutionFailedException>(() => uc1.Resolve<ListOfClassParameter>("List"));
        }

        [TestMethod]
        public void ResolveArrayOfAtestAsParameterToConstructor()
        {
            UnityContainer uc1 = new UnityContainer();

            uc1.RegisterType<ArrParameter>("Array");
            ArrParameter arr = (ArrParameter)uc1.Resolve<ArrParameter>("Array");
        }

        [TestMethod]
        public void ResolveDotNetClassAsParameterToConstructor()
        {
            //Constructor Int32 as parameter not supported
            UnityContainer uc1 = new UnityContainer();
            Int32 i32 = new Int32();
            uc1.RegisterInstance<Int32>(i32);

            uc1.RegisterType<IntParameter>("Int32");
            IntParameter int32 = uc1.Resolve<IntParameter>("Int32");
        }

        #endregion Basic Parameterized Constructor
    }

    public interface ITest
    { }

    public class ATest : ITest
    {
        public string Strtest = "Hello";
    }

    public class BTest : ATest
    { }

    public class CTest : BTest
    { }

    public interface ITestColl
    { }

    public class ATestColl : ITestColl
    {
        public string Strtest = "Hello";

        public ATestColl()
        {
        }
    }

    public class PremitiveParameter : ITestColl
    {
        public PremitiveParameter(int i)
        {
        }
    }

    public class ListOfClassParameter : ITestColl
    {
        public ListOfClassParameter(List<ATest> lst)
        {
        }
    }

    public class IntParameter : ITestColl
    {
        public IntParameter(Int32 i32)
        {
        }
    }

    public class ArrParameter : ITestColl
    {
        public ArrParameter(ATest[] i)
        {
        }
    }

    public class BTestColl : ATestColl
    {
        public BTestColl(ATestColl[] acoll)
        {
        }
    }

    public class CTestColl : ITestColl
    {
        public CTestColl(char acoll)
        {
        }
    }
}