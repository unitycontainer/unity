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
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            ConstructorInfo ctor = typeof(ClassWithSimpleConstructor).GetConstructor(new Type[0]);

            SpecifiedConstructorSelectorPolicy policy = new SpecifiedConstructorSelectorPolicy(ctor, new InjectionParameterValue[0]);
            BuilderContextMock builderContext = new BuilderContextMock(new NamedTypeBuildKey(typeof(ClassWithSimpleConstructor)));

            SelectedConstructor selectedCtor = policy.SelectConstructor(builderContext);

            Assert.AreEqual(ctor, selectedCtor.Constructor);
            Assert.AreEqual(0, selectedCtor.GetParameterKeys().Length);

        }

        [TestMethod]
        public void SelectConstructorWith2Parameters()
        {
            ConstructorInfo ctor = typeof(ClassWithConstructorParameters).GetConstructor(Types(typeof(int), typeof(string)));

            SpecifiedConstructorSelectorPolicy policy = new SpecifiedConstructorSelectorPolicy(ctor,
                new InjectionParameterValue[]
                {
                    new InjectionParameter<int>(37),
                    new InjectionParameter<string>("abc")
                });

            BuilderContextMock builderContext = new BuilderContextMock(new NamedTypeBuildKey(typeof(ClassWithConstructorParameters)));

            SelectedConstructor selectedCtor = policy.SelectConstructor(builderContext);

            Assert.AreEqual(ctor, selectedCtor.Constructor);
            Assert.AreEqual(2, selectedCtor.GetParameterKeys().Length);

            string[] keys = selectedCtor.GetParameterKeys();
            Assert.AreEqual(2, keys.Length);
            foreach (string key in keys)
            {
                AssertPolicyIsCorrect(key, builderContext);
            }
        }

        [TestMethod]
        public void CanSelectConcreteConstructorGivenGenericConstructor()
        {
            ConstructorInfo ctor = typeof(LoggingCommand<>).GetConstructors()[0];
            SpecifiedConstructorSelectorPolicy policy = new SpecifiedConstructorSelectorPolicy(
                ctor,
                new InjectionParameterValue[]
                {
                    new ResolvedParameter(typeof (ICommand<>), "concrete")
                });

            BuilderContextMock ctx = new BuilderContextMock();
            ctx.BuildKey = new NamedTypeBuildKey(typeof(LoggingCommand<User>));

            SelectedConstructor result = policy.SelectConstructor(ctx);

            ConstructorInfo expectedCtor = typeof(LoggingCommand<User>).GetConstructor(Types(typeof(ICommand<User>)));
            Assert.AreSame(expectedCtor, result.Constructor);
        }

        private void AssertPolicyIsCorrect(string key, IBuilderContext context)
        {
            IDependencyResolverPolicy policy = context.PersistentPolicies.Get<IDependencyResolverPolicy>(key);
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
