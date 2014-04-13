// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.Unity.TestSupport;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.Tests
{
    /// <summary>
    /// Summary description for SpecifiedConstructorSelectorPolicyFixture
    /// </summary>
    [TestClass]
    public class SpecifiedConstructorSelectorPolicyFixture
    {
        [TestMethod]
        public void SelectConstructorWithNoParameters()
        {
            ConstructorInfo ctor = typeof(ClassWithSimpleConstructor).GetMatchingConstructor(new Type[0]);

            var policy = new SpecifiedConstructorSelectorPolicy(ctor, new InjectionParameterValue[0]);
            var builderContext = new BuilderContextMock(new NamedTypeBuildKey(typeof(ClassWithSimpleConstructor)));

            SelectedConstructor selectedCtor = policy.SelectConstructor(builderContext, new PolicyList());

            Assert.AreEqual(ctor, selectedCtor.Constructor);
            Assert.AreEqual(0, selectedCtor.GetParameterResolvers().Length);
        }

        [TestMethod]
        public void SelectConstructorWith2Parameters()
        {
            ConstructorInfo ctor = typeof(ClassWithConstructorParameters).GetMatchingConstructor(Types(typeof(int), typeof(string)));

            var policy = new SpecifiedConstructorSelectorPolicy(ctor,
                new InjectionParameterValue[]
                {
                    new InjectionParameter<int>(37),
                    new InjectionParameter<string>("abc")
                });

            var builderContext = new BuilderContextMock(new NamedTypeBuildKey(typeof(ClassWithConstructorParameters)));

            SelectedConstructor selectedCtor = policy.SelectConstructor(builderContext, builderContext.PersistentPolicies);

            Assert.AreEqual(ctor, selectedCtor.Constructor);
            Assert.AreEqual(2, selectedCtor.GetParameterResolvers().Length);

            var resolvers = selectedCtor.GetParameterResolvers();
            Assert.AreEqual(2, resolvers.Length);
            foreach (var resolverPolicy in resolvers)
            {
                AssertPolicyIsCorrect(resolverPolicy);
            }
        }

        [TestMethod]
        public void CanSelectConcreteConstructorGivenGenericConstructor()
        {
            ConstructorInfo ctor = typeof(LoggingCommand<>).GetTypeInfo().DeclaredConstructors.ElementAt(0);
            var policy = new SpecifiedConstructorSelectorPolicy(
                ctor,
                new InjectionParameterValue[]
                {
                    new ResolvedParameter(typeof(ICommand<>), "concrete")
                });

            var ctx = new BuilderContextMock
                {
                    BuildKey = new NamedTypeBuildKey(typeof(LoggingCommand<User>))
                };

            SelectedConstructor result = policy.SelectConstructor(ctx, new PolicyList());

            ConstructorInfo expectedCtor = typeof(LoggingCommand<User>).GetMatchingConstructor(Types(typeof(ICommand<User>)));
            Assert.AreSame(expectedCtor, result.Constructor);
        }

        private static void AssertPolicyIsCorrect(IDependencyResolverPolicy policy)
        {
            Assert.IsNotNull(policy);
            Assert.IsInstanceOfType(policy, typeof(LiteralValueDependencyResolverPolicy));
        }

        private Type[] Types(params Type[] types)
        {
            return types;
        }

        private class ClassWithSimpleConstructor
        {
        }

        private class ClassWithConstructorParameters
        {
            public ClassWithConstructorParameters(int param1, string param2)
            {
            }
        }

        private class BuilderContextMock : IBuilderContext
        {
            private readonly IPolicyList persistentPolicies = new PolicyList();

            public BuilderContextMock()
            {
            }

            public BuilderContextMock(NamedTypeBuildKey buildKey)
            {
                this.BuildKey = buildKey;
            }

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
                get { return BuildKey; }
            }

            public IPolicyList PersistentPolicies
            {
                get { return persistentPolicies; }
            }

            public IPolicyList Policies
            {
                get { throw new NotImplementedException(); }
            }

            public IRecoveryStack RecoveryStack
            {
                get { throw new NotImplementedException(); }
            }

            public NamedTypeBuildKey BuildKey { get; set; }

            public object Existing
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public bool BuildComplete
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public object CurrentOperation
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public IBuilderContext ChildContext
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public object NewBuildUp(NamedTypeBuildKey newBuildKey)
            {
                throw new NotImplementedException();
            }

            public object NewBuildUp(NamedTypeBuildKey newBuildKey, Action<IBuilderContext> childCustomizationBlock)
            {
                throw new NotImplementedException();
            }

            public void AddResolverOverrides(IEnumerable<ResolverOverride> newOverrides)
            {
                throw new NotImplementedException();
            }

            public IDependencyResolverPolicy GetOverriddenResolver(Type dependencyType)
            {
                throw new NotImplementedException();
            }
        }
    }
}
