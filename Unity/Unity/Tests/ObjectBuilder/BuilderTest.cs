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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
	public class BuilderTest
	{
		[TestClass]
		public class BuildUp
		{

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public void NullStrategies()
			{
				Builder builder = new Builder();

				builder.BuildUp<object>(null, null, null, null, null, null);
			}

			[TestMethod]
			public void StrategyStagesRunInProperOrder()
			{
				Builder builder = new Builder();
				StrategyChain strategies = new StrategyChain();
				strategies.Add(new StringConcatStrategy("1"));
				strategies.Add(new StringConcatStrategy("2"));
				strategies.Add(new StringConcatStrategy("3"));
				strategies.Add(new StringConcatStrategy("4"));

				string s = builder.BuildUp<string>(null, null, null, strategies, null, null);

				Assert.AreEqual("1234", s);
			}
		}

		[TestClass]
		public class TearDown
		{
			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public void NullItem()
			{
				Builder builder = new Builder();
				StrategyChain strategies = new StrategyChain();

				builder.TearDown<object>(null, null, null, strategies, null);
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public void NullStrategies()
			{
				Builder builder = new Builder();

				builder.TearDown(null, null, null, null, new object());
			}

			[TestMethod]
			public void StrategiesRunInReverseOrder()
			{
				Builder builder = new Builder();
				StrategyChain strategies = new StrategyChain();
				strategies.Add(new StringConcatStrategy("1"));
				strategies.Add(new StringConcatStrategy("2"));
				strategies.Add(new StringConcatStrategy("3"));
				strategies.Add(new StringConcatStrategy("4"));

				string s = builder.TearDown(null, null, null, strategies, "");

				Assert.AreEqual("4321", s);
			}
		}

		// Helpers

		class StringConcatStrategy : BuilderStrategy
		{
			public readonly string StringValue;

			public StringConcatStrategy(string value)
			{
				StringValue = value;
			}

			string AppendString(object item)
			{
				string result;

				if (item == null)
					result = StringValue;
				else
					result = ((string)item) + StringValue;

				return result;
			}

			public override void PreBuildUp(IBuilderContext context)
			{
			    context.Existing = AppendString(context.Existing);
			}

			public override void PreTearDown(IBuilderContext context)
			{
			    context.Existing = AppendString(context.Existing);
			}
		}
	}
}
