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
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
	class AssertActualExpectedException : AssertFailedException
	{
		readonly string actual;
		readonly string differencePosition = "";
		readonly string expected;

		public AssertActualExpectedException(object actual,
		                                     object expected,
		                                     string userMessage)
			: this(actual, expected, userMessage, false) {}

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
							break;

						if (!Equals(enumeratorActual.Current, enumeratorExpected.Current))
							break;

						position++;
					}

					differencePosition = "Position: First difference is at position " + position + Environment.NewLine;
				}
			}

			this.actual = actual == null ? null : ConvertToString(actual);
			this.expected = expected == null ? null : ConvertToString(expected);
		}

		public string Actual
		{
			get { return actual; }
		}

		public string Expected
		{
			get { return expected; }
		}

		public override string Message
		{
			get
			{
				return string.Format("{0}{4}{1}Expected: {2}{4}Actual:   {3}",
				                     base.Message,
				                     differencePosition,
				                     FormatMultiLine(Expected ?? "(null)"),
				                     FormatMultiLine(Actual ?? "(null)"),
				                     Environment.NewLine);
			}
		}

		static string ConvertToString(object value)
		{
			Array valueArray = value as Array;
			if (valueArray == null)
				return value.ToString();

			List<string> valueStrings = new List<string>();

			foreach (object valueObject in valueArray)
				valueStrings.Add(valueObject.ToString());

			return value.GetType().FullName + " { " + String.Join(", ", valueStrings.ToArray()) + " }";
		}

		static string FormatMultiLine(string value)
		{
			return value.Replace(Environment.NewLine, Environment.NewLine + "          ");
		}
	}
}
