// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#else
using Xunit;
#endif

namespace Unity.Tests
{
    /// <summary>
    /// Summary description for OptionalDependencyAttributeFixture
    /// </summary>
     
    public class OptionalDependencyAttributeFixture
    {
        public void OptionalDependencyParametersAreInjectedWithNull()
        {
            IUnityContainer container = new UnityContainer();

            var result = container.Resolve<ObjectWithOptionalConstructorParameter>();
            Assert.Null(result.SomeInterface);
        }

        [Fact]
        public void OptionalDependencyParameterIsResolvedIfRegisteredInContainer()
        {
            ISomeInterface expectedSomeInterface = new SomeInterfaceMock();
            IUnityContainer container = new UnityContainer()
                .RegisterInstance<ISomeInterface>(expectedSomeInterface);

            var result = container.Resolve<ObjectWithOptionalConstructorParameter>();

            Assert.Same(expectedSomeInterface, result.SomeInterface);
        }

        [Fact]
        public void OptionalDependencyParameterIsResolvedByName()
        {
            ISomeInterface namedSomeInterface = new SomeInterfaceMock();
            ISomeInterface defaultSomeInterface = new SomeInterfaceMock();

            IUnityContainer container = new UnityContainer()
                .RegisterInstance<ISomeInterface>(defaultSomeInterface)
                .RegisterInstance<ISomeInterface>("named", namedSomeInterface);

            var result = container.Resolve<ObjectWithNamedOptionalConstructorParameter>();

            Assert.Same(namedSomeInterface, result.SomeInterface);
        }

        [Fact]
        public void OptionalPropertiesGetNullWhenNotConfigured()
        {
            IUnityContainer container = new UnityContainer();

            var result = container.Resolve<ObjectWithOptionalProperty>();

            Assert.Null(result.SomeInterface);
        }

        [Fact]
        public void OptionalPropertiesAreInjectedWhenRegisteredInContainer()
        {
            ISomeInterface expected = new SomeInterfaceMock();
            IUnityContainer container = new UnityContainer()
                .RegisterInstance(expected);

            var result = container.Resolve<ObjectWithOptionalProperty>();

            Assert.Same(expected, result.SomeInterface);
        }

        [Fact]
        public void OptionalPropertiesAreInjectedByName()
        {
            ISomeInterface namedSomeInterface = new SomeInterfaceMock();
            ISomeInterface defaultSomeInterface = new SomeInterfaceMock();

            IUnityContainer container = new UnityContainer()
                .RegisterInstance<ISomeInterface>(defaultSomeInterface)
                .RegisterInstance<ISomeInterface>("named", namedSomeInterface);

            var result = container.Resolve<ObjectWithNamedOptionalProperty>();

            Assert.Same(namedSomeInterface, result.SomeInterface);
        }

        public class SomeInterfaceMock : ISomeInterface
        { }

        public interface ISomeInterface
        {
        }

        public class ObjectWithOptionalConstructorParameter
        {
            private ISomeInterface someInterface;

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
