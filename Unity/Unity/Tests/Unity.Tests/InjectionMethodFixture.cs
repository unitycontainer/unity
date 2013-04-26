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
using Microsoft.Practices.Unity.TestSupport;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using System.Reflection;

namespace Microsoft.Practices.Unity.Tests
{
    [TestClass]
    public class InjectionMethodFixture
    {
        [TestMethod]
        public void QualifyingInjectionMethodCanBeConfiguredAndIsCalled()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<LegalInjectionMethod>(
                        new InjectionMethod("InjectMe"));

            LegalInjectionMethod result = container.Resolve<LegalInjectionMethod>();
            Assert.IsTrue(result.WasInjected);
        }

        [TestMethod]       
        public void CannotConfigureGenericInjectionMethod()
        {
            AssertExtensions.AssertException<InvalidOperationException>(() =>
                {
                    new UnityContainer()
                        .RegisterType<OpenGenericInjectionMethod>(
                        new InjectionMethod("InjectMe"));
                });
        }

        [TestMethod]
        public void CannotConfigureMethodWithOutParams()
        {
            AssertExtensions.AssertException<InvalidOperationException>(() =>
                {
                    new UnityContainer().RegisterType<OutParams>(
                        new InjectionMethod("InjectMe", 12));
                });
        }

        [TestMethod]
        public void CannotConfigureMethodWithRefParams()
        {
            AssertExtensions.AssertException<InvalidOperationException>(() =>
                {
                    new UnityContainer()
                        .RegisterType<RefParams>(
                        new InjectionMethod("InjectMe", 15));
                });
        }

        [TestMethod]
        public void CanInvokeInheritedMethod()
        {
            IUnityContainer container = new UnityContainer()
                          .RegisterType<InheritedClass>(
                                  new InjectionMethod("InjectMe"));

            InheritedClass result = container.Resolve<InheritedClass>();
            Assert.IsTrue(result.WasInjected);
        }

        public class LegalInjectionMethod
        {
            public bool WasInjected = false;

            public void InjectMe()
            {
                WasInjected = true;
            }
        }

        public class OpenGenericInjectionMethod
        {
            public void InjectMe<T>()
            {

            }
        }

        public class OutParams
        {
            public void InjectMe(out int x)
            {
                x = 2;
            }
        }

        public class RefParams
        {
            public void InjectMe(ref int x)
            {
                x *= 2;
            }
        }

        public class InheritedClass : LegalInjectionMethod
        {

        }
    }
}
