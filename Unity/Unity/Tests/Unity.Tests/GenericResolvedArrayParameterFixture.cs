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
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Tests
{
    [TestClass]
    public class GenericResolvedArrayParameterFixture
    {
        [TestMethod]
        public void MatchesArrayOfGenericTypeOnly()
        {
            InjectionParameterValue parameterValue = new GenericResolvedArrayParameter("T");

            Type genericTypeT
                = this.GetType().GetMethod("GetT", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetGenericArguments()[0];
            Type genericTypeU
                = this.GetType().GetMethod("GetU", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetGenericArguments()[0];

            Assert.IsFalse(parameterValue.MatchesType(genericTypeT));
            Assert.IsFalse(parameterValue.MatchesType(genericTypeU));
            Assert.IsFalse(parameterValue.MatchesType(typeof(object)));
            Assert.IsTrue(parameterValue.MatchesType(genericTypeT.MakeArrayType(1)));
            Assert.IsFalse(parameterValue.MatchesType(genericTypeT.MakeArrayType(2)));
            Assert.IsFalse(parameterValue.MatchesType(genericTypeU.MakeArrayType(1)));
            Assert.IsFalse(parameterValue.MatchesType(typeof(object[])));
        }

        [TestMethod]
        public void CanCallConstructorTakingGenericParameterArray()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(
                    typeof(ClassWithOneArrayGenericParameter<>),
                    new InjectionConstructor(new GenericParameter("T[]")));

            Account a0 = new Account();
            container.RegisterInstance<Account>("a0", a0);
            Account a1 = new Account();
            container.RegisterInstance<Account>("a1", a1);
            Account a2 = new Account();
            container.RegisterInstance<Account>(a2);

            ClassWithOneArrayGenericParameter<Account> result
                = container.Resolve<ClassWithOneArrayGenericParameter<Account>>();
            Assert.IsFalse(result.DefaultConstructorCalled);
            Assert.AreEqual(2, result.InjectedValue.Length);
            Assert.AreSame(a0, result.InjectedValue[0]);
            Assert.AreSame(a1, result.InjectedValue[1]);
        }

        [TestMethod]
        public void CanCallConstructorTakingGenericParameterArrayWithValues()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(
                    typeof(ClassWithOneArrayGenericParameter<>),
                    new InjectionConstructor(
                        new GenericResolvedArrayParameter(
                            "T",
                            new GenericParameter("T", "a2"),
                            new GenericParameter("T", "a1"))));

            Account a0 = new Account();
            container.RegisterInstance<Account>("a0", a0);
            Account a1 = new Account();
            container.RegisterInstance<Account>("a1", a1);
            Account a2 = new Account();
            container.RegisterInstance<Account>("a2", a2);

            ClassWithOneArrayGenericParameter<Account> result
                = container.Resolve<ClassWithOneArrayGenericParameter<Account>>();
            Assert.IsFalse(result.DefaultConstructorCalled);
            Assert.AreEqual(2, result.InjectedValue.Length);
            Assert.AreSame(a2, result.InjectedValue[0]);
            Assert.AreSame(a1, result.InjectedValue[1]);
        }

        [TestMethod]
        public void CanSetPropertyWithGenericParameterArrayType()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ClassWithOneArrayGenericParameter<>),
                                       new InjectionConstructor(),
                                       new InjectionProperty("InjectedValue", new GenericParameter("T()")));

            Account a0 = new Account();
            container.RegisterInstance<Account>("a1", a0);
            Account a1 = new Account();
            container.RegisterInstance<Account>("a2", a1);
            Account a2 = new Account();
            container.RegisterInstance<Account>(a2);

            ClassWithOneArrayGenericParameter<Account> result
                = container.Resolve<ClassWithOneArrayGenericParameter<Account>>();
            Assert.IsTrue(result.DefaultConstructorCalled);
            Assert.AreEqual(2, result.InjectedValue.Length);
            Assert.AreSame(a0, result.InjectedValue[0]);
            Assert.AreSame(a1, result.InjectedValue[1]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AppropriateExceptionIsThrownWhenNoMatchingConstructorCanBeFound()
        {
            new UnityContainer()
                .RegisterType(typeof(ClassWithOneGenericParameter<>),
                    new InjectionConstructor(new GenericResolvedArrayParameter("T")));
        }

        private void GetT<T>() { }
        private void GetU<U>() { }

        public class ClassWithOneArrayGenericParameter<T>
        {
            public T[] injectedValue;
            public readonly bool DefaultConstructorCalled;

            public ClassWithOneArrayGenericParameter()
            {
                DefaultConstructorCalled = true;
            }

            public ClassWithOneArrayGenericParameter(T[] injectedValue)
            {
                DefaultConstructorCalled = false;

                this.injectedValue = injectedValue;
            }

            public T[] InjectedValue
            {
                get { return this.injectedValue; }
                set { this.injectedValue = value; }
            }
        }

        public class ClassWithOneGenericParameter<T>
        {
        }
    }
}
