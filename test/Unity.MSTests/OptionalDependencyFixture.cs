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
    /// Summary description for OptionalDependencyFixture
    /// </summary>
    [TestClass]
    public class OptionalDependencyFixture
    {
        [TestMethod]
        public void OptionalParametersSetToNullIfNotRegistered()
        {
            IUnityContainer container = new UnityContainer();

            OptionalConstParameterClass result = container.Resolve<OptionalConstParameterClass>();

            Assert.IsNull(result.TestObject);
        }

        [TestMethod]
        public void OptionalParametersResolvedIfInstanceRegistered()
        {
            IUnityContainer container = new UnityContainer();
            var input = new TestObject();

            container.RegisterInstance<ITestObject>(input);

            OptionalConstParameterClass result = container.Resolve<OptionalConstParameterClass>();

            Assert.AreSame(input, result.TestObject);
        }

        [TestMethod]
        public void OptionalParametersResolvedIfInstanceRegisteredWithName()
        {
            IUnityContainer container = new UnityContainer();
            var input = new TestObject();

            container.RegisterInstance<ITestObject>("test", input);

            NamedOptionalConstParameterClass result = container.Resolve<NamedOptionalConstParameterClass>();

            Assert.AreSame(input, result.TestObject);
        }

        [TestMethod]
        public void OptionalParametersResolvedIfInstanceRegisteredInParent()
        {
            IUnityContainer parent = new UnityContainer();
            IUnityContainer child = parent.CreateChildContainer();

            var input = new TestObject();
            parent.RegisterInstance<ITestObject>(input);

            OptionalConstParameterClass result = child.Resolve<OptionalConstParameterClass>();

            Assert.AreSame(input, result.TestObject);
        }

        [TestMethod]
        public void OptionalParametersResolvedIfInstanceRegisteredInParentWithName()
        {
            IUnityContainer parent = new UnityContainer();
            IUnityContainer child = parent.CreateChildContainer();

            var input = new TestObject();
            parent.RegisterInstance<ITestObject>("test", input);

            NamedOptionalConstParameterClass result = child.Resolve<NamedOptionalConstParameterClass>();

            Assert.AreSame(input, result.TestObject);
        }

        [TestMethod]
        public void OptionalParametersNotResolvedIfMoreSpecificTypeRegistered()
        {
            IUnityContainer container = new UnityContainer();
            var input = new TestObject();

            container.RegisterInstance<TestObject>(input);

            OptionalConstParameterClass result = container.Resolve<OptionalConstParameterClass>();

            Assert.IsNull(result.TestObject);
        }

        [TestMethod]
        public void OptionalParametersNotResolvedIfMoreSpecificTypeRegisteredWithName()
        {
            IUnityContainer container = new UnityContainer();
            var input = new TestObject();

            container.RegisterInstance<TestObject>("test", input);

            NamedOptionalConstParameterClass result = container.Resolve<NamedOptionalConstParameterClass>();

            Assert.IsNull(result.TestObject);
        }

        [TestMethod]
        public void OptionalParametersResolvedIfTypeRegistered()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType<TestObject>();

            OptionalConstParameterClass1 result = container.Resolve<OptionalConstParameterClass1>();

            Assert.IsNotNull(result.TestObject);
        }

        [TestMethod]
        public void OptionalParametersResolvedIfTypeRegisteredInParent()
        {
            IUnityContainer parent = new UnityContainer();
            IUnityContainer child = parent.CreateChildContainer();

            parent.RegisterType<TestObject>();

            OptionalConstParameterClass1 result = child.Resolve<OptionalConstParameterClass1>();

            Assert.IsNotNull(result.TestObject);
        }

        [TestMethod]
        public void OptionalParametersNullIfTypeRegisteredThrowsAtResolve()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType<RandomTestObject>();

            OptionalConstParameterThrowsAtResolve result = container.Resolve<OptionalConstParameterThrowsAtResolve>();

            Assert.IsNull(result.TestObject);
        }

        [TestMethod]
        public void CanConfigureInjectionConstWithOptionalParameters()
        {
            IUnityContainer container = new UnityContainer();
            var input = new TestObject();

            container.RegisterInstance<ITestObject>(input);

            container.RegisterType<OptionalDependencyTestClass>(new InjectionConstructor(new OptionalParameter<ITestObject>()));

            var result = container.Resolve<OptionalDependencyTestClass>();

            Assert.IsNotNull(result.InternalTestObject);
        }

        [TestMethod]
        public void CanConfigureInjectionPropertyWithOptionalParameters()
        {
            IUnityContainer container = new UnityContainer();
            var input = new TestObject();

            container.RegisterInstance<ITestObject>(input);

            container.RegisterType<OptionalDependencyTestClass>(new InjectionProperty("InternalTestObject", new OptionalParameter<ITestObject>()));

            var result = container.Resolve<OptionalDependencyTestClass>();

            Assert.IsNotNull(result.InternalTestObject);
        }
    }

    public class OptionalConstParameterClass
    {
        public ITestObject TestObject;
        public OptionalConstParameterClass([OptionalDependency()] ITestObject test)
        {
            TestObject = test;
        }
    }

    public class OptionalConstParameterClass1
    {
        public TestObject TestObject;
        public OptionalConstParameterClass1([OptionalDependency()] TestObject test)
        {
            TestObject = test;
        }
    }

    public class NamedOptionalConstParameterClass
    {
        public ITestObject TestObject;
        public NamedOptionalConstParameterClass([OptionalDependency("test")] ITestObject test)
        {
            TestObject = test;
        }
    }

    public class OptionalConstParameterThrowsAtResolve
    {
        public RandomTestObject TestObject;
        public OptionalConstParameterThrowsAtResolve([OptionalDependency()] RandomTestObject test)
        {
            TestObject = test;
        }
    }

    public class OptionalDependencyTestClass
    {
        private ITestObject internalTestObject;

        public OptionalDependencyTestClass()
        {
        }

        public ITestObject InternalTestObject
        {
            get { return internalTestObject; }
            set { internalTestObject = value; }
        }

        public OptionalDependencyTestClass(ITestObject obj)
        {
            internalTestObject = obj;
        }
    }

    public interface ITestObject { }

    public class TestObject : ITestObject
    {
    }

    public class RandomTestObject
    {
        public RandomTestObject()
        {
            throw (new Exception("Test Exception"));
        }
    }
}