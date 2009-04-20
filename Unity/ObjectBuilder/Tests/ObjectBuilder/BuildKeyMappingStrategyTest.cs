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
	public class BuildKeyMappingStrategyTest
	{
		[TestMethod]
		public void CanMakeArbitraryKeysToConcreteTypes()
		{
			MockBuilderContext context = new MockBuilderContext();
			context.Policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(typeof(Foo)), "bar");
			BuildKeyMappingStrategy strategy = new BuildKeyMappingStrategy();
			context.Strategies.Add(strategy);
			SpyStrategy spy = new SpyStrategy();
			context.Strategies.Add(spy);

		    context.BuildKey = "bar";
		    context.Strategies.ExecuteBuildUp(context);

			Assert.AreEqual<object>(typeof(Foo), spy.BuildKey);
		}

		[TestMethod]
		public void CanMapGenericsWithIdenticalGenericParameters()
		{
			MockBuilderContext context = new MockBuilderContext();
			context.Policies.Set<IBuildKeyMappingPolicy>(new GenericTypeBuildKeyMappingPolicy(typeof(Foo<>)), typeof(IFoo<>));
			BuildKeyMappingStrategy strategy = new BuildKeyMappingStrategy();
			context.Strategies.Add(strategy);
			SpyStrategy spy = new SpyStrategy();
			context.Strategies.Add(spy);
		    context.BuildKey = typeof (IFoo<int>);
		    context.Strategies.ExecuteBuildUp(context);

			Assert.AreEqual<object>(typeof(Foo<int>), spy.BuildKey);
		}

	    [TestMethod]
	    public void CanMapGenericsWithANonTypeBuildKey()
	    {
			MockBuilderContext context = new MockBuilderContext();
			context.Policies.Set<IBuildKeyMappingPolicy>(
                new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(typeof(Foo<>), "two")),
                new NamedTypeBuildKey(typeof(IFoo<>), "one"));

			BuildKeyMappingStrategy strategy = new BuildKeyMappingStrategy();
			context.Strategies.Add(strategy);
			SpyStrategy spy = new SpyStrategy();
			context.Strategies.Add(spy);
	        context.BuildKey = new NamedTypeBuildKey(typeof (IFoo<int>), "one");
		    context.Strategies.ExecuteBuildUp(context);

	        Assert.IsInstanceOfType(spy.BuildKey, typeof (NamedTypeBuildKey));
			Assert.AreEqual<object>(typeof(Foo<int>), BuildKey.GetType(spy.BuildKey));
	        Assert.AreEqual("two", ((NamedTypeBuildKey) spy.BuildKey).Name);
	    }

		[TestMethod]
		public void CanMapInterfacesToConcreteTypes()
		{
			MockBuilderContext context = new MockBuilderContext();
			context.Policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(typeof(Foo)), typeof(IFoo));
			BuildKeyMappingStrategy strategy = new BuildKeyMappingStrategy();
			context.Strategies.Add(strategy);
			SpyStrategy spy = new SpyStrategy();
			context.Strategies.Add(spy);
		    context.BuildKey = typeof (IFoo);
		    context.Strategies.ExecuteBuildUp(context);

			Assert.AreEqual<object>(typeof(Foo), spy.BuildKey);
		}

	    [TestMethod]
	    public void MappingStrategyActuallyReturnsTheBuildKeyThePolicySpecifies()
	    {
            MockBuilderContext context = new MockBuilderContext();
	        NamedTypeBuildKey fromKey = new NamedTypeBuildKey(typeof (Foo), "id");
	        NamedTypeBuildKey toKey = new NamedTypeBuildKey(typeof (IFoo), "id");
            context.Policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(toKey), fromKey);
            BuildKeyMappingStrategy strategy = new BuildKeyMappingStrategy();
            context.Strategies.Add(strategy);
            SpyStrategy spy = new SpyStrategy();
            context.Strategies.Add(spy);
	        context.BuildKey = fromKey;
	        context.Existing = null;
	        context.Strategies.ExecuteBuildUp(context);

	        Assert.IsInstanceOfType(spy.BuildKey, typeof (NamedTypeBuildKey));
            Assert.AreEqual(toKey, spy.BuildKey);
	    }

		class Foo : IFoo {}

		class Foo<T> : IFoo<T> {}

		interface IFoo {}

		interface IFoo<T> {}

		class SpyStrategy : BuilderStrategy
		{
			public object BuildKey;

			public override void PreBuildUp(IBuilderContext context)
			{
				BuildKey = context.BuildKey;
			}
		}
	}
}
