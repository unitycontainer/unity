// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using AssertFailedException = NUnit.Framework.AssertionException;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Unity.TestSupport
{
    public static class CollectionAssertExtensions
    {
        public static void AreEqual(ICollection expected, ICollection actual)
        {
            CollectionAssertExtensions.AreEqual(expected, actual, new DefaultComparer());
        }

        public static void AreEqual(ICollection expected, ICollection actual, IComparer comparer)
        {
            CollectionAssertExtensions.AreEqual(expected, actual, comparer, string.Empty);
        }

        public static void AreEqual(ICollection expected, ICollection actual, string message)
        {
            CollectionAssertExtensions.AreEqual(expected, actual, new DefaultComparer(), message);
        }

        public static void AreEqual(ICollection expected, ICollection actual, IComparer comparer, string message)
        {
            string reason;
            if (!CollectionAssertExtensions.AreCollectionsEqual(expected, actual, comparer, out reason))
            {
                throw new AssertFailedException(string.Format(CultureInfo.CurrentCulture, "{0}({1})", message, reason));
            }
        }

        public static void AreEquivalent(ICollection expected, ICollection actual)
        {
            if (expected == actual)
            {
                return;
            }

            if (expected.Count != actual.Count)
            {
                throw new AssertFailedException("collections differ in size");
            }

            var expectedCounts = expected.Cast<object>().GroupBy(e => e).ToDictionary(g => g.Key, g => g.Count());
            var actualCounts = actual.Cast<object>().GroupBy(e => e).ToDictionary(g => g.Key, g => g.Count());

            foreach (var kvp in expectedCounts)
            {
                int actualCount = 0;
                if (actualCounts.TryGetValue(kvp.Key, out actualCount))
                {
                    if (actualCount != kvp.Value)
                    {
                        throw new AssertFailedException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "collections have different count for element {0}", kvp.Key));
                    }
                }
                else
                {
                    throw new AssertFailedException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "actual does not contain element {0}", kvp.Key));
                }
            }
        }

        private static bool AreCollectionsEqual(ICollection expected, ICollection actual, IComparer comparer, out string reason)
        {
            if (expected == actual)
            {
                reason = null;
                return true;
            }

            if (expected.Count != actual.Count)
            {
                reason = "collections differ in size";
                return false;
            }

            var expectedEnum = expected.GetEnumerator();
            var actualEnum = actual.GetEnumerator();
            for (int i = 0; expectedEnum.MoveNext() && actualEnum.MoveNext(); i++)
            {
                if (comparer.Compare(expectedEnum.Current, actualEnum.Current) != 0)
                {
                    reason = string.Format(CultureInfo.CurrentCulture, "collections differ at index {0}", i);
                    return false;
                }
            }

            reason = null;
            return true;
        }

        private class DefaultComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return x.Equals(y) ? 0 : -1;
            }
        }
    }
}
