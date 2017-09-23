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
using Unity;

namespace Microsoft.Practices.Unity.Tests
{
    /// <summary>
    /// Summary description for OptionalDependencyAPIConfigurationFixture
    /// </summary>
    [TestClass]
    public class OptionalDependencyAPIConfigurationFixture
    {
        IUnityContainer container;

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

        // TODO: Verify
        //[TestMethod]
        //public void CanResolveOptionalDependencyWhenConfiguredByAPI()
        //{
        //    IGuineaPig mockPig = new Mock<IGuineaPig>().Object;

        //    container.RegisterType<GuineaPig>(
        //            new InjectionConstructor(new OptionalParameter<IGuineaPig>()))
        //        .RegisterInstance<IGuineaPig>(mockPig);

        //    var result = container.Resolve<GuineaPig>();

        //    Assert.AreSame(mockPig, result.Pig);
        //}

        //[TestMethod]
        //public void CanResolveOptionalDependenciesByNameWithAPI()
        //{
        //    IGuineaPig expected = new Mock<IGuineaPig>().Object;

        //    container.RegisterType<GuineaPig>(
        //            new InjectionConstructor(new OptionalParameter(typeof(IGuineaPig), "named")))
        //        .RegisterInstance<IGuineaPig>("named", expected);

        //    var result = container.Resolve<GuineaPig>();

        //    Assert.AreSame(expected, result.Pig);
        //}

        [TestMethod]
        public void CanConfigureOptionalPropertiesViaAPI()
        {
            container.RegisterType<GuineaPig>(
                new InjectionConstructor(),
                new InjectionProperty("Pig", new OptionalParameter<IGuineaPig>()));

            var result = container.Resolve<GuineaPig>();

            Assert.IsNull(result.Pig);
        }

        // TODO: Verify
        //[TestMethod]
        //public void CanConfigureOptionalParameterToInjectionMethod()
        //{
        //    IGuineaPig expected = new Mock<IGuineaPig>().Object;

        //    container.RegisterType<GuineaPig>(
        //            new InjectionConstructor(),
        //            new InjectionMethod("SetPig", new OptionalParameter<IGuineaPig>("named")))
        //        .RegisterInstance("named", expected);

        //    var result = container.Resolve<GuineaPig>();

        //    Assert.AreSame(expected, result.Pig);
        //}

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
