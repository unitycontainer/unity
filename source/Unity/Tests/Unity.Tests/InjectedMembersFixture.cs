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
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Unity.Tests
{
    [TestClass]
    public class InjectedMembersFixture
    {
        [TestMethod]
        public void CanConfigureContainerToCallDefaultConstructor()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<GuineaPig>(new InjectionConstructor());

            GuineaPig pig = container.Resolve<GuineaPig>();
            Assert.IsTrue(pig.DefaultConstructorCalled);
        }

        [TestMethod]
        public void CanConfigureContainerToCallConstructorWithValues()
        {
            int expectedInt = 37;
            string expectedString = "Hey there";
            double expectedDouble = Math.PI;

            IUnityContainer container = new UnityContainer()
                .RegisterType<GuineaPig>(
                            new InjectionConstructor(expectedInt, expectedString, expectedDouble));

            GuineaPig pig = container.Resolve<GuineaPig>();
            Assert.IsTrue(pig.ThreeArgumentConstructorCalled);
            Assert.AreEqual(expectedInt, pig.I);
            Assert.AreEqual(expectedDouble, pig.D);
            Assert.AreEqual(expectedString, pig.S);
        }

        [TestMethod]
        public void CanConfigureContainerToInjectProperty()
        {
            object expectedObject = new object();

            IUnityContainer container = new UnityContainer()
                .RegisterInstance<object>(expectedObject)
                .RegisterType<GuineaPig>(
                        new InjectionConstructor(),
                        new InjectionProperty("ObjectProperty"));

            GuineaPig pig = container.Resolve<GuineaPig>();
            Assert.IsTrue(pig.DefaultConstructorCalled);
            Assert.AreSame(expectedObject, pig.ObjectProperty);
        }

        [TestMethod]
        public void CanConfigureContainerToInjectPropertyWithValue()
        {
            int expectedInt = 82;

            IUnityContainer container = new UnityContainer()
                .RegisterType<GuineaPig>(
                        new InjectionConstructor(),
                        new InjectionProperty("IntProperty", expectedInt));

            GuineaPig pig = container.Resolve<GuineaPig>();

            Assert.IsTrue(pig.DefaultConstructorCalled);
            Assert.AreEqual(expectedInt, pig.IntProperty);
        }

        [TestMethod]
        public void CanConfigureInjectionByNameWithoutUsingGenerics()
        {
            object expectedObjectZero = new object();
            object expectedObjectOne = new object();

            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(GuineaPig), "one",
                    new InjectionConstructor(expectedObjectOne),
                    new InjectionProperty("IntProperty", 35))
                .RegisterType(typeof(GuineaPig),
                    new InjectionConstructor(),
                    new InjectionProperty("ObjectProperty", new ResolvedParameter(typeof(object), "zero")))
                .RegisterInstance<object>("zero", expectedObjectZero);

            GuineaPig pigZero = container.Resolve<GuineaPig>();
            GuineaPig pigOne = container.Resolve<GuineaPig>("one");

            Assert.IsTrue(pigZero.DefaultConstructorCalled);
            Assert.AreSame(expectedObjectZero, pigZero.ObjectProperty);
            Assert.IsTrue(pigOne.OneArgumentConstructorCalled);
            Assert.AreSame(expectedObjectOne, pigOne.ObjectProperty);
            Assert.AreEqual(35, pigOne.IntProperty);
        }

        [TestMethod]
        public void CanConfigureInjectionWithGenericProperty()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterInstance<int>(35)
                .RegisterType(typeof(GenericGuineaPig<>),
                    new InjectionProperty("GenericProperty"));

            GenericGuineaPig<int> pig = container.Resolve<GenericGuineaPig<int>>();

            Assert.AreEqual(35, pig.GenericProperty);
        }

        [TestMethod]
        public void CanConfigureContainerToDoMethodInjection()
        {
            string expectedString = "expected string";

            IUnityContainer container = new UnityContainer()
                .RegisterType<GuineaPig>(
                        new InjectionConstructor(),
                        new InjectionMethod("InjectMeHerePlease", expectedString));

            GuineaPig pig = container.Resolve<GuineaPig>();

            Assert.IsTrue(pig.DefaultConstructorCalled);
            Assert.AreEqual(expectedString, pig.S);
        }

        [TestMethod]
        public void ConfiguringInjectionAfterResolvingTakesEffect()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<GuineaPig>(new InjectionConstructor());

            container.Resolve<GuineaPig>();

            container.RegisterType<GuineaPig>(
                new InjectionConstructor(new InjectionParameter(typeof(object), "someValue")));

            GuineaPig pig2 = container.Resolve<GuineaPig>();

            Assert.AreEqual("someValue", pig2.ObjectProperty.ToString());
        }

        [TestMethod]
        public void ConfiguringInjectionConstructorThatDoesNotExistThrows()
        {
            IUnityContainer container = new UnityContainer();

            AssertExtensions.AssertException<InvalidOperationException>(() =>
                {
                    container.RegisterType<GuineaPig>(
                        new InjectionConstructor(typeof(string), typeof(string)));
                });
        }

        [TestMethod]
        public void RegisterTypeThrowsIfTypeIsNull()
        {
            IUnityContainer container = new UnityContainer();

            AssertExtensions.AssertException<ArgumentNullException>(() =>
                {
                    container.RegisterType(null);
                });
        }

        public class GuineaPig
        {
            public bool DefaultConstructorCalled = false;
            public bool OneArgumentConstructorCalled = false;
            public bool ThreeArgumentConstructorCalled = false;

            public object O;
            public int I;
            public string S;
            public double D;

            public GuineaPig()
            {
                DefaultConstructorCalled = true;
            }

            public GuineaPig(object o)
            {
                OneArgumentConstructorCalled = true;
                O = o;
            }

            public GuineaPig(int i, string s, double d)
            {
                ThreeArgumentConstructorCalled = true;
                I = i;
                S = s;
                D = d;
            }

            public int IntProperty
            {
                get { return I; }
                set { I = value; }
            }

            public object ObjectProperty
            {
                get { return O; }
                set { O = value; }
            }

            public void InjectMeHerePlease(string s)
            {
                S = s;
            }
        }

        public class GenericGuineaPig<T>
        {
            public T G;

            public T GenericProperty
            {
                get { return G; }
                set { G = value; }
            }
        }
    }
}
