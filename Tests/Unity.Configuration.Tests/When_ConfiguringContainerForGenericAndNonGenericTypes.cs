// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.Unity.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.Configuration.Tests.TestObjects.MyGenericTypes;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerForGenericAndNonGenericTypes
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerForGenericAndNonGenericTypes : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerForGenericAndNonGenericTypes()
            : base("Generics", "container1")
        {
        }

        [TestMethod]
        public void Then_CanResolveConfiguredGenericType()
        {
            var result = Container.Resolve<ItemsCollection<IItem>>();

            Assert.AreEqual(8, result.Items.Length);
            Assert.IsInstanceOfType(result.Printer, typeof(MyPrintService<IItem>));
        }

        [TestMethod]
        public void Then_CanResolveConfiguredGenericTypeWithSpecificElements()
        {
            var result = Container.Resolve<ItemsCollection<IItem>>("OnlyThree");
            Assert.AreEqual(3, result.Items.Length);
        }

        [TestMethod]
        public void Then_CanConfigureGenericArrayInjectionViaAPI()
        {
            Container.RegisterType(typeof(ItemsCollection<>), "More",
                new InjectionConstructor("MyGenericCollection", new ResolvedParameter(typeof(IGenericService<>))),
                new InjectionProperty("Items", 
                    new GenericResolvedArrayParameter("T",
                        new GenericParameter("T", "Xray"), 
                        new GenericParameter("T", "Common"),
                        new GenericParameter("T", "Tractor"))));

            var result = Container.Resolve<ItemsCollection<IItem>>("More");
            Assert.AreEqual(3, result.Items.Length);
        }

        [TestMethod]
        public void Then_CanResolveConfiguredResolvableOptionalGenericType()
        {
            var result = Container.Resolve<ItemsCollection<IItem>>("optional resolvable");

            Assert.AreEqual(1, result.Items.Length);
            Assert.IsNotNull(result.Items[0]);
            Assert.AreEqual("Charlie Miniature", result.Items[0].ItemName);
        }

        [TestMethod]
        public void Then_CanResolveConfiguredNonResolvableOptionalGenericType()
        {
            var result = Container.Resolve<ItemsCollection<IItem>>("optional non resolvable");

            Assert.AreEqual(1, result.Items.Length);
            Assert.IsNull(result.Items[0]);
        }

        [TestMethod]
        public void Then_CanResolveConfiguredGenericTypeWithArrayInjectedInConstructor()
        {
            var result = Container.Resolve<ItemsCollection<IItem>>("ThroughConstructor");

            Assert.AreEqual(8, result.Items.Length);
            Assert.IsInstanceOfType(result.Printer, typeof(MyPrintService<IItem>));
        }

        [TestMethod]
        public void Then_CanResolveConfiguredGenericTypeWithArrayInjectedInConstructorWithSpecificElements()
        {
            var result = Container.Resolve<ItemsCollection<IItem>>("ThroughConstructorWithSpecificElements");

            Assert.AreEqual(3, result.Items.Length);
        }

        // [TestMethod]
        // nested arrays with generics not supported by container
        public void Then_CanResolveConfiguredGenericTypeWithArrayOfArraysInjectedInConstructorWithSpecificElements()
        {
            var result = Container.Resolve<ItemsCollection<IItem>>("ArrayOfArraysThroughConstructorWithSpecificElements");

            Assert.AreEqual(3, result.Items.Length);
        }
    }

    [TestClass]
    public class When_ConfiguringContainerWithDependencyElementForGenericPropertyArrayWithTypeSet : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithDependencyElementForGenericPropertyArrayWithTypeSet()
            : base("Generics", "dependency with type")
        { }

        InvalidOperationException exception;

        protected override void Act()
        {
            try
            {
                base.Act();
            }
            catch (InvalidOperationException ex)
            {
                this.exception = ex;
            }
        }

        [TestMethod]
        public void ThenContainerSetupThrows()
        {
            Assert.IsNotNull(this.exception);
        }
    }

    [TestClass]
    public class When_ConfiguringContainerWithParameterWithValueElement : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithParameterWithValueElement()
            : base("Generics", "property with value")
        { }

        InvalidOperationException exception;

        protected override void Act()
        {
            try
            {
                base.Act();
            }
            catch (InvalidOperationException ex)
            {
                this.exception = ex;
            }
        }

        [TestMethod]
        public void ThenContainerSetupThrows()
        {
            Assert.IsNotNull(this.exception);
        }
    }

    [TestClass]
    public class When_ConfiguringContainerWithGenericArrayPropertyWithValueElement : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithGenericArrayPropertyWithValueElement()
            : base("Generics", "generic array property with value")
        { }

        InvalidOperationException exception;

        protected override void Act()
        {
            try
            {
                base.Act();
            }
            catch (InvalidOperationException ex)
            {
                this.exception = ex;
            }
        }

        [TestMethod]
        public void ThenContainerSetupThrows()
        {
            Assert.IsNotNull(this.exception);
        }
    }

    [TestClass]
    public class When_ConfiguringContainerWithChainedGenericParameterWithValueElement : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithChainedGenericParameterWithValueElement()
            : base("Generics", "chained generic parameter with value")
        { }

        InvalidOperationException exception;

        protected override void Act()
        {
            try
            {
                base.Act();
            }
            catch (InvalidOperationException ex)
            {
                this.exception = ex;
            }
        }

        [TestMethod]
        public void ThenContainerSetupThrows()
        {
            Assert.IsNotNull(this.exception);
        }
    }

    [TestClass]
    public class When_ConfiguringContainerWithDependencyElementForArrayWithTypeSet : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithDependencyElementForArrayWithTypeSet()
            : base("Generics", "generic array property with dependency with type")
        { }

        InvalidOperationException exception;

        protected override void Act()
        {
            try
            {
                base.Act();
            }
            catch (InvalidOperationException ex)
            {
                this.exception = ex;
            }
        }

        [TestMethod]
        public void ThenContainerSetupThrows()
        {
            Assert.IsNotNull(this.exception);
        }
    }

    [TestClass]
    public class When_ConfiguringContainerWithArrayElementForChainedGenericParameter : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithArrayElementForChainedGenericParameter()
            : base("Generics", "chained generic parameter with array")
        { }

        InvalidOperationException exception;

        protected override void Act()
        {
            try
            {
                base.Act();
            }
            catch (InvalidOperationException ex)
            {
                this.exception = ex;
            }
        }

        [TestMethod]
        public void ThenContainerSetupThrows()
        {
            Assert.IsNotNull(this.exception);
        }
    }
}
