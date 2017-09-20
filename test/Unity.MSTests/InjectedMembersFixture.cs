// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Unity.Tests
{
#pragma warning disable CS0618 // Type or member is obsolete
    [TestClass]
    public class InjectedMembersFixture
    {
        [TestMethod]
        public void ArgumentNullExceptionThrownwhenConfigureInjectionForIsNull()
        {
            AssertHelper.ThrowsException<ArgumentNullException>(() =>
            new UnityContainer()
                                .Configure<InjectedMembers>()
                    .ConfigureInjectionFor(null).Container);
        }

        [TestMethod]
        public void InjectClassWithTwoConstructors()
        {
            int myInt = 37;
            string myStr = "Test";

            IUnityContainer container = new UnityContainer();

            //constructor without params
            container.Configure<InjectedMembers>().ConfigureInjectionFor<TestClass>(new InjectionConstructor());

            TestClass withOutCon = container.Resolve<TestClass>();
            Assert.IsFalse(withOutCon.StringConstructorCalled);
            Assert.IsFalse(withOutCon.IntConstructorCalled);
            //constructor with one param
            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor<TestClass>("First",
                    new InjectionConstructor(myInt));

            TestClass myTestClass = container.Resolve<TestClass>("First");

            Assert.IsFalse(myTestClass.StringConstructorCalled);
            Assert.IsTrue(myTestClass.IntConstructorCalled);

            //constructor with one param
            container.Configure<InjectedMembers>()
               .ConfigureInjectionFor<TestClass>("Second",
                   new InjectionConstructor(myStr));

            TestClass myTestClass1 = container.Resolve<TestClass>("Second");
            
            Assert.IsFalse(myTestClass1.IntConstructorCalled);
            Assert.IsTrue(myTestClass1.StringConstructorCalled);
        }

        [TestMethod]
        public void CanConfigureContainerToCallDefaultConstructor()
        {
            IUnityContainer container = new UnityContainer()

                .Configure<InjectedMembers>()
                    .ConfigureInjectionFor<GuineaPig>(
                        new InjectionConstructor())
                    .Container;

            GuineaPig pig = container.Resolve<GuineaPig>();
            Assert.IsTrue(pig.DefaultConstructorCalled);
        }

        [TestMethod]
        public void CanConfigureContainerToCallDefCostrToUseRegisterTypeInsteadofConfigureInjectionFor()
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
                .Configure<InjectedMembers>()
                    .ConfigureInjectionFor<GuineaPig>(
                            new InjectionConstructor(expectedInt, expectedString, expectedDouble))
                    .Container;

            GuineaPig pig = container.Resolve<GuineaPig>();
            Assert.IsTrue(pig.ThreeArgumentConstructorCalled);
            Assert.AreEqual(expectedInt, pig.I);
            Assert.AreEqual(expectedDouble, pig.D);
            Assert.AreEqual(expectedString, pig.S);
        }

        [TestMethod]
        public void CanConfigureContainerWithRegisterTypeToCallConstructorWithValues()
        {
            int expectedInt = 37;
            string expectedString = "Hey there";
            double expectedDouble = Math.PI;

            IUnityContainer container = new UnityContainer()
                .RegisterType<GuineaPig>(new InjectionConstructor(expectedInt, expectedString, expectedDouble));

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
                .Configure<InjectedMembers>()
                    .ConfigureInjectionFor<GuineaPig>(
                        new InjectionConstructor(),
                        new InjectionProperty("ObjectProperty"))
                    .Container;

            GuineaPig pig = container.Resolve<GuineaPig>();
            
            Assert.IsTrue(pig.DefaultConstructorCalled);
            Assert.AreSame(expectedObject, pig.ObjectProperty);
        }

        [TestMethod]
        public void CannotConfigureContainerToInjectAStaticProperty()
        {
            AssertHelper.ThrowsException<InvalidOperationException>(() => new UnityContainer()
                .RegisterInstance<string>("first", "first")
                .RegisterType<TestClassWithStaticProperties>(new InjectionProperty("PropertyToInject")));
        }

        [TestMethod]
        public void CannotConfigureContainerToInjectAStaticPropertyWhenUsingProperties()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterInstance<string>("first", "first");

            TestClassWithStaticPropertiesAnnotated o = container.Resolve<TestClassWithStaticPropertiesAnnotated>();
            Assert.IsNull(TestClassWithStaticPropertiesAnnotated.PropertyToInject);
        }

        [TestMethod]
        public void CanConfigureContainerToInjectPropertyWithRegisterType()
        {
            object expectedObject = new object();

            IUnityContainer container = new UnityContainer()
                .RegisterInstance<object>(expectedObject)
                .RegisterType<GuineaPig>(new InjectionConstructor(),
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
                .Configure<InjectedMembers>()
                    .ConfigureInjectionFor<GuineaPig>(
                        new InjectionConstructor(),
                        new InjectionProperty("IntProperty", expectedInt))
                    .Container;

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
                .Configure<InjectedMembers>()
                    .ConfigureInjectionFor(typeof(GuineaPig), "one",
                        new InjectionConstructor(expectedObjectOne),
                        new InjectionProperty("IntProperty", 35))
                    .ConfigureInjectionFor(typeof(GuineaPig),
                        new InjectionConstructor(),
                        new InjectionProperty("ObjectProperty", new ResolvedParameter(typeof(object), "zero")))
                    .Container
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
        public void CanConfigureContainerToDoMethodInjection()
        {
            string expectedString = "expected string";

            IUnityContainer container = new UnityContainer()
                                .Configure<InjectedMembers>()
                    .ConfigureInjectionFor<GuineaPig>(
                        new InjectionConstructor(),
                        new InjectionMethod("InjectMeHerePlease", expectedString))
                    .Container;

            GuineaPig pig = container.Resolve<GuineaPig>();

            Assert.IsTrue(pig.DefaultConstructorCalled);
            Assert.AreEqual(expectedString, pig.S);
        }

        [TestMethod]
        public void CanConfigureContainerToDoMethodInjectionWithRegisterType()
        {
            string expectedString = "expected string";

            IUnityContainer container = new UnityContainer()
                .RegisterType<GuineaPig>(new InjectionConstructor(),
                        new InjectionMethod("InjectMeHerePlease", expectedString));
            GuineaPig pig = container.Resolve<GuineaPig>();

            Assert.IsTrue(pig.DefaultConstructorCalled);
            Assert.AreEqual(expectedString, pig.S);
        }

        [TestMethod]
        public void CanConfigureMultipleMethodsusingRegisterType()
        {
            string expectedString = "expected string";
            int expectedValue = 10;
            IUnityContainer container = new UnityContainer()
                .RegisterType<GuineaPig>(new InjectionConstructor(),
                    new InjectionMethod("InjectMeHerePlease", expectedString),
                    new InjectionMethod("InjectMeHerePleaseOnceMore", expectedValue));

            GuineaPig pig = container.Resolve<GuineaPig>();

            Assert.AreEqual(expectedString, pig.S);
            Assert.AreEqual(expectedValue, pig.I);
        }

        public class TestClassWithStaticProperties
        {
            public static string PropertyToInject { get; set; }
        }

        public class TestClassWithStaticPropertiesAnnotated
        {
            [Dependency]
            public static string PropertyToInject { get; set; }
        }

        public class TestClass
        {
            public bool IntConstructorCalled = false;
            public bool StringConstructorCalled = false;
            
            public TestClass()
            {
            }
            
            public TestClass(int i)
            {
                IntConstructorCalled = true;
            }
            
            public TestClass(string s)
            {
                StringConstructorCalled = true;
            }
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

            public void InjectMeHerePleaseOnceMore(int i)
            {
                I = i;
            }
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
