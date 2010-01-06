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
	public class PolicyListTest
	{
		[TestMethod]
		public void CanAddMultiplePoliciesToBagAndRetrieveThem()
		{
			PolicyList list = new PolicyList();
			FakePolicy policy1 = new FakePolicy();
			FakePolicy policy2 = new FakePolicy();

			list.Set<IBuilderPolicy>(policy1, "1");
			list.Set<IBuilderPolicy>(policy2, "2");

			Assert.AreSame(policy1, list.Get<IBuilderPolicy>("1"));
			Assert.AreSame(policy2, list.Get<IBuilderPolicy>("2"));
		}

		[TestMethod]
		public void CanAddPolicyToBagAndRetrieveIt()
		{
			PolicyList list = new PolicyList();
			FakePolicy policy = new FakePolicy();
			list.Set<IBuilderPolicy>(policy, typeof(object));

			IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(object));

			Assert.AreSame(policy, result);
		}

		[TestMethod]
		public void CanClearAllPolicies()
		{
			PolicyList list = new PolicyList();
			list.Set<IBuilderPolicy>(new FakePolicy(), "1");
			list.Set<IBuilderPolicy>(new FakePolicy(), "2");

			list.ClearAll();

			Assert.AreEqual(0, list.Count);
		}

		[TestMethod]
		public void CanClearDefaultPolicy()
		{
			PolicyList list = new PolicyList();
			FakePolicy defaultPolicy = new FakePolicy();
			list.SetDefault<IBuilderPolicy>(defaultPolicy);

			list.ClearDefault<IBuilderPolicy>();

			IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(object));
			Assert.IsNull(result);
		}

		[TestMethod]
		public void CanClearPolicy()
		{
			PolicyList list = new PolicyList();
			FakePolicy policy = new FakePolicy();

			list.Set<IBuilderPolicy>(policy, typeof(string));
			list.Clear<IBuilderPolicy>(typeof(string));

			Assert.IsNull(list.Get<IBuilderPolicy>(typeof(string)));
		}

		[TestMethod]
		public void CanGetLocalPolicy()
		{
			PolicyList innerList = new PolicyList();
			PolicyList outerList = new PolicyList(innerList);
			FakePolicy innerPolicy = new FakePolicy();
			innerList.Set(innerPolicy, typeof(object));

			FakePolicy result = outerList.Get<FakePolicy>(typeof(object), true);

			Assert.IsNull(result);
		}

		[TestMethod]
		public void CanRegisterGenericPolicyAndRetrieveWithSpecificGenericInstance()
		{
			PolicyList list = new PolicyList();
			FakePolicy policy = new FakePolicy();
			list.Set(policy, typeof(IFoo<>));

			FakePolicy result = list.Get<FakePolicy>(typeof(IFoo<int>));

			Assert.AreSame(policy, result);
		}

		[TestMethod]
		public void DefaultPolicyUsedWhenSpecificPolicyIsntAvailable()
		{
			PolicyList list = new PolicyList();
			FakePolicy defaultPolicy = new FakePolicy();
			list.SetDefault<IBuilderPolicy>(defaultPolicy);

			IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(object));

			Assert.AreSame(defaultPolicy, result);
		}

        [TestMethod]
        public void PolicyRegisteredForTypeIsUsedIfKeyIsNotFound()
        {
            PolicyList list = new PolicyList();
            FakePolicy policyForType = new FakePolicy();
            list.Set<IBuilderPolicy>(policyForType, typeof (object));

            IBuilderPolicy result = list.Get<IBuilderPolicy>(new NamedTypeBuildKey<object>("name"));

            Assert.AreSame(policyForType, result);
        }

        [TestMethod]
        public void PolicyForClosedGenericTypeOverridesPolicyForOpenType()
        {
            PolicyList list = new PolicyList();
            FakePolicy openTypePolicy = new FakePolicy();
            FakePolicy closedTypePolicy = new FakePolicy();
            list.Set<IBuilderPolicy>(openTypePolicy, typeof (IFoo<>));
            list.Set<IBuilderPolicy>(closedTypePolicy, typeof (IFoo<object>));

            IBuilderPolicy result = list.Get<IBuilderPolicy>(new NamedTypeBuildKey<IFoo<object>>("name"));
            Assert.AreSame(closedTypePolicy, result);

        }

        [TestMethod]
        public void PolicyRegisteredForOpenGenericTypeUsedIfKeyIsNotFound()
        {
            PolicyList list = new PolicyList();
            FakePolicy policyForType = new FakePolicy();
            list.Set<IBuilderPolicy>(policyForType, typeof(IFoo<>));

            IBuilderPolicy result = list.Get<IBuilderPolicy>(new NamedTypeBuildKey<IFoo<object>>("name"));
            Assert.AreSame(policyForType, result);
        }

		[TestMethod]
		public void OuterPolicyDefaultOverridesInnerPolicyDefault()
		{
			PolicyList innerList = new PolicyList();
			PolicyList outerList = new PolicyList(innerList);
			FakePolicy innerPolicy = new FakePolicy();
			FakePolicy outerPolicy = new FakePolicy();
			innerList.SetDefault(innerPolicy);
			outerList.SetDefault(outerPolicy);

			FakePolicy result = outerList.Get<FakePolicy>(typeof(object));

			Assert.AreSame(outerPolicy, result);
		}

		[TestMethod]
		public void OuterPolicyOverridesInnerPolicy()
		{
			PolicyList innerList = new PolicyList();
			PolicyList outerList = new PolicyList(innerList);
			FakePolicy innerPolicy = new FakePolicy();
			FakePolicy outerPolicy = new FakePolicy();
			innerList.Set(innerPolicy, typeof(object));
			outerList.Set(outerPolicy, typeof(object));

			FakePolicy result = outerList.Get<FakePolicy>(typeof(object));

			Assert.AreSame(outerPolicy, result);
		}

		[TestMethod]
		public void SetOverwritesExistingPolicy()
		{
			PolicyList list = new PolicyList();
			FakePolicy policy1 = new FakePolicy();
			FakePolicy policy2 = new FakePolicy();
			list.Set<IBuilderPolicy>(policy1, typeof(string));
			list.Set<IBuilderPolicy>(policy2, typeof(string));

			IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(string));

			Assert.AreSame(policy2, result);
		}

		[TestMethod]
		public void SpecificGenericPolicyComesBeforeGenericPolicy()
		{
			PolicyList list = new PolicyList();
			FakePolicy genericPolicy = new FakePolicy();
			FakePolicy specificPolicy = new FakePolicy();
			list.Set(genericPolicy, typeof(IFoo<>));
			list.Set(specificPolicy, typeof(IFoo<int>));

			FakePolicy result = list.Get<FakePolicy>(typeof(IFoo<int>));

			Assert.AreSame(specificPolicy, result);
		}

		[TestMethod]
		public void SpecificInnerPolicyOverridesDefaultOuterPolicy()
		{
			PolicyList innerList = new PolicyList();
			PolicyList outerList = new PolicyList(innerList);
			FakePolicy innerPolicy = new FakePolicy();
			FakePolicy outerPolicy = new FakePolicy();
			innerList.Set(innerPolicy, typeof(object));
			outerList.SetDefault(outerPolicy);

			FakePolicy result = outerList.Get<FakePolicy>(typeof(object));

			Assert.AreSame(innerPolicy, result);
		}

		[TestMethod]
		public void SpecificPolicyOverridesDefaultPolicy()
		{
			PolicyList list = new PolicyList();
			FakePolicy defaultPolicy = new FakePolicy();
			FakePolicy specificPolicy = new FakePolicy();
			list.Set<IBuilderPolicy>(specificPolicy, typeof(object));
			list.SetDefault<IBuilderPolicy>(defaultPolicy);

			IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(object));

			Assert.AreSame(specificPolicy, result);
		}

		[TestMethod]
		public void WillAskInnerPolicyListWhenOuterHasNoAnswer()
		{
			PolicyList innerList = new PolicyList();
			PolicyList outerList = new PolicyList(innerList);
			FakePolicy policy = new FakePolicy();
			innerList.Set(policy, typeof(object));

			FakePolicy result = outerList.Get<FakePolicy>(typeof(object));

			Assert.AreSame(policy, result);
		}

		[TestMethod]
		public void WillUseInnerDefaultPolicyWhenOuterHasNoAnswer()
		{
			PolicyList innerList = new PolicyList();
			PolicyList outerList = new PolicyList(innerList);
			FakePolicy policy = new FakePolicy();
			innerList.SetDefault(policy);

			FakePolicy result = outerList.Get<FakePolicy>(typeof(object));

			Assert.AreSame(policy, result);
		}

		class FakePolicy : IBuilderPolicy {}

		interface IFoo<T> {}
	}
}
