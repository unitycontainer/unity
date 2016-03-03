// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
     
    public class PolicyListTest
    {
        [Fact]
        public void CanAddMultiplePoliciesToBagAndRetrieveThem()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy1 = new FakePolicy();
            FakePolicy policy2 = new FakePolicy();

            list.Set<IBuilderPolicy>(policy1, "1");
            list.Set<IBuilderPolicy>(policy2, "2");

            Assert.Same(policy1, list.Get<IBuilderPolicy>("1"));
            Assert.Same(policy2, list.Get<IBuilderPolicy>("2"));
        }

        [Fact]
        public void CanAddPolicyToBagAndRetrieveIt()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy = new FakePolicy();
            list.Set<IBuilderPolicy>(policy, typeof(object));

            IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(object));

            Assert.Same(policy, result);
        }

        [Fact]
        public void CanClearAllPolicies()
        {
            PolicyList list = new PolicyList();
            list.Set<IBuilderPolicy>(new FakePolicy(), "1");
            list.Set<IBuilderPolicy>(new FakePolicy(), "2");

            list.ClearAll();

            Assert.Equal(0, list.Count);
        }

        [Fact]
        public void CanClearDefaultPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy defaultPolicy = new FakePolicy();
            list.SetDefault<IBuilderPolicy>(defaultPolicy);

            list.ClearDefault<IBuilderPolicy>();

            IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(object));
            Assert.Null(result);
        }

        [Fact]
        public void CanClearPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy = new FakePolicy();

            list.Set<IBuilderPolicy>(policy, typeof(string));
            list.Clear<IBuilderPolicy>(typeof(string));

            Assert.Null(list.Get<IBuilderPolicy>(typeof(string)));
        }

        [Fact]
        public void CanGetLocalPolicy()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy innerPolicy = new FakePolicy();
            innerList.Set(innerPolicy, typeof(object));

            IPolicyList containingPolicyList;
            FakePolicy result = outerList.Get<FakePolicy>(typeof(object), true, out containingPolicyList);

            Assert.Null(result);
            Assert.Null(containingPolicyList);
        }

        [Fact]
        public void CanRegisterGenericPolicyAndRetrieveWithSpecificGenericInstance()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy = new FakePolicy();
            list.Set(policy, typeof(IDummy<>));

            FakePolicy result = list.Get<FakePolicy>(typeof(IDummy<int>));

            Assert.Same(policy, result);
        }

        [Fact]
        public void DefaultPolicyUsedWhenSpecificPolicyIsntAvailable()
        {
            PolicyList list = new PolicyList();
            FakePolicy defaultPolicy = new FakePolicy();
            list.SetDefault<IBuilderPolicy>(defaultPolicy);

            IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(object));

            Assert.Same(defaultPolicy, result);
        }

        [Fact]
        public void PolicyRegisteredForTypeIsUsedIfKeyIsNotFound()
        {
            PolicyList list = new PolicyList();
            FakePolicy policyForType = new FakePolicy();
            list.Set<IBuilderPolicy>(policyForType, typeof(object));

            IBuilderPolicy result = list.Get<IBuilderPolicy>(new NamedTypeBuildKey<object>("name"));

            Assert.Same(policyForType, result);
        }

        [Fact]
        public void PolicyForClosedGenericTypeOverridesPolicyForOpenType()
        {
            PolicyList list = new PolicyList();
            FakePolicy openTypePolicy = new FakePolicy();
            FakePolicy closedTypePolicy = new FakePolicy();
            list.Set<IBuilderPolicy>(openTypePolicy, typeof(IDummy<>));
            list.Set<IBuilderPolicy>(closedTypePolicy, typeof(IDummy<object>));

            IBuilderPolicy result = list.Get<IBuilderPolicy>(new NamedTypeBuildKey<IDummy<object>>("name"));
            Assert.Same(closedTypePolicy, result);
        }

        [Fact]
        public void PolicyRegisteredForOpenGenericTypeUsedIfKeyIsNotFound()
        {
            PolicyList list = new PolicyList();
            FakePolicy policyForType = new FakePolicy();
            list.Set<IBuilderPolicy>(policyForType, typeof(IDummy<>));

            IBuilderPolicy result = list.Get<IBuilderPolicy>(new NamedTypeBuildKey<IDummy<object>>("name"));
            Assert.Same(policyForType, result);
        }

        [Fact]
        public void OuterPolicyDefaultOverridesInnerPolicyDefault()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy innerPolicy = new FakePolicy();
            FakePolicy outerPolicy = new FakePolicy();
            innerList.SetDefault(innerPolicy);
            outerList.SetDefault(outerPolicy);

            IPolicyList containingPolicyList;
            FakePolicy result = outerList.Get<FakePolicy>(typeof(object), out containingPolicyList);

            Assert.Same(outerPolicy, result);
            Assert.Same(outerList, containingPolicyList);
        }

        [Fact]
        public void OuterPolicyOverridesInnerPolicy()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy innerPolicy = new FakePolicy();
            FakePolicy outerPolicy = new FakePolicy();
            innerList.Set(innerPolicy, typeof(object));
            outerList.Set(outerPolicy, typeof(object));

            IPolicyList containingPolicyList;
            FakePolicy result = outerList.Get<FakePolicy>(typeof(object), out containingPolicyList);

            Assert.Same(outerPolicy, result);
            Assert.Same(outerList, containingPolicyList);
        }

        [Fact]
        public void SetOverwritesExistingPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy1 = new FakePolicy();
            FakePolicy policy2 = new FakePolicy();
            list.Set<IBuilderPolicy>(policy1, typeof(string));
            list.Set<IBuilderPolicy>(policy2, typeof(string));

            IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(string));

            Assert.Same(policy2, result);
        }

        [Fact]
        public void SpecificGenericPolicyComesBeforeGenericPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy genericPolicy = new FakePolicy();
            FakePolicy specificPolicy = new FakePolicy();
            list.Set(genericPolicy, typeof(IDummy<>));
            list.Set(specificPolicy, typeof(IDummy<int>));

            FakePolicy result = list.Get<FakePolicy>(typeof(IDummy<int>));

            Assert.Same(specificPolicy, result);
        }

        [Fact]
        public void SpecificInnerPolicyOverridesDefaultOuterPolicy()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy innerPolicy = new FakePolicy();
            FakePolicy outerPolicy = new FakePolicy();
            innerList.Set(innerPolicy, typeof(object));
            outerList.SetDefault(outerPolicy);

            IPolicyList containingPolicyList;
            FakePolicy result = outerList.Get<FakePolicy>(typeof(object), out containingPolicyList);

            Assert.Same(innerPolicy, result);
            Assert.Same(innerList, containingPolicyList);
        }

        [Fact]
        public void SpecificPolicyOverridesDefaultPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy defaultPolicy = new FakePolicy();
            FakePolicy specificPolicy = new FakePolicy();
            list.Set<IBuilderPolicy>(specificPolicy, typeof(object));
            list.SetDefault<IBuilderPolicy>(defaultPolicy);

            IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(object));

            Assert.Same(specificPolicy, result);
        }

        [Fact]
        public void WillAskInnerPolicyListWhenOuterHasNoAnswer()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy policy = new FakePolicy();
            innerList.Set(policy, typeof(object));

            IPolicyList containingPolicies;
            FakePolicy result = outerList.Get<FakePolicy>(typeof(object), out containingPolicies);

            Assert.Same(policy, result);
            Assert.Same(innerList, containingPolicies);
        }

        [Fact]
        public void WillUseInnerDefaultPolicyWhenOuterHasNoAnswer()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy policy = new FakePolicy();
            innerList.SetDefault(policy);

            IPolicyList containingPolicyList;
            FakePolicy result = outerList.Get<FakePolicy>(typeof(object), out containingPolicyList);

            Assert.Same(policy, result);
            Assert.Same(innerList, containingPolicyList);
        }

        private class FakePolicy : IBuilderPolicy { }

        private interface IDummy<T> { }
    }
}
