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
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.Unity.Tests.TestDoubles;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Tests
{
    [TestClass]
    public class InjectionConstructorFixture
    {
        [TestMethod]
        public void InjectionConstructorInsertsChooserForDefaultConstructor()
        {
            InjectionConstructor ctor = new InjectionConstructor();
            TestingBuilderContext context = new TestingBuilderContext();
            context.BuildKey = typeof (GuineaPig);
            IPolicyList policies = context.PersistentPolicies;

            ctor.AddPolicies(typeof(GuineaPig), policies);

            IConstructorSelectorPolicy selector = policies.Get<IConstructorSelectorPolicy>(
                new NamedTypeBuildKey(typeof(GuineaPig)));

            SelectedConstructor selected = selector.SelectConstructor(context);
            Assert.AreEqual(typeof(GuineaPig).GetConstructor(new Type[0]), selected.Constructor);
            Assert.AreEqual(0, selected.GetParameterKeys().Length);
        }

        [TestMethod]
        public void InjectionConstructorInsertsChooserForConstructorWithParameters()
        {
            string expectedString = "Hello";
            int expectedInt = 12;

            InjectionConstructor ctor = new InjectionConstructor(expectedString, expectedInt);
            TestingBuilderContext context = new TestingBuilderContext();
            context.BuildKey = typeof (GuineaPig);
            IPolicyList policies = context.PersistentPolicies;

            ctor.AddPolicies(typeof(GuineaPig), policies);

            IConstructorSelectorPolicy selector = policies.Get<IConstructorSelectorPolicy>(
                new NamedTypeBuildKey(typeof(GuineaPig)));

            SelectedConstructor selected = selector.SelectConstructor(context);
            string[] keys = selected.GetParameterKeys();

            Assert.AreEqual(typeof(GuineaPig).GetConstructor(Sequence.Collect(typeof(string), typeof(int))), selected.Constructor);
            Assert.AreEqual(2, keys.Length);

            Assert.AreEqual(expectedString, (string)ResolveValue(policies, keys[0]));
            Assert.AreEqual(expectedInt, (int)ResolveValue(policies, keys[1]));
        }

        [TestMethod]
        public void InjectionConstructorSetsResolverForInterfaceToLookupInContainer()
        {
            InjectionConstructor ctor = new InjectionConstructor("Logger", typeof(ILogger));
            TestingBuilderContext context = new TestingBuilderContext();
            context.BuildKey = typeof (GuineaPig);
            IPolicyList policies = context.PersistentPolicies;

            ctor.AddPolicies(typeof(GuineaPig), policies);

            IConstructorSelectorPolicy selector = policies.Get<IConstructorSelectorPolicy>(
                new NamedTypeBuildKey(typeof(GuineaPig)));

            SelectedConstructor selected = selector.SelectConstructor(context);
            string[] keys = selected.GetParameterKeys();

            Assert.AreEqual(typeof(GuineaPig).GetConstructor(Sequence.Collect(typeof(string), typeof(ILogger))), selected.Constructor);
            Assert.AreEqual(2, keys.Length);

            IDependencyResolverPolicy policy =
                context.Policies.Get<IDependencyResolverPolicy>(keys[1]);
            Assert.IsTrue(policy is NamedTypeDependencyResolverPolicy);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InjectionConstructorThrowsIfNoMatchingConstructor()
        {
            InjectionConstructor ctor = new InjectionConstructor(typeof(double));
            TestingBuilderContext context = new TestingBuilderContext();

            ctor.AddPolicies(typeof(GuineaPig), context.PersistentPolicies);
        }

        private object ResolveValue(IPolicyList policies, string key)
        {
            IDependencyResolverPolicy resolver = policies.Get<IDependencyResolverPolicy>(key);
            return resolver.Resolve(null);
        }

        private class GuineaPig
        {
            public GuineaPig()
            {

            }

            public GuineaPig(int i)
            {

            }

            public GuineaPig(string s)
            {

            }

            public GuineaPig(int i, string s)
            {
                
            }

            public GuineaPig(string s, int i)
            {
                
            }

            public GuineaPig(string s, ILogger logger)
            {
                
            }
        }

    }
}
