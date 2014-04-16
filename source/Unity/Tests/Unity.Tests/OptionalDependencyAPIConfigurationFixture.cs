// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.Tests
{
    /// <summary>
    /// Summary description for OptionalDependencyAPIConfigurationFixture
    /// </summary>
    [TestClass]
    public class OptionalDependencyAPIConfigurationFixture
    {
        private IUnityContainer container;

        [TestInitialize]
        public void Setup()
        {
            container = new UnityContainer();
        }

        [TestMethod]
        public void CanConfigureConstructorWithOptionalDependency()
        {
            container.RegisterType<GuineaPig>(
                    new InjectionConstructor(new OptionalParameter<IGuineaPig>()));

            var result = container.Resolve<GuineaPig>();

            Assert.IsNull(result.Pig);
        }

        [TestMethod]
        public void CanResolveOptionalDependencyWhenConfiguredByAPI()
        {
            IGuineaPig mockPig = new GuineaPigImpl();

            container.RegisterType<GuineaPig>(
                    new InjectionConstructor(new OptionalParameter<IGuineaPig>()))
                .RegisterInstance<IGuineaPig>(mockPig);

            var result = container.Resolve<GuineaPig>();

            Assert.AreSame(mockPig, result.Pig);
        }

        [TestMethod]
        public void CanResolveOptionalDependenciesByNameWithAPI()
        {
            IGuineaPig expected = new GuineaPigImpl();

            container.RegisterType<GuineaPig>(
                    new InjectionConstructor(new OptionalParameter(typeof(IGuineaPig), "named")))
                .RegisterInstance<IGuineaPig>("named", expected);

            var result = container.Resolve<GuineaPig>();

            Assert.AreSame(expected, result.Pig);
        }

        [TestMethod]
        public void CanConfigureOptionalPropertiesViaAPI()
        {
            container.RegisterType<GuineaPig>(
                new InjectionConstructor(),
                new InjectionProperty("Pig", new OptionalParameter<IGuineaPig>()));

            var result = container.Resolve<GuineaPig>();

            Assert.IsNull(result.Pig);
        }

        [TestMethod]
        public void CanConfigureOptionalParameterToInjectionMethod()
        {
            IGuineaPig expected = new GuineaPigImpl();

            container.RegisterType<GuineaPig>(
                    new InjectionConstructor(),
                    new InjectionMethod("SetPig", new OptionalParameter<IGuineaPig>("named")))
                .RegisterInstance("named", expected);

            var result = container.Resolve<GuineaPig>();

            Assert.AreSame(expected, result.Pig);
        }

        public class GuineaPig
        {
            public bool ConstructorWithArgsWasCalled = false;

            public GuineaPig()
            {
            }

            public GuineaPig(IGuineaPig pig)
            {
                Pig = pig;
                ConstructorWithArgsWasCalled = true;
            }

            public IGuineaPig Pig
            {
                get;
                set;
            }

            public void SetPig(IGuineaPig pig)
            {
                Pig = pig;
            }
        }

        public interface IGuineaPig
        {
        }

        public class GuineaPigImpl : IGuineaPig
        {
        }
    }
}
