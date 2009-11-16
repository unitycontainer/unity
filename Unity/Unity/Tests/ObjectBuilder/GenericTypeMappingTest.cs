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
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public class GenericTypeMappingTest
    {
        [TestMethod]
        public void CanMapGenericTypeToNewGenericType()
        {
            var original = new NamedTypeBuildKey(typeof (IList<int>));
            var expected = new NamedTypeBuildKey(typeof (List<int>));

            IBuildKeyMappingPolicy policy = new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(typeof (List<>)));

            var result = policy.Map(original);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CanMapGenericTypeFromNamedTypeBuildKey()
        {
            NamedTypeBuildKey original = new NamedTypeBuildKey(typeof (IList<string>), "test");
            IBuildKeyMappingPolicy policy = new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(typeof (List<>), "test"));

            NamedTypeBuildKey result = policy.Map(original);

            Assert.AreEqual(typeof (List<string>), result.Type);
            Assert.AreEqual(original.Name, result.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PolicyThrowsIfWrongNumberOfGenericParameters()
        {
            var original = new NamedTypeBuildKey(typeof (IList<string>));
            IBuildKeyMappingPolicy policy = new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(typeof (Dictionary<,>)));
            policy.Map(original);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PolicyThrowsIfInputIsNotAGeneric()
        {
            IBuildKeyMappingPolicy policy = new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(typeof (List<>)));
            policy.Map(new NamedTypeBuildKey<int>());
        }

    }
}
