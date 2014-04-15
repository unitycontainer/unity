// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using AssertFailedException = NUnit.Framework.AssertionException;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    internal class AssertActualExpectedException : AssertFailedException
    {
        private readonly string actual;
        private readonly string differencePosition = String.Empty;
        private readonly string expected;

        public AssertActualExpectedException(object actual,
                                             object expected,
                                             string userMessage)
            : this(actual, expected, userMessage, false) { }

        public AssertActualExpectedException(object actual,
                                             object expected,
                                             string userMessage,
                                             bool skipPositionCheck)
            : base(userMessage)
        {
            if (!skipPositionCheck)
            {
                IEnumerable enumerableActual = actual as IEnumerable;
                IEnumerable enumerableExpected = expected as IEnumerable;

                if (enumerableActual != null && enumerableExpected != null)
                {
                    IEnumerator enumeratorActual = enumerableActual.GetEnumerator();
                    IEnumerator enumeratorExpected = enumerableExpected.GetEnumerator();
                    int position = 0;

                    while (true)
                    {
                        bool actualHasNext = enumeratorActual.MoveNext();
                        bool expectedHasNext = enumeratorExpected.MoveNext();

                        if (!actualHasNext || !expectedHasNext)
                        {
                            break;
                        }
                        if (!Object.Equals(enumeratorActual.Current, enumeratorExpected.Current))
                        {
                            break;
                        }
                        position++;
                    }

                    this.differencePosition = "Position: First difference is at position " + position + Environment.NewLine;
                }
            }

            this.actual = actual == null ? null : ConvertToString(actual);
            this.expected = expected == null ? null : ConvertToString(expected);
        }

        public string Actual
        {
            get { return this.actual; }
        }

        public string Expected
        {
            get { return this.expected; }
        }

        public override string Message
        {
            get
            {
                return string.Format("{0}{4}{1}Expected: {2}{4}Actual:   {3}",
                                     base.Message,
                                     this.differencePosition,
                                     FormatMultiLine(this.Expected ?? "(null)"),
                                     FormatMultiLine(this.Actual ?? "(null)"),
                                     Environment.NewLine);
            }
        }

        private static string ConvertToString(object value)
        {
            Array valueArray = value as Array;
            if (valueArray == null)
            {
                return value.ToString();
            }
            List<string> valueStrings = new List<string>();

            foreach (object valueObject in valueArray)
            {
                valueStrings.Add(valueObject.ToString());
            }
            return value.GetType().FullName + " { " + String.Join(", ", valueStrings.ToArray()) + " }";
        }

        private static string FormatMultiLine(string value)
        {
            return value.Replace(Environment.NewLine, Environment.NewLine + "          ");
        }
    }
}
