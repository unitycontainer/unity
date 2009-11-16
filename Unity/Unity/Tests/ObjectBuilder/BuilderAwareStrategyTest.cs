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

using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
	[TestClass]
	public class BuilderAwareStrategyTest
	{
		[TestMethod]
		public void BuildCallsClassWithInterface()
		{
			var strategy = new BuilderAwareStrategy();
			var context = new MockBuilderContext();
			var obj = new Aware();

			context.Strategies.Add(strategy);

		    context.ExecuteBuildUp(new NamedTypeBuildKey<Aware>(), obj);

			Assert.IsTrue(obj.OnBuiltUp__Called);
			Assert.IsFalse(obj.OnTearingDown__Called);
			Assert.AreEqual(new NamedTypeBuildKey<Aware>(), obj.OnBuiltUp_BuildKey);
		}

		[TestMethod]
		public void BuildChecksConcreteTypeAndNotRequestedType()
		{
			var strategy = new BuilderAwareStrategy();
			var context = new MockBuilderContext();
			var obj = new Aware();

			context.Strategies.Add(strategy);

		    context.ExecuteBuildUp(new NamedTypeBuildKey<Aware>(), obj);

			Assert.IsTrue(obj.OnBuiltUp__Called);
			Assert.IsFalse(obj.OnTearingDown__Called);
		}

		[TestMethod]
		public void BuildIgnoresClassWithoutInterface()
		{
			var strategy = new BuilderAwareStrategy();
			var context = new MockBuilderContext();
			var obj = new Ignorant();

			context.Strategies.Add(strategy);

		    context.ExecuteBuildUp(new NamedTypeBuildKey<Ignorant>(), obj);

			Assert.IsFalse(obj.OnBuiltUp__Called);
			Assert.IsFalse(obj.OnTearingDown__Called);
		}

		[TestMethod]
		public void TearDownCallsClassWithInterface()
		{
			var strategy = new BuilderAwareStrategy();
			var context = new MockBuilderContext();
			var obj = new Aware();

			context.Strategies.Add(strategy);

		    context.ExecuteTearDown(obj);

			Assert.IsFalse(obj.OnBuiltUp__Called);
			Assert.IsTrue(obj.OnTearingDown__Called);
		}

		[TestMethod]
		public void TearDownIgnoresClassWithoutInterface()
		{
			var strategy = new BuilderAwareStrategy();
			var context = new MockBuilderContext();
			var obj = new Ignorant();

			context.Strategies.Add(strategy);

		    context.ExecuteTearDown(obj);

			Assert.IsFalse(obj.OnBuiltUp__Called);
			Assert.IsFalse(obj.OnTearingDown__Called);
		}

		class Aware : Ignorant, IBuilderAware {}

		class Ignorant
		{
			public bool OnBuiltUp__Called;
			public NamedTypeBuildKey OnBuiltUp_BuildKey;
			public bool OnTearingDown__Called;

			public void OnBuiltUp(NamedTypeBuildKey buildKey)
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
