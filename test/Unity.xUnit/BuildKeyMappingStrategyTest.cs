// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.TestSupport;
using Xunit;

namespace ObjectBuilder2.Tests
{
     
    public class BuildKeyMappingStrategyTest
    {
        [Fact]
        public void CanMapGenericsWithIdenticalGenericParameters()
        {
            // TODO: No longer apply, create new one
            //MockBuilderContext context = new MockBuilderContext();
            //context.Policies.Set<IBuildKeyMappingPolicy>(new GenericTypeBuildKeyMappingPolicy(
            //    new NamedTypeBuildKey(typeof(ConcreteType<>))),
            //    new NamedTypeBuildKey(typeof(ITestType<>)));
            //BuildKeyMappingStrategy strategy = new BuildKeyMappingStrategy();
            //context.Strategies.Add(strategy);
            //SpyStrategy spy = new SpyStrategy();
            //context.Strategies.Add(spy);
            //context.BuildKey = new NamedTypeBuildKey<ITestType<int>>();
            //context.Strategies.ExecuteBuildUp(context);

            //Assert.Equal(new NamedTypeBuildKey(typeof(ConcreteType<int>)), spy.BuildKey);
        }

        [Fact]
        public void CanMapGenericsWithANonTypeBuildKey()
        {
            // TODO: No longer apply, create new one
            //MockBuilderContext context = new MockBuilderContext();
            //context.Policies.Set<IBuildKeyMappingPolicy>(
            //    new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(typeof(ConcreteType<>), "two")),
            //    new NamedTypeBuildKey(typeof(ITestType<>), "one"));

            //BuildKeyMappingStrategy strategy = new BuildKeyMappingStrategy();
            //context.Strategies.Add(strategy);
            //SpyStrategy spy = new SpyStrategy();
            //context.Strategies.Add(spy);
            //context.BuildKey = new NamedTypeBuildKey(typeof(ITestType<int>), "one");
            //context.Strategies.ExecuteBuildUp(context);

            //AssertExtensions.IsInstanceOfType(spy.BuildKey, typeof(NamedTypeBuildKey));
            //Assert.Equal(typeof(ConcreteType<int>), spy.BuildKey.Type);
            //Assert.Equal("two", spy.BuildKey.Name);
        }

        [Fact]
        public void CanMapInterfacesToConcreteTypes()
        {
            // TODO: No longer apply, create new one
            //MockBuilderContext context = new MockBuilderContext();
            //context.Policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(new NamedTypeBuildKey<ConcreteType>()),
            //    new NamedTypeBuildKey<ITestType>());
            //BuildKeyMappingStrategy strategy = new BuildKeyMappingStrategy();
            //context.Strategies.Add(strategy);
            //SpyStrategy spy = new SpyStrategy();
            //context.Strategies.Add(spy);
            //context.BuildKey = new NamedTypeBuildKey<ITestType>();
            //context.Strategies.ExecuteBuildUp(context);

            //Assert.True((new NamedTypeBuildKey(typeof(ConcreteType))).Equals(spy.BuildKey));
        }

        [Fact]
        public void MappingStrategyActuallyReturnsTheBuildKeyThePolicySpecifies()
        {
            // TODO: No longer apply, create new one
            //MockBuilderContext context = new MockBuilderContext();
            //NamedTypeBuildKey fromKey = new NamedTypeBuildKey(typeof(ConcreteType), "id");
            //NamedTypeBuildKey toKey = new NamedTypeBuildKey(typeof(ITestType), "id");
            //context.Policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(toKey), fromKey);
            //BuildKeyMappingStrategy strategy = new BuildKeyMappingStrategy();
            //context.Strategies.Add(strategy);
            //SpyStrategy spy = new SpyStrategy();
            //context.Strategies.Add(spy);
            //context.BuildKey = fromKey;
            //context.Existing = null;
            //context.Strategies.ExecuteBuildUp(context);

            //AssertExtensions.IsInstanceOfType(spy.BuildKey, typeof(NamedTypeBuildKey));
            //Assert.Equal(toKey, spy.BuildKey);
        }

        private class ConcreteType : ITestType { }

        private class ConcreteType<T> : ITestType<T> { }

        private interface ITestType { }

        private interface ITestType<T> { }

        private class SpyStrategy : BuilderStrategy
        {
            public NamedTypeBuildKey BuildKey;

            public override void PreBuildUp(IBuilderContext context)
            {
                this.BuildKey = context.BuildKey;
            }
        }
    }
}
