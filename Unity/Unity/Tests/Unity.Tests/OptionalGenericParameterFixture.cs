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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Tests
{
    /// <summary>
    /// Tests that use the GenericParameter class to ensure that
    /// generic object injection works.
    /// </summary>
    [TestClass]
    public class OptionalGenericParameterFixture
    {
        [TestMethod]
        public void CanCallConstructorTakingGenericParameterWithResolvableOptional()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ClassWithOneGenericParameter<>),
                    new InjectionConstructor(new OptionalGenericParameter("T")));

            Account a = new Account();
            container.RegisterInstance<Account>(a);

            ClassWithOneGenericParameter<Account> result = container.Resolve<ClassWithOneGenericParameter<Account>>();

            Assert.AreSame(a, result.InjectedValue);
        }

        [TestMethod]
        public void CanCallConstructorTakingGenericParameterWithNonResolvableOptional()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ClassWithOneGenericParameter<>),
                    new InjectionConstructor(new OptionalGenericParameter("T")));

            var result = container.Resolve<ClassWithOneGenericParameter<IComparable>>();

            Assert.IsNull(result.InjectedValue);
        }

        [TestMethod]
        public void CanConfiguredNamedResolutionOfOptionalGenericParameter()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ClassWithOneGenericParameter<>),
                    new InjectionConstructor(new OptionalGenericParameter("T", "named")));

            Account a = new Account();
            container.RegisterInstance<Account>(a);
            Account named = new Account();
            container.RegisterInstance<Account>("named", named);


            ClassWithOneGenericParameter<Account> result = container.Resolve<ClassWithOneGenericParameter<Account>>();
            Assert.AreSame(named, result.InjectedValue);
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
    }
}
