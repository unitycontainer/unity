// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.TestSupport;
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
     
    public class MethodInjectionFixture
    {
        [Fact]
        public void CanInjectMethodReturningVoid()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(GuineaPig),
                    new InjectionMethod("Inject2", "Hello"));

            GuineaPig pig = container.Resolve<GuineaPig>();

            Assert.Equal("Hello", pig.StringValue);
        }

        [Fact]
        public void CanInjectMethodReturningInt()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(GuineaPig),
                        new InjectionMethod("Inject3", 17));

            GuineaPig pig = container.Resolve<GuineaPig>();

            Assert.Equal(17, pig.IntValue);
        }

        [Fact]
        public void CanConfigureMultipleMethods()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<GuineaPig>(
                    new InjectionMethod("Inject3", 37),
                    new InjectionMethod("Inject2", "Hi there"));

            GuineaPig pig = container.Resolve<GuineaPig>();

            Assert.Equal(37, pig.IntValue);
            Assert.Equal("Hi there", pig.StringValue);
        }

        [Fact]
        public void StaticMethodsShouldNotBeInjected()
        {
            IUnityContainer container = new UnityContainer();

            GuineaPig pig = container.Resolve<GuineaPig>();
            Assert.False(GuineaPig.StaticMethodWasCalled);
        }

        [Fact]
        public void ContainerThrowsWhenConfiguringStaticMethodForInjection()
        {
            AssertExtensions.AssertException<InvalidOperationException>(() =>
                {
                    IUnityContainer container = new UnityContainer()
                        .RegisterType<GuineaPig>(
                               new InjectionMethod("ShouldntBeCalled"));
                });
        }

        public class GuineaPig
        {
            public int IntValue;
            public string StringValue;
            public static bool StaticMethodWasCalled = false;

            public void Inject1()
            {
            }

            public void Inject2(string stringValue)
            {
                this.StringValue = stringValue;
            }

            public int Inject3(int intValue)
            {
                IntValue = intValue;
                return intValue * 2;
            }

            [InjectionMethod]
            public static void ShouldntBeCalled()
            {
                StaticMethodWasCalled = true;
            }
        }
    }
}
