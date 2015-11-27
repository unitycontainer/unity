// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.Configuration.Tests.ConfigFiles;
using Unity.Configuration.Tests.TestObjects;
using Unity.TestSupport;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerWithConstructorsWithValues
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerWithConstructorsWithValues : ContainerConfiguringFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerWithConstructorsWithValues()
            : base("VariousConstructors", "constructorWithValue")
        {
        }

        [TestMethod]
        public void Then_ConstructorGetsProperLiteralValuePassedFromChildElement()
        {
            var result = Container.Resolve<MockDatabase>("withExplicitValueElement");

            Assert.AreEqual("northwind", result.ConnectionString);
        }

        [TestMethod]
        public void Then_ConstructorGetsProperResolvedDependency()
        {
            var result = Container.Resolve<MockDatabase>("resolvedWithName");

            Assert.AreEqual("adventureWorks", result.ConnectionString);
        }

        [TestMethod]
        public void Then_ConstructorGetsProperResolvedDependencyViaAttribute()
        {
            var result = Container.Resolve<MockDatabase>("resolvedWithNameViaAttribute");

            Assert.AreEqual("contosoDB", result.ConnectionString);
        }

        [TestMethod]
        public void Then_ValuesAreProperlyConvertedWhenTypeIsNotString()
        {
            var result = Container.Resolve<ObjectTakingScalars>("injectInt");

            Assert.AreEqual(17, result.IntValue);
        }

        [TestMethod]
        public void Then_ConstructorGetsPropertyLiteralValueFromValueAttribute()
        {
            var result = Container.Resolve<ObjectTakingScalars>("injectIntWithValueAttribute");

            Assert.AreEqual(35, result.IntValue);
        }

        [TestMethod]
        public void Then_TypeConverterIsUsedToGenerateConstructorValue()
        {
            var result = Container.Resolve<ObjectTakingScalars>("injectIntWithTypeConverter");

            Assert.AreEqual(-35, result.IntValue);
        }

        [TestMethod]
        public void Then_TypeConverterIsUsedToGenerateConstructorValueViaAttribute()
        {
            var result = Container.Resolve<ObjectTakingScalars>("injectIntWithTypeConverterAttribute");

            Assert.AreEqual(-35, result.IntValue);
        }
    }
}
