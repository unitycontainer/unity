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
        public void BuildKeyMethodsWrapTypeObjects()
        {
            Type t = typeof (string);
            Type t2 = BuildKey.GetType(t);

            Assert.AreEqual(t, t2);
        }

        [TestMethod]
        public void CanChangeTypeOfTypeBuildKey()
        {
            Type t = typeof (string);
            Type t2 = (Type)BuildKey.ReplaceType(t, typeof (int));

            Assert.AreEqual(typeof (int), t2);
        }

        [TestMethod]
        public void CanMapGenericTypeToNewGenericType()
        {
            Type original = typeof (IList<int>);
            Type expected = typeof (List<int>);

            IBuildKeyMappingPolicy policy = new GenericTypeBuildKeyMappingPolicy(typeof (List<>));

            Type result = (Type)policy.Map(original);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CanMapGenericTypeFromNamedTypeBuildKey()
        {
            NamedTypeBuildKey original = new NamedTypeBuildKey(typeof (IList<string>), "test");
            IBuildKeyMappingPolicy policy = new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(typeof (List<>), "test"));

            NamedTypeBuildKey result = (NamedTypeBuildKey) policy.Map(original);

            Assert.AreEqual(typeof (List<string>), result.Type);
            Assert.AreEqual(original.Name, result.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PolicyThrowsIfWrongNumberOfGenericParameters()
        {
            Type original = typeof (IList<string>);
            IBuildKeyMappingPolicy policy = new GenericTypeBuildKeyMappingPolicy(typeof (Dictionary<,>));
            policy.Map(original);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PolicyThrowsIfInputIsNotAGeneric()
        {
            IBuildKeyMappingPolicy policy = new GenericTypeBuildKeyMappingPolicy(typeof (List<>));
            policy.Map(typeof (int));
        }

    }
}
