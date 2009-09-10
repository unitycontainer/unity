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

using System;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Practices.Unity.Tests
{
    /// <summary>
    /// Summary description for OptionalDependencyResolverPolicyFixture
    /// </summary>
    [TestClass]
    public class OptionalDependencyResolverPolicyFixture
    {
        [TestMethod]
        public void CanCreateResolverWithNoName()
        {
            var resolver = new OptionalDependencyResolverPolicy(typeof(object));
            Assert.AreEqual(typeof(object), resolver.DependencyType);
            Assert.IsNull(resolver.Name);
        }

        [TestMethod]
        public void CanCreateResolverWithName()
        {
            var resolver = new OptionalDependencyResolverPolicy(typeof(object), "name");
            Assert.AreEqual(typeof(object), resolver.DependencyType);
            Assert.AreEqual("name", resolver.Name);
        }

        [TestMethod]
        public void ResolverReturnsNullWhenDependencyIsNotResolved()
        {
            IBuilderContext context = GetMockContextThatThrows();
            var resolver = new OptionalDependencyResolverPolicy(typeof(object));

            object result = resolver.Resolve(context);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ResolverReturnsBuiltObject()
        {
            string expected = "Here's the string to resolve";
            IBuilderContext context = GetMockContextThatResolvesUnnamedStrings(expected);
            var resolver = new OptionalDependencyResolverPolicy(typeof(string));

            object result = resolver.Resolve(context);

            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void ResolverReturnsProperNamedObject()
        {
            string expected = "We want this one";
            string notExpected = "Not this one";

            var expectedKey = NamedTypeBuildKey.Make<string>("expected");
            var notExpectedKey = NamedTypeBuildKey.Make<string>();

            var mainContext = new Mock<IBuilderContext>();
            mainContext.Setup(c => c.NewBuildUp(expectedKey)).Returns(expected);
            mainContext.Setup(c => c.NewBuildUp(notExpectedKey)).Returns(notExpected);

            var resolver = new OptionalDependencyResolverPolicy(typeof(string), "expected");

            object result = resolver.Resolve(mainContext.Object);

            Assert.AreSame(expected, result);
        }

        #region Helper methods to get appropriate OB mock contexts

        IBuilderContext GetMockContextThatThrows()
        {
            var mockContext = new Mock<IBuilderContext>();
            mockContext.Setup(c => c.NewBuildUp(It.IsAny<object>()))
                .Throws(new InvalidOperationException());
            return mockContext.Object;
        }

        IBuilderContext GetMockContextThatResolvesUnnamedStrings(string expected)
        {
            var mockContext = new Mock<IBuilderContext>();
            mockContext.Setup(c => c.NewBuildUp(It.IsAny<object>()))
                .Returns(expected);
            return mockContext.Object;
        }

        #endregion
    }
}
