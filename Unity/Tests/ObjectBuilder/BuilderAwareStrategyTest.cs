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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
	[TestClass]
	public class BuilderAwareStrategyTest
	{
		[TestMethod]
		public void BuildCallsClassWithInterface()
		{
			BuilderAwareStrategy strategy = new BuilderAwareStrategy();
			MockBuilderContext context = new MockBuilderContext();
			Aware obj = new Aware();

			context.Strategies.Add(strategy);

		    context.ExecuteBuildUp(typeof (Aware), obj);

			Assert.IsTrue(obj.OnBuiltUp__Called);
			Assert.IsFalse(obj.OnTearingDown__Called);
			Assert.AreEqual(typeof(Aware), obj.OnBuiltUp_BuildKey);
		}

		[TestMethod]
		public void BuildChecksConcreteTypeAndNotRequestedType()
		{
			BuilderAwareStrategy strategy = new BuilderAwareStrategy();
			MockBuilderContext context = new MockBuilderContext();
			Aware obj = new Aware();

			context.Strategies.Add(strategy);

		    context.ExecuteBuildUp(typeof (Aware), obj);

			Assert.IsTrue(obj.OnBuiltUp__Called);
			Assert.IsFalse(obj.OnTearingDown__Called);
		}

		[TestMethod]
		public void BuildIgnoresClassWithoutInterface()
		{
			BuilderAwareStrategy strategy = new BuilderAwareStrategy();
			MockBuilderContext context = new MockBuilderContext();
			Ignorant obj = new Ignorant();

			context.Strategies.Add(strategy);

		    context.ExecuteBuildUp(typeof (Ignorant), obj);

			Assert.IsFalse(obj.OnBuiltUp__Called);
			Assert.IsFalse(obj.OnTearingDown__Called);
		}

		[TestMethod]
		public void TearDownCallsClassWithInterface()
		{
			BuilderAwareStrategy strategy = new BuilderAwareStrategy();
			MockBuilderContext context = new MockBuilderContext();
			Aware obj = new Aware();

			context.Strategies.Add(strategy);

		    context.ExecuteTearDown(obj);

			Assert.IsFalse(obj.OnBuiltUp__Called);
			Assert.IsTrue(obj.OnTearingDown__Called);
		}

		[TestMethod]
		public void TearDownIgnoresClassWithoutInterface()
		{
			BuilderAwareStrategy strategy = new BuilderAwareStrategy();
			MockBuilderContext context = new MockBuilderContext();
			Ignorant obj = new Ignorant();

			context.Strategies.Add(strategy);

		    context.ExecuteTearDown(obj);

			Assert.IsFalse(obj.OnBuiltUp__Called);
			Assert.IsFalse(obj.OnTearingDown__Called);
		}

		class Aware : Ignorant, IBuilderAware {}

		class Ignorant
		{
			public bool OnBuiltUp__Called = false;
			public object OnBuiltUp_BuildKey = null;
			public bool OnTearingDown__Called = false;

			public void OnBuiltUp(object buildKey)
			{
				OnBuiltUp__Called = true;
				OnBuiltUp_BuildKey = buildKey;
			}

			public void OnTearingDown()
			{
				OnTearingDown__Called = true;
			}
		}
	}
}
