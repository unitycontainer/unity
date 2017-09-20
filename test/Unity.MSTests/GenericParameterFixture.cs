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
using Unity;

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
                .RegisterType(typeof (ClassWithOneGenericParameter<>),
                        new InjectionConstructor("Fiddle", new InjectionParameter<object>("foo")));

            ClassWithOneGenericParameter<User> result = container.Resolve<ClassWithOneGenericParameter<User>>();

            Assert.IsNull(result.InjectedValue);
        }

        [TestMethod]
        public void CanCallConstructorTakingGenericParameter()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof (ClassWithOneGenericParameter<>),
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
