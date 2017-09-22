// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests.ContainerRegistration
{
    [TestClass]
    public class GivenContainerIntrospectionCorrectUsageFixture
    {
        private IUnityContainer container;

        [TestInitialize]
        public void Init()
        {
            container = new UnityContainer();
        }

        [TestMethod]
        public void WhenIsRegisteredIsCalledForDefaultInstance()
        {
            container.RegisterInstance<ITypeInterface>(new TypeImplementation("default"));

            var result = container.IsRegistered(typeof(ITypeInterface));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WhenIsRegisteredIsCalledForNamedInstance()
        {
            container.RegisterInstance<ITypeInterface>("foo", new TypeImplementation("foo"));

            var result = container.IsRegistered(typeof(ITypeInterface), "foo");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WhenIsRegisteredIsCalledForDefaultTypeRegisteredOnParentFromChildContainer()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var child = container.CreateChildContainer();

            var result = child.IsRegistered(typeof(ITypeInterface));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WhenIsRegisteredIsCalledForDefaultTypeFromChildContainer()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var child = container.CreateChildContainer();

            child.RegisterType<ITypeAnotherInterface, AnotherTypeImplementation>(new InjectionConstructor("default"));
            child.RegisterType<ITypeAnotherInterface, AnotherTypeImplementation>("another", new InjectionConstructor("another"));

            var result = child.IsRegistered(typeof(ITypeAnotherInterface));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WhenIsRegisteredIsCalledForDefaultTypeRegisteredOnChildContainerFromParent()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var child = container.CreateChildContainer();

            child.RegisterType<ITypeAnotherInterface, AnotherTypeImplementation>(new InjectionConstructor("default"));
            child.RegisterType<ITypeAnotherInterface, AnotherTypeImplementation>("another", new InjectionConstructor("another"));

            var result = container.IsRegistered(typeof(ITypeAnotherInterface));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void WhenIsRegisteredIsCalledForDefaultType()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var result = container.IsRegistered(typeof(ITypeInterface));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WhenIsRegisteredIsCalledForSpecificName()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var result = container.IsRegistered(typeof(ITypeInterface), "foo");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WhenIsRegisteredGenericIsCalledForDefaultType()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var result = container.IsRegistered<ITypeInterface>();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WhenIsRegisteredGenericIsCalledForSpecificName()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var result = container.IsRegistered<ITypeInterface>("foo");

            Assert.IsTrue(result);
        }
    }
}
