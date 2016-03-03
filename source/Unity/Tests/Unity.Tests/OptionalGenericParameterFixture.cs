// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
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

namespace Unity.Tests
{
    /// <summary>
    /// Tests that use the GenericParameter class to ensure that
    /// generic object injection works.
    /// </summary>
     
    public class OptionalGenericParameterFixture
    {
        [Fact]
        public void CanCallConstructorTakingGenericParameterWithResolvableOptional()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ClassWithOneGenericParameter<>),
                    new InjectionConstructor(new OptionalGenericParameter("T")));

            Account a = new Account();
            container.RegisterInstance<Account>(a);

            ClassWithOneGenericParameter<Account> result = container.Resolve<ClassWithOneGenericParameter<Account>>();

            Assert.Same(a, result.InjectedValue);
        }

        [Fact]
        public void CanCallConstructorTakingGenericParameterWithNonResolvableOptional()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ClassWithOneGenericParameter<>),
                    new InjectionConstructor(new OptionalGenericParameter("T")));

            var result = container.Resolve<ClassWithOneGenericParameter<IComparable>>();

            Assert.Null(result.InjectedValue);
        }

        [Fact]
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
            Assert.Same(named, result.InjectedValue);
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
