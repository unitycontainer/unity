//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.Tests
{
    /// <summary>
    /// Summary description for OptionalDependencyAttributeFixture
    /// </summary>
    [TestClass]
    public class OptionalDependencyAttributeFixture
    {
        public void OptionalDependencyParametersAreInjectedWithNull()
        {
            IUnityContainer container = new UnityContainer();

            var result = container.Resolve<ObjectWithOptionalConstructorParameter>();
            Assert.IsNull(result.SomeInterface);
        }

        [TestMethod]
        public void OptionalDependencyParameterIsResolvedIfRegisteredInContainer()
        {
            ISomeInterface expectedSomeInterface = new SomeInterfaceMock();
            IUnityContainer container = new UnityContainer()
                .RegisterInstance<ISomeInterface>(expectedSomeInterface);

            var result = container.Resolve<ObjectWithOptionalConstructorParameter>();

            Assert.AreSame(expectedSomeInterface, result.SomeInterface);
        }

        [TestMethod]
        public void OptionalDependencyParameterIsResolvedByName()
        {
            ISomeInterface namedSomeInterface = new SomeInterfaceMock();
            ISomeInterface defaultSomeInterface = new SomeInterfaceMock();

            IUnityContainer container = new UnityContainer()
                .RegisterInstance<ISomeInterface>(defaultSomeInterface)
                .RegisterInstance<ISomeInterface>("named", namedSomeInterface);

            var result = container.Resolve<ObjectWithNamedOptionalConstructorParameter>();

            Assert.AreSame(namedSomeInterface, result.SomeInterface);
        }

        [TestMethod]
        public void OptionalPropertiesGetNullWhenNotConfigured()
        {
            IUnityContainer container = new UnityContainer();

            var result = container.Resolve<ObjectWithOptionalProperty>();

            Assert.IsNull(result.SomeInterface);
        }

        [TestMethod]
        public void OptionalPropertiesAreInjectedWhenRegisteredInContainer()
        {
            ISomeInterface expected = new SomeInterfaceMock();
            IUnityContainer container = new UnityContainer()
                .RegisterInstance(expected);

            var result = container.Resolve<ObjectWithOptionalProperty>();

            Assert.AreSame(expected, result.SomeInterface);
        }

        [TestMethod]
        public void OptionalPropertiesAreInjectedByName()
        {
            ISomeInterface namedSomeInterface = new SomeInterfaceMock();
            ISomeInterface defaultSomeInterface = new SomeInterfaceMock();

            IUnityContainer container = new UnityContainer()
                .RegisterInstance<ISomeInterface>(defaultSomeInterface)
                .RegisterInstance<ISomeInterface>("named", namedSomeInterface);

            var result = container.Resolve<ObjectWithNamedOptionalProperty>();

            Assert.AreSame(namedSomeInterface, result.SomeInterface);
        }

        public class SomeInterfaceMock : ISomeInterface
        { }

        public interface ISomeInterface
        {
        }

        public class ObjectWithOptionalConstructorParameter
        {
            ISomeInterface someInterface;

            public ISomeInterface SomeInterface { get { return someInterface; } }

            public ObjectWithOptionalConstructorParameter([OptionalDependency] ISomeInterface someInterface)
            {
                this.someInterface = someInterface;
            }
        }

        public class ObjectWithNamedOptionalConstructorParameter
            : ObjectWithOptionalConstructorParameter
        {
            public ObjectWithNamedOptionalConstructorParameter([OptionalDependency("named")] ISomeInterface someInterface)
                : base(someInterface)
            {

            }
        }

        public class ObjectWithOptionalProperty
        {
            [OptionalDependency]
            public ISomeInterface SomeInterface
            {
                get;
                set;
            }
        }

        public class ObjectWithNamedOptionalProperty
        {
            [OptionalDependency("named")]
            public ISomeInterface SomeInterface
            {
                get;
                set;
            }
        }
    }
}
