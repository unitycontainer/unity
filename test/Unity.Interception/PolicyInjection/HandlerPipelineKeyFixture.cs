// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Tests.PolicyInjection
{
    [TestClass]
    public class HandlerPipelineKeyFixture
    {
        [TestMethod]
        public void KeysForSameMethodAreEqual()
        {
            var key1 = HandlerPipelineKey.ForMethod(StaticReflection.GetMethodInfo<Base>(b => b.Method1()));
            var key2 = HandlerPipelineKey.ForMethod(StaticReflection.GetMethodInfo<Base>(b => b.Method1()));

            Assert.AreEqual(key1, key2);
        }

        [TestMethod]
        public void KeysForSameMethodReflectedFromDifferentTypesAreEqual()
        {
            var key1 = HandlerPipelineKey.ForMethod(StaticReflection.GetMethodInfo<Base>(b => b.Method2()));
            var key2 = HandlerPipelineKey.ForMethod(StaticReflection.GetMethodInfo<Derived>(b => b.Method2()));

            Assert.AreEqual(key1, key2);
        }

        [TestMethod]
        public void KeysForSameMethodReflectedFromDifferentTypesOnDifferentModulesAreEqual()
        {
            var key1 = HandlerPipelineKey.ForMethod(StaticReflection.GetMethodInfo<object>(o => o.ToString()));
            var key2 = HandlerPipelineKey.ForMethod(StaticReflection.GetMethodInfo<Derived>(b => b.ToString()));

            Assert.AreEqual(key1, key2);
        }

        [TestMethod]
        public void KeysForDifferentMethodsAreNotEqual()
        {
            var key1 = HandlerPipelineKey.ForMethod(StaticReflection.GetMethodInfo<Base>(b => b.Method1()));
            var key2 = HandlerPipelineKey.ForMethod(StaticReflection.GetMethodInfo<Base>(b => b.Method2()));

            Assert.AreNotEqual(key1, key2);
        }

        [TestMethod]
        public void KeysForOverridenMethodReflectedFromDifferentTypesAreNotEqual()
        {
            // using plain reflection - lambdas get optimized so we cannot get the override through them
            var key1 = HandlerPipelineKey.ForMethod(typeof(Base).GetMethod("Method1"));
            var key2 = HandlerPipelineKey.ForMethod(typeof(Derived).GetMethod("Method1"));

            Assert.AreNotEqual(key1, key2);
        }

        public class Base
        {
            public virtual void Method1() { }
            public virtual void Method2() { }
            public override string ToString()
            {
                return base.ToString();
            }
        }

        public class Derived : Base
        {
            public override void Method1()
            {
                base.Method1();
            }
        }
    }
}
