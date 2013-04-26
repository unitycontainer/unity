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
using Microsoft.Practices.Unity.TestSupport;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.Tests
{
    /// <summary>
    /// Tests that use the GenericParameter class to ensure that
    /// generic object injection works.
    /// </summary>
    [TestClass]
    public class GenericParameterFixture
    {
        [TestMethod]
        public void CanCallNonGenericConstructorOnOpenGenericType()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ClassWithOneGenericParameter<>),
                        new InjectionConstructor("Fiddle", new InjectionParameter<object>("someValue")));

            ClassWithOneGenericParameter<User> result = container.Resolve<ClassWithOneGenericParameter<User>>();

            Assert.IsNull(result.InjectedValue);
        }

        [TestMethod]
        public void CanCallConstructorTakingGenericParameter()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ClassWithOneGenericParameter<>),
                    new InjectionConstructor(new GenericParameter("T")));

            Account a = new Account();
            container.RegisterInstance<Account>(a);

            ClassWithOneGenericParameter<Account> result = container.Resolve<ClassWithOneGenericParameter<Account>>();
            Assert.AreSame(a, result.InjectedValue);
        }

        [TestMethod]
        public void CanConfiguredNamedResolutionOfGenericParameter()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ClassWithOneGenericParameter<>),
                    new InjectionConstructor(new GenericParameter("T", "named")));

            Account a = new Account();
            container.RegisterInstance<Account>(a);
            Account named = new Account();
            container.RegisterInstance<Account>("named", named);


            ClassWithOneGenericParameter<Account> result = container.Resolve<ClassWithOneGenericParameter<Account>>();
            Assert.AreSame(named, result.InjectedValue);
        }

        [TestMethod]
        public void ResolvingOpenGenericWithConstructorParameterAmbiguityThrows()
        {
            var container = new UnityContainer();
            container.RegisterType(
                typeof(GenericTypeWithMultipleGenericTypeParameters<,>),
                new InjectionConstructor(new GenericParameter("T", "instance")));
            container.RegisterInstance("instance", "the string");

            AssertExtensions.AssertException<ResolutionFailedException>(
                () => container.Resolve<GenericTypeWithMultipleGenericTypeParameters<string, string>>(),
                e => { });
        }

        [TestMethod]
        public void ResolvingOpenGenericWithMethodAmbiguityThrows()
        {
            var container = new UnityContainer();
            container.RegisterType(
                typeof(GenericTypeWithMultipleGenericTypeParameters<,>),
                new InjectionMethod("Set", new GenericParameter("T", "instance")));
            container.RegisterInstance("instance", "the string");

            //// equivalent to doing the following, which would be rejected by the compiler
            //new GenericTypeWithMultipleGenericTypeParameters<string, string>().Set(container.Resolve<string>("instance"));

            AssertExtensions.AssertException<ResolutionFailedException>(
                () => container.Resolve<GenericTypeWithMultipleGenericTypeParameters<string, string>>(),
                e => { });
        }

        // Our various test objects
        public class ClassWithOneGenericParameter<T>
        {
            public T InjectedValue;

            public ClassWithOneGenericParameter(string s, object o)
            {
            }

            public ClassWithOneGenericParameter(T injectedValue)
            {
                InjectedValue = injectedValue;
            }
        }

        public class GenericTypeWithMultipleGenericTypeParameters<T, U>
        {
            private T theT;
            private U theU;
            public string value;

            [InjectionConstructor]
            public GenericTypeWithMultipleGenericTypeParameters()
            {
            }

            public GenericTypeWithMultipleGenericTypeParameters(T theT)
            {
                this.theT = theT;
            }

            public GenericTypeWithMultipleGenericTypeParameters(U theU)
            {
                this.theU = theU;
            }

            public void Set(T theT)
            {
                this.theT = theT;
            }

            public void Set(U theU)
            {
                this.theU = theU;
            }

            public void SetAlt(T theT)
            {
                this.theT = theT;
            }

            public void SetAlt(string value)
            {
                this.value = value;
            }
        }
    }
}
