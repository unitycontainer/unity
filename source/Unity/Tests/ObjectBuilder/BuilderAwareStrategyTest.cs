// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.TestSupport;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#else
using Xunit;
#endif

namespace ObjectBuilder2.Tests
{
     
    public class BuilderAwareStrategyTest
    {
        [Fact]
        public void BuildCallsClassWithInterface()
        {
            var strategy = new BuilderAwareStrategy();
            var context = new MockBuilderContext();
            var obj = new Aware();

            context.Strategies.Add(strategy);

            context.ExecuteBuildUp(new NamedTypeBuildKey<Aware>(), obj);

            Assert.True(obj.OnBuiltUpWasCalled);
            Assert.False(obj.OnTearingDownWasCalled);
            Assert.Equal(new NamedTypeBuildKey<Aware>(), obj.OnBuiltUpBuildKey);
        }

        [Fact]
        public void BuildChecksConcreteTypeAndNotRequestedType()
        {
            var strategy = new BuilderAwareStrategy();
            var context = new MockBuilderContext();
            var obj = new Aware();

            context.Strategies.Add(strategy);

            context.ExecuteBuildUp(new NamedTypeBuildKey<Aware>(), obj);

            Assert.True(obj.OnBuiltUpWasCalled);
            Assert.False(obj.OnTearingDownWasCalled);
        }

        [Fact]
        public void BuildIgnoresClassWithoutInterface()
        {
            var strategy = new BuilderAwareStrategy();
            var context = new MockBuilderContext();
            var obj = new Ignorant();

            context.Strategies.Add(strategy);

            context.ExecuteBuildUp(new NamedTypeBuildKey<Ignorant>(), obj);

            Assert.False(obj.OnBuiltUpWasCalled);
            Assert.False(obj.OnTearingDownWasCalled);
        }

        [Fact]
        public void TearDownCallsClassWithInterface()
        {
            var strategy = new BuilderAwareStrategy();
            var context = new MockBuilderContext();
            var obj = new Aware();

            context.Strategies.Add(strategy);

            context.ExecuteTearDown(obj);

            Assert.False(obj.OnBuiltUpWasCalled);
            Assert.True(obj.OnTearingDownWasCalled);
        }

        [Fact]
        public void TearDownIgnoresClassWithoutInterface()
        {
            var strategy = new BuilderAwareStrategy();
            var context = new MockBuilderContext();
            var obj = new Ignorant();

            context.Strategies.Add(strategy);

            context.ExecuteTearDown(obj);

            Assert.False(obj.OnBuiltUpWasCalled);
            Assert.False(obj.OnTearingDownWasCalled);
        }

        private class Aware : Ignorant, IBuilderAware { }

        private class Ignorant
        {
            public bool OnBuiltUpWasCalled;
            public NamedTypeBuildKey OnBuiltUpBuildKey;
            public bool OnTearingDownWasCalled;

            public void OnBuiltUp(NamedTypeBuildKey buildKey)
            {
                OnBuiltUpWasCalled = true;
                OnBuiltUpBuildKey = buildKey;
            }

            public void OnTearingDown()
            {
                OnTearingDownWasCalled = true;
            }
        }
    }
}
