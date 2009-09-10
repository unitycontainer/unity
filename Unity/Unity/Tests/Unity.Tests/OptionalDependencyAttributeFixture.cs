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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
            Assert.IsNull(result.Foo);
        }

        [TestMethod]
        public void OptionalDependencyParameterIsResolvedIfRegisteredInContainer()
        {
            IFoo expectedFoo = new Mock<IFoo>().Object;
            IUnityContainer container = new UnityContainer()
                .RegisterInstance<IFoo>(expectedFoo);

            var result = container.Resolve<ObjectWithOptionalConstructorParameter>();

            Assert.AreSame(expectedFoo, result.Foo);
        }

        [TestMethod]
        public void OptionalDependencyParameterIsResolvedByName()
        {
            IFoo namedFoo = new Mock<IFoo>().Object;
            IFoo defaultFoo = new Mock<IFoo>().Object;

            IUnityContainer container = new UnityContainer()
                .RegisterInstance<IFoo>(defaultFoo)
                .RegisterInstance<IFoo>("named", namedFoo);

            var result = container.Resolve<ObjectWithNamedOptionalConstructorParameter>();

            Assert.AreSame(namedFoo, result.Foo);
        }

        [TestMethod]
        public void OptionalPropertiesGetNullWhenNotConfigured()
        {
            IUnityContainer container = new UnityContainer();

            var result = container.Resolve<ObjectWithOptionalProperty>();

            Assert.IsNull(result.Foo);
        }

        [TestMethod]
        public void OptionalPropertiesAreInjectedWhenRegisteredInContainer()
        {
            IFoo expected = new Mock<IFoo>().Object;
            IUnityContainer container = new UnityContainer()
                .RegisterInstance(expected);

            var result = container.Resolve<ObjectWithOptionalProperty>();

            Assert.AreSame(expected, result.Foo);
        }

        [TestMethod]
        public void OptionalPropertiesAreInjectedByName()
        {
            IFoo namedFoo = new Mock<IFoo>().Object;
            IFoo defaultFoo = new Mock<IFoo>().Object;

            IUnityContainer container = new UnityContainer()
                .RegisterInstance<IFoo>(defaultFoo)
                .RegisterInstance<IFoo>("named", namedFoo);

            var result = container.Resolve<ObjectWithNamedOptionalProperty>();

            Assert.AreSame(namedFoo, result.Foo);
        }

        public interface IFoo
        {
        }

        public class ObjectWithOptionalConstructorParameter
        {
            IFoo foo;

            public IFoo Foo { get { return foo; } }

            public ObjectWithOptionalConstructorParameter([OptionalDependency] IFoo foo)
            {
                this.foo = foo;
            }
        }

        public class ObjectWithNamedOptionalConstructorParameter
            : ObjectWithOptionalConstructorParameter
        {
            public ObjectWithNamedOptionalConstructorParameter([OptionalDependency("named")] IFoo foo)
                : base(foo)
            {

            }
        }

        public class ObjectWithOptionalProperty
        {
            [OptionalDependency]
            public IFoo Foo
            {
                get;
                set;
            }
        }

        public class ObjectWithNamedOptionalProperty
        {
            [OptionalDependency("named")]
            public IFoo Foo
            {
                get;
                set;
            }
        }
    }
}
