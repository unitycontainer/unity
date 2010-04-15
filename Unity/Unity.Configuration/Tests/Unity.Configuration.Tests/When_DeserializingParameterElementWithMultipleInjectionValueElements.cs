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

using System.Configuration;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    [TestClass]
    public class When_DeserializingParameterElement
    {
        [TestMethod]
        public void Then_CanDeserializeSingleInjectionValueChild()
        {
            var elementXml = @"
                <param name=""connectionString"">
                    <value value=""northwind"" />
                </param>";

            var reader = new XmlTextReader(new StringReader(elementXml));
            var result = reader.MoveToContent();
            var element = new ParameterElement();

            element.Deserialize(reader);

            Assert.AreSame(typeof(ValueElement), element.Value.GetType());
            Assert.AreEqual("northwind", ((ValueElement)element.Value).Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void Then_DeserializingMultipleInjectionValueChildrenThrows()
        {
            var elementXml = @"
                <param name=""connectionString"">
                    <value value=""northwind"" />
                    <value value=""northwind"" />
                </param>";

            var reader = new XmlTextReader(new StringReader(elementXml));
            var result = reader.MoveToContent();
            var element = new ParameterElement();

            element.Deserialize(reader);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void Then_DeserializingWithParametersAndValueChildrenThrows()
        {
            var elementXml = @"
                <param name=""connectionString"" value=""northwind2"">
                    <value value=""northwind"" />
                </param>";

            var reader = new XmlTextReader(new StringReader(elementXml));
            var result = reader.MoveToContent();
            var element = new ParameterElement();

            element.Deserialize(reader);
        }
    }

    [TestClass]
    public class When_DeserializingPropertyElement
    {
        [TestMethod]
        public void Then_CanDeserializeSingleInjectionValueChild()
        {
            var elementXml = @"
                <property name=""connectionString"">
                    <value value=""northwind"" />
                </property>";

            var reader = new XmlTextReader(new StringReader(elementXml));
            var result = reader.MoveToContent();
            var element = new PropertyElement();

            element.Deserialize(reader);

            Assert.AreSame(typeof(ValueElement), element.Value.GetType());
            Assert.AreEqual("northwind", ((ValueElement)element.Value).Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void Then_DeserializingMultipleInjectionValueChildrenThrows()
        {
            var elementXml = @"
                <property name=""connectionString"">
                    <value value=""northwind"" />
                    <value value=""northwind"" />
                </property>";

            var reader = new XmlTextReader(new StringReader(elementXml));
            var result = reader.MoveToContent();
            var element = new PropertyElement();

            element.Deserialize(reader);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void Then_DeserializingWithParametersAndValueChildrenThrows()
        {
            var elementXml = @"
                <property name=""connectionString"" value=""northwind2"">
                    <value value=""northwind"" />
                </property>";

            var reader = new XmlTextReader(new StringReader(elementXml));
            var result = reader.MoveToContent();
            var element = new PropertyElement();

            element.Deserialize(reader);
        }
    }
}
