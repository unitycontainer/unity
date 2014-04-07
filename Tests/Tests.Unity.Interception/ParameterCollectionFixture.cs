// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    [TestClass]
    public class ParameterCollectionFixture
    {
        [TestMethod]
        public void CanAccessNonFilteredParameters()
        {
            var collection =
                new ParameterCollection(
                    new object[] { 10, 20, 30, 40, 50 },
                    StaticReflection.GetMethodInfo(() => TestMethod(null, null, null, null, null)).GetParameters(),
                    pi => true);

            Assert.AreEqual(5, collection.Count);
            CollectionAssertExtensions.AreEqual(
                new[] { 10, 20, 30, 40, 50 },
                collection);
            CollectionAssertExtensions.AreEqual(
                new[] { 50, 20, 40, 30, 10 },
                new[] { 4, 1, 3, 2, 0 }.Select(i => collection[i]).ToArray());
            CollectionAssertExtensions.AreEqual(
                new[] { 50, 20, 40, 30, 10 },
                new[] { "param5", "param2", "param4", "param3", "param1" }.Select(i => collection[i]).ToArray());
            CollectionAssertExtensions.AreEqual(
                new[] { "param1", "param2", "param3", "param4", "param5" },
                Enumerable.Range(0, 5).Select(i => collection.ParameterName(i)).ToArray());
            CollectionAssertExtensions.AreEqual(
                new[] { "param1", "param2", "param3", "param4", "param5" },
                Enumerable.Range(0, 5).Select(i => collection.GetParameterInfo(i).Name).ToArray());
        }

        [TestMethod]
        public void CanAccessFilteredParameters()
        {
            var param = 1;
            var collection =
                new ParameterCollection(
                    new object[] { 10, 20, 30, 40, 50 },
                    StaticReflection.GetMethodInfo(() => TestMethod(null, null, null, null, null)).GetParameters(),
                    pi => param++ % 2 == 1);

            Assert.AreEqual(3, collection.Count);
            CollectionAssertExtensions.AreEqual(
                new[] { 10, 30, 50 },
                collection);
            CollectionAssertExtensions.AreEqual(
                new[] { 50, 30, 10 },
                new[] { 2, 1, 0 }.Select(i => collection[i]).ToArray());
            CollectionAssertExtensions.AreEqual(
                new[] { 50, 30, 10 },
                new[] { "param5", "param3", "param1" }.Select(i => collection[i]).ToArray());
            CollectionAssertExtensions.AreEqual(
                new[] { "param1", "param3", "param5" },
                Enumerable.Range(0, 3).Select(i => collection.ParameterName(i)).ToArray());
            CollectionAssertExtensions.AreEqual(
                new[] { "param1", "param3", "param5" },
                Enumerable.Range(0, 3).Select(i => collection.GetParameterInfo(i).Name).ToArray());
        }

        [TestMethod]
        public void FilteredCollectionReturnsRightParameterByName()
        {
            object dummy;
            object dummy2;
            var inputsCollection =
                new ParameterCollection(new object[] { "one", "two", "three", "four" },
                    StaticReflection.GetMethodInfo(() => MethodWithOuts(out dummy, null, out dummy2, null)).GetParameters(),
                    pi => !pi.IsOut);

            Assert.AreEqual(2, inputsCollection.Count);
            CollectionAssertExtensions.AreEqual(new object[] { "two", "four" }, inputsCollection);
            Assert.AreEqual("two", inputsCollection["param2"]);
            Assert.AreEqual("four", inputsCollection["param4"]);
        }

        [TestMethod]
        public void WhenParameterValueIsNull()
        {
            var collection =
                new ParameterCollection(
                    new object[] { null },
                    StaticReflection.GetMethodInfo(() => TestMethod(null, null, null, null, null)).GetParameters(),
                    p => true);

            var result = collection.Contains(null);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsParameterWorksAsExpected()
        {
            var collection =
                new ParameterCollection(
                    new object[] { null },
                    StaticReflection.GetMethodInfo(() => TestMethod(null, null, null, null, null)).GetParameters(),
                    p => true);

            Assert.IsTrue(new[] { "param1", "param2", "param3", "param4", "param5" }.All(collection.ContainsParameter));
            Assert.IsTrue(new[] { "someOtherParam", "notThisOneEither" }.All(p => !collection.ContainsParameter(p)));
        }

        public static void TestMethod(object param1, object param2, object param3, object param4, object param5)
        {
        }

        public static void MethodWithOuts(out object param1, object param2, out object param3, object param4)
        {
            param1 = null;
            param3 = null;
        }
    }
}
