// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.TestSupport;
using System;
using System.Collections;
using System.Collections.Generic;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using AssertFailedException = System.Exception;
#else
using Xunit;
#endif

namespace ObjectBuilder2.Tests
{
    internal class AssertHelper
    {
        public static void Contains<T>(T expected,
                                       IEnumerable<T> collection)
        {
            Contains(expected, collection, GetComparer<T>(), null);
        }

        public static void Contains<T>(T expected,
                                       IEnumerable<T> collection,
                                       string userMessage)
        {
            Contains(expected, collection, GetComparer<T>(), userMessage);
        }

        public static void Contains<T>(T expected,
                                       IEnumerable<T> collection,
                                       IComparer<T> comparer)
        {
            Contains(expected, collection, comparer, null);
        }

        public static void Contains<T>(T expected,
                                       IEnumerable<T> collection,
                                       IComparer<T> comparer,
                                       string userMessage)
        {
            foreach (T item in collection)
            {
                if (comparer.Compare(expected, item) == 0)
                {
                    return;
                }
            }
            throw new AssertFailedException(string.Format("Not found: {0}", expected));
        }

        public static void Contains(string expectedSubString,
                                    string actualString,
                                    string userMessage)
        {
            Contains(expectedSubString, actualString, StringComparison.CurrentCulture, userMessage);
        }

        public static void Contains(string expectedSubString,
                                    string actualString,
                                    StringComparison comparisonType)
        {
            Contains(expectedSubString, actualString, comparisonType, null);
        }

        public static void Contains(string expectedSubString,
                                    string actualString,
                                    StringComparison comparisonType,
                                    string userMessage)
        {
            if (actualString.IndexOf(expectedSubString, comparisonType) < 0)
            {
                throw new AssertFailedException(string.Format("Not found: {0}", expectedSubString));
            }
        }

        public static IComparer<T> GetComparer<T>()
        {
            return new AssertComparer<T>();
        }

        public static T IsType<T>(object @object)
        {
            IsType(typeof(T), @object, null);
            return (T)@object;
        }

        public static void IsType(Type expectedType,
                                  object @object)
        {
            IsType(expectedType, @object, null);
        }

        public static T IsType<T>(object @object,
                                  string userMessage)
        {
            IsType(typeof(T), @object, userMessage);
            return (T)@object;
        }

        public static void IsType(Type expectedType,
                                  object @object,
                                  string userMessage)
        {
            if (!expectedType.Equals(@object.GetType()))
            {
                throw new AssertActualExpectedException(@object, expectedType, userMessage);
            }
        }

        public static void NotEmpty(IEnumerable collection)
        {
            NotEmpty(collection, null);
        }

        public static void NotEmpty(IEnumerable collection,
                                    string userMessage)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection", "cannot be null");
            }
#pragma warning disable 168
            foreach (object @object in collection)
            {
                return;
            }
#pragma warning restore 168

            throw new AssertFailedException(userMessage);
        }

        private class AssertComparer<T> : IComparer<T>
        {
            public int Compare(T x,
                               T y)
            {
                // Compare against null
                if (Object.Equals(x, default(T)))
                {
                    if (Object.Equals(y, default(T)))
                    {
                        return 0;
                    }
                    return -1;
                }

                if (Object.Equals(y, default(T)))
                {
                    return -1;
                }
                // Are they the same type?
                if (x.GetType() != y.GetType())
                {
                    return -1;
                }
                // Are they arrays?
                if (x.GetType().IsArray)
                {
                    Array arrayX = x as Array;
                    Array arrayY = y as Array;

                    if (arrayX != null && arrayY != null)
                    {
                        if (arrayX.Rank != 1)
                        {
                            throw new ArgumentException("Multi-dimension array comparison is not supported");
                        }
                        if (arrayX.Length != arrayY.Length)
                        {
                            return -1;
                        }
                        for (int index = 0; index < arrayX.Length; index++)
                        {
                            if (!Object.Equals(arrayX.GetValue(index), arrayY.GetValue(index)))
                            {
                                return -1;
                            }
                        }
                    }

                    return 0;
                }

                // Compare with IComparable<T>
                IComparable<T> comparable1 = x as IComparable<T>;

                if (comparable1 != null)
                {
                    return comparable1.CompareTo(y);
                }
                // Compare with IComparable
                IComparable comparable2 = x as IComparable;

                if (comparable2 != null)
                {
                    return comparable2.CompareTo(y);
                }
                // Compare with IEquatable
                IEquatable<T> equatable1 = x as IEquatable<T>;

                if (equatable1 != null && equatable1.Equals(y))
                {
                    return 0;
                }
                // Last case, rely on Object.AreEquals
                return Object.Equals(x, y) ? 0 : -1;
            }
        }
    }
}
