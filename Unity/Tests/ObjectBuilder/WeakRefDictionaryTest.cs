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
	public class WeakRefDictionaryTest
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void AddingToSameKeyTwiceAlwaysThrows()
		{
			object o = new object();
			WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();
			dict.Add("foo1", o);

			dict.Add("foo1", o);
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void AskingForAKeyThatDoesntExistThrows()
		{
			WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

			object unused = dict["foo"];
		}

		[TestMethod]
		public void CanAddItemAfterPreviousItemIsCollected()
		{
			WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();
			dict.Add("foo", new object());

			GC.Collect();

			dict.Add("foo", new object());
		}

		[TestMethod]
		public void CanAddSameObjectTwiceWithDifferentKeys()
		{
			object o = new object();
			WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

			dict.Add("foo1", o);
			dict.Add("foo2", o);

			Assert.AreSame(dict["foo1"], dict["foo2"]);
		}

		[TestMethod]
		public void CanEnumerate()
		{
			object o1 = new object();
			object o2 = new object();
			WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

			dict.Add("foo1", o1);
			dict.Add("foo2", o2);

			foreach (KeyValuePair<object, object> kvp in dict)
			{
				Assert.IsNotNull(kvp);
				Assert.IsNotNull(kvp.Key);
				Assert.IsNotNull(kvp.Value);
			}
		}

		[TestMethod]
		public void CanFindOutIfContainsAKey()
		{
			WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

			dict.Add("foo", null);
			Assert.IsTrue(dict.ContainsKey("foo"));
			Assert.IsFalse(dict.ContainsKey("foo2"));
		}

		[TestMethod]
		public void CanRegisterObjectAndFindItByID()
		{
			object o = new object();
			WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

			dict.Add("foo", o);
			Assert.IsNotNull(dict["foo"]);
			Assert.AreSame(o, dict["foo"]);
		}

		[TestMethod]
		public void CanRegisterTwoObjectsAndGetThemBoth()
		{
			object o1 = new object();
			object o2 = new object();
			WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

			dict.Add("foo1", o1);
			dict.Add("foo2", o2);

			Assert.AreSame(o1, dict["foo1"]);
			Assert.AreSame(o2, dict["foo2"]);
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void CanRemoveAnObjectThatWasAlreadyAdded()
		{
			object o = new object();
			WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

			dict.Add("foo", o);
			dict.Remove("foo");

			object unused = dict["foo"];
		}

		[TestMethod]
		public void CountReturnsNumberOfKeysWithLiveValues()
		{
			object o = new object();
			WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

			dict.Add("foo1", o);
			dict.Add("foo2", o);

			Assert.AreEqual(2, dict.Count);

			o = null;
			GC.Collect();

			Assert.AreEqual(0, dict.Count);
		}

		[TestMethod]
		public void KeyCanBeOfArbitraryType()
		{
			object oKey = new object();
			object oVal = new object();
			WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

			dict.Add(oKey, oVal);

			Assert.AreSame(oVal, dict[oKey]);
		}

		[TestMethod]
		public void NullIsAValidValue()
		{
			WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();
			dict.Add("foo", null);
			Assert.IsNull(dict["foo"]);
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void RegistrationDoesNotPreventGarbageCollection()
		{
			WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();
			dict.Add("foo", new object());
			GC.Collect();

			object unused = dict["foo"];
		}

		[TestMethod]
		public void RemovingAKeyDoesNotAffectOtherKeys()
		{
			object o1 = new object();
			object o2 = new object();
			WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

			dict.Add("foo1", o1);
			dict.Add("foo2", o2);
			dict.Remove("foo1");

			Assert.AreSame(o2, dict["foo2"]);
		}

		[TestMethod]
		public void RemovingAKeyOfOneObjectDoesNotAffectOtherKeysForSameObject()
		{
			object o = new object();
			WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

			dict.Add("foo1", o);
			dict.Add("foo2", o);
			dict.Remove("foo1");

			Assert.AreSame(o, dict["foo2"]);
		}

		[TestMethod]
		public void RemovingANonExistantKeyDoesntThrow()
		{
			WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();
			dict.Remove("foo1");
		}
	}
}
