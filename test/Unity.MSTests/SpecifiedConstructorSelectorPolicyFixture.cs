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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.ObjectBuilder;
using ObjectBuilder2;
using Unity;

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
            BuilderContextMock builderContext = new BuilderContextMock(typeof(ClassWithSimpleConstructor));

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

            BuilderContextMock builderContext = new BuilderContextMock(typeof(ClassWithConstructorParameters));

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
            ctx.BuildKey = typeof(LoggingCommand<User>);

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
            private IPolicyList persistentPolicies = new PolicyList();
            private object buildKey;


            public BuilderContextMock()
            {
            }

            public BuilderContextMock(object buildKey)
            {
                this.buildKey = buildKey;
            }

            public IStrategyChain Strategies
            {
                get { throw new NotImplementedException(); }
            }

            public ILifetimeContainer Lifetime
            {
                get { throw new NotImplementedException(); }
            }

            public object OriginalBuildKey
            {
                get { return buildKey; }
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

            public object BuildKey
            {
                get { return buildKey; }
                set { buildKey = value; }
            }

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

            public object NewBuildUp(object newBuildKey)
            {
                throw new NotImplementedException();
            }

            public object NewBuildUp(object newBuildKey, Action<IPolicyList> policyAdderBlock)
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
