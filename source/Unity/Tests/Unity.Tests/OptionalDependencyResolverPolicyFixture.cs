// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using ObjectBuilder2;
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

namespace Unity.Tests
{
    /// <summary>
    /// Summary description for OptionalDependencyResolverPolicyFixture
    /// </summary>
     
    public class OptionalDependencyResolverPolicyFixture
    {
        [Fact]
        public void CanCreateResolverWithNoName()
        {
            var resolver = new OptionalDependencyResolverPolicy(typeof(object));
            Assert.Equal(typeof(object), resolver.DependencyType);
            Assert.Null(resolver.Name);
        }

        [Fact]
        public void CanCreateResolverWithName()
        {
            var resolver = new OptionalDependencyResolverPolicy(typeof(object), "name");
            Assert.Equal(typeof(object), resolver.DependencyType);
            Assert.Equal("name", resolver.Name);
        }

        [Fact]
        public void ResolverReturnsNullWhenDependencyIsNotResolved()
        {
            IBuilderContext context = GetMockContextThatThrows();
            var resolver = new OptionalDependencyResolverPolicy(typeof(object));

            object result = resolver.Resolve(context);

            Assert.Null(result);
        }

        [Fact]
        public void ResolverReturnsBuiltObject()
        {
            string expected = "Here's the string to resolve";
            IBuilderContext context = GetMockContextThatResolvesUnnamedStrings(expected);
            var resolver = new OptionalDependencyResolverPolicy(typeof(string));

            object result = resolver.Resolve(context);

            Assert.Same(expected, result);
        }

        [Fact]
        public void ResolverReturnsProperNamedObject()
        {
            string expected = "We want this one";
            string notExpected = "Not this one";

            var expectedKey = NamedTypeBuildKey.Make<string>("expected");
            var notExpectedKey = NamedTypeBuildKey.Make<string>();

            var mainContext = new MockContext();
            mainContext.NewBuildupCallback = (k) =>
            {
                if (k == expectedKey)
                {
                    return expected;
                }
                if (k == notExpectedKey)
                {
                    return notExpected;
                }
                return null;
            };

            var resolver = new OptionalDependencyResolverPolicy(typeof(string), "expected");

            object result = resolver.Resolve(mainContext);

            Assert.Same(expected, result);
        }

        #region Helper methods and classes to get appropriate OB mock contexts

        private IBuilderContext GetMockContextThatThrows()
        {
            var mockContext = new MockContext();
            mockContext.NewBuildupCallback = (c) => { throw new InvalidOperationException(); };
            return mockContext;
        }

        private IBuilderContext GetMockContextThatResolvesUnnamedStrings(string expected)
        {
            var mockContext = new MockContext();
            mockContext.NewBuildupCallback = (c) =>
            {
                return expected;
            };
            return mockContext;
        }

        public class MockContext : IBuilderContext
        {
            public Func<NamedTypeBuildKey, object> NewBuildupCallback;

            public IStrategyChain Strategies
            {
                get { throw new NotImplementedException(); }
            }

            public ILifetimeContainer Lifetime
            {
                get { throw new NotImplementedException(); }
            }

            public NamedTypeBuildKey OriginalBuildKey
            {
                get { throw new NotImplementedException(); }
            }

            public NamedTypeBuildKey BuildKey
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public IPolicyList PersistentPolicies
            {
                get { throw new NotImplementedException(); }
            }

            public IPolicyList Policies
            {
                get { throw new NotImplementedException(); }
            }

            public IRecoveryStack RecoveryStack
            {
                get { throw new NotImplementedException(); }
            }

            public object Existing
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public bool BuildComplete
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public object CurrentOperation
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public IBuilderContext ChildContext
            {
                get { throw new NotImplementedException(); }
            }

            public void AddResolverOverrides(System.Collections.Generic.IEnumerable<ResolverOverride> newOverrides)
            {
                throw new NotImplementedException();
            }

            public IDependencyResolverPolicy GetOverriddenResolver(Type dependencyType)
            {
                throw new NotImplementedException();
            }

            public object NewBuildUp(NamedTypeBuildKey newBuildKey)
            {
                return NewBuildupCallback(newBuildKey);
            }

            public object NewBuildUp(NamedTypeBuildKey newBuildKey, Action<IBuilderContext> childCustomizationBlock)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
