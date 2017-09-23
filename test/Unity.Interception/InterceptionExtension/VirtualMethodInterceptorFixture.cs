// TODO: Verify
//// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
//using System;
//using System.Reflection;

//using Microsoft.Practices.Unity.InterceptionExtension;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Unity.InterceptionExtension;

//namespace Unity.Tests.InterceptionExtension
//{
//    [TestClass]
//    public class VirtualMethodInterceptorFixture : InterceptorFixtureBase
//    {
//        [TestMethod]
//        public virtual void RefParameterMethod1()
//        {
//            IUnityContainer container = this.GetContainer();

//            ParameterTestClass testClass = container.Resolve<ParameterTestClass>();
//            int reference = 5;
//            testClass.RefParameterMethod1(ref reference);

//            Assert.AreEqual(6, reference);
//            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
//        }

//        [TestMethod]
//        public virtual void RefParameterMethod2()
//        {
//            IUnityContainer container = this.GetContainer();

//            ParameterTestClass testClass = container.Resolve<ParameterTestClass>();
//            string reference = "merry";
//            testClass.RefParameterMethod2(ref reference);

//            Assert.AreEqual("merryxmas", reference);
//            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
//        }

//        [TestMethod]
//        public virtual void RefParameterMethod3()
//        {
//            IUnityContainer container = this.GetContainer();

//            ParameterTestClass testClass = container.Resolve<ParameterTestClass>();
//            ParameterTestClass.FooClass reference = new ParameterTestClass.FooClass();
//            ParameterTestClass.FooClass referenceOriginal = reference;
//            testClass.RefParameterMethod3(ref reference);

//            Assert.AreNotSame(referenceOriginal, reference);
//            Assert.IsNotNull(reference);
//            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
//        }

//        [TestMethod]
//        public virtual void RefParameterMethod4()
//        {
//            IUnityContainer container = this.GetContainer();

//            ParameterTestClass testClass = container.Resolve<ParameterTestClass>();
//            ParameterTestClass.FooStruct reference = new ParameterTestClass.FooStruct();
//            ParameterTestClass.FooStruct referenceOriginal = reference;
//            testClass.RefParameterMethod4(ref reference);

//            Assert.AreNotSame(referenceOriginal, reference);
//            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
//        }

//        [TestMethod]
//        public virtual void OutParameterMethod1()
//        {
//            IUnityContainer container = this.GetContainer();

//            ParameterTestClass testClass = container.Resolve<ParameterTestClass>();
//            int reference;
//            testClass.OutParameterMethod1(out reference);

//            Assert.AreEqual(5, reference);
//            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
//        }

//        [TestMethod]
//        public virtual void OutParameterMethod2()
//        {
//            IUnityContainer container = this.GetContainer();

//            ParameterTestClass testClass = container.Resolve<ParameterTestClass>();
//            string reference;
//            testClass.OutParameterMethod2(out reference);

//            Assert.AreEqual("merry", reference);
//            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
//        }

//        [TestMethod]
//        public virtual void OutParameterMethod3()
//        {
//            IUnityContainer container = this.GetContainer();

//            ParameterTestClass testClass = container.Resolve<ParameterTestClass>();
//            ParameterTestClass.FooClass reference;
//            testClass.OutParameterMethod3(out reference);

//            Assert.IsNotNull(reference);
//            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
//        }

//        [TestMethod]
//        public virtual void OutParameterMethod4()
//        {
//            IUnityContainer container = this.GetContainer();

//            ParameterTestClass testClass = container.Resolve<ParameterTestClass>();
//            ParameterTestClass.FooStruct reference = new ParameterTestClass.FooStruct();
//            ParameterTestClass.FooStruct referenceOriginal = reference;
//            testClass.OutParameterMethod4(out reference);

//            Assert.AreNotSame(referenceOriginal, reference);
//            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
//        }

//        [TestMethod]
//        [ExpectedException(typeof(ArgumentException))]
//        public virtual void SealedConcreteClassesCannotBeIntercepted()
//        {
//            IUnityContainer container = this.GetContainer();

//            container.Configure<Interception>()
//                .SetDefaultInterceptorFor<SealedTestClass>(new VirtualMethodInterceptor());
//        }

//        [TestMethod]
//        public virtual void InterceptionDoesNotCreateANewProxyEverytime()
//        {
//            IUnityContainer container = this.GetContainer();

//            IdentityTestClass testClass1 = container.Resolve<IdentityTestClass>();
//            IdentityTestClass testClass2 = container.Resolve<IdentityTestClass>();

//            Assert.IsNotNull(testClass1);
//            Assert.IsNotNull(testClass2);
//            Assert.AreSame(testClass1, testClass2);
//        }


//        private void TestClass_EventTest1(object sender, EventArgs e)
//        {
//        }

//        [TestMethod]
//        public virtual void TestBasicInterceptionMethods()
//        {
//            IUnityContainer container = this.GetContainer();

//            BasicInterceptionTestClass testClass = container.Resolve<BasicInterceptionTestClass>();
//            testClass.MethodTest1();

//            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
//        }

//        [TestMethod]
//        public virtual void TestBasicInterceptionIndexers()
//        {
//            IUnityContainer container = this.GetContainer();

//            BasicInterceptionTestClass testClass = container.Resolve<BasicInterceptionTestClass>();
//            testClass[5] = new object();
//            object o = testClass[3];

//            Assert.AreEqual(2, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
//        }

//        [TestMethod]
//        public virtual void TestBasicInterceptionProperties()
//        {
//            IUnityContainer container = this.GetContainer();

//            BasicInterceptionTestClass testClass = container.Resolve<BasicInterceptionTestClass>();
//            object o1 = testClass.PropertyTest1;
//            testClass.PropertyTest2 = new object();
//            testClass.PropertyTest3 = new object();
//            object o2 = testClass.PropertyTest3;
//            object o3 = testClass.PropertyTest4;

//            Assert.AreEqual(5, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
//        }

//        [TestMethod]
//        public virtual void TestAttributesInheritance()
//        {
//            IUnityContainer container = this.GetContainer();

//            AttributeTestClass testClass = container.Resolve<AttributeTestClass>();
//            Type interceptedType = testClass.GetType();

//            Assert.AreNotEqual(typeof(AttributeTestClass), interceptedType);
//            Assert.AreEqual(1, AttributeHelper.GetAttributes<InheritableTestAttribute>(interceptedType).Length);
//            Assert.AreEqual(0, AttributeHelper.GetAttributes<InheritableTestAttribute>(interceptedType.GetMethod("TestMethod1")).Length);
//            Assert.AreEqual(1, AttributeHelper.GetAttributes<InheritableTestAttribute>(interceptedType.GetMethod("TestMethod2")).Length);
//            Assert.AreEqual(1, AttributeHelper.GetAttributes<InheritableTestAttribute>(interceptedType.GetProperty("TestProperty")).Length);
//            Assert.AreEqual(1, AttributeHelper.GetAttributes<InheritableTestAttribute>(interceptedType.GetEvent("TestEvent")).Length);
//            Assert.AreEqual(1, AttributeHelper.GetAttributes<InheritableTestAttribute>(interceptedType.GetProperty("Item")).Length);
//        }

//        [TestMethod]
//        public virtual void TestAttributesMultiple() // bug 18951
//        {
//            IUnityContainer container = this.GetContainer();

//            AttributeTestClass testClass = container.Resolve<AttributeTestClass>();
//            Type interceptedType = testClass.GetType();

//            Assert.AreNotEqual(typeof(AttributeTestClass), interceptedType);
//            Assert.AreEqual(3, AttributeHelper.GetAttributes<MultipleTestAttribute>(interceptedType).Length);
//            Assert.AreEqual(3, AttributeHelper.GetAttributes<MultipleTestAttribute>(interceptedType.GetMethod("TestMethod1")).Length);
//            Assert.AreEqual(0, AttributeHelper.GetAttributes<MultipleTestAttribute>(interceptedType.GetMethod("TestMethod2")).Length);

//            // Note: Known bug here. The expected values should all be 3. Appears to be a bug in the CLR reflection API.
//            Assert.AreEqual(6, AttributeHelper.GetAttributes<MultipleTestAttribute>(interceptedType.GetProperty("TestProperty")).Length); //todo finds 6
//            Assert.AreEqual(6, AttributeHelper.GetAttributes<MultipleTestAttribute>(interceptedType.GetEvent("TestEvent")).Length); //todo finds 6
//            Assert.AreEqual(6, AttributeHelper.GetAttributes<MultipleTestAttribute>(interceptedType.GetProperty("Item")).Length); //todo finds 6
//        }

//        [TestMethod]
//        public virtual void TestAttributesWithValue()
//        {
//            IUnityContainer container = this.GetContainer();

//            AttributeTestClass testClass = container.Resolve<AttributeTestClass>();
//            Type interceptedType = testClass.GetType();

//            Assert.AreNotEqual(typeof(AttributeTestClass), interceptedType);
//            Assert.AreEqual(1, AttributeHelper.GetAttributes<TestAttribute>(interceptedType).Length);
//            Assert.AreEqual(1, AttributeHelper.GetAttributes<TestAttribute>(interceptedType.GetMethod("TestMethod1")).Length);
//            Assert.AreEqual(1, AttributeHelper.GetAttributes<TestAttribute>(interceptedType.GetMethod("TestMethod2")).Length);
//            Assert.AreEqual(1, AttributeHelper.GetAttributes<TestAttribute>(interceptedType.GetProperty("TestProperty")).Length);
//            Assert.AreEqual(1, AttributeHelper.GetAttributes<TestAttribute>(interceptedType.GetEvent("TestEvent")).Length);
//            Assert.AreEqual(1, AttributeHelper.GetAttributes<TestAttribute>(interceptedType.GetProperty("Item")).Length);
//            Assert.AreEqual(5, (AttributeHelper.GetAttributes<TestAttribute>(interceptedType.GetMethod("TestMethod2"))[0] as TestAttribute).MyProperty);
//            Assert.AreEqual(5, (AttributeHelper.GetAttributes<TestAttribute>(interceptedType.GetProperty("TestProperty"))[0] as TestAttribute).MyProperty);
//            Assert.AreEqual(5, (AttributeHelper.GetAttributes<TestAttribute>(interceptedType.GetEvent("TestEvent"))[0] as TestAttribute).MyProperty);
//            Assert.AreEqual(5, (AttributeHelper.GetAttributes<TestAttribute>(interceptedType.GetProperty("Item"))[0] as TestAttribute).MyProperty);
//        }

//        [TestMethod]
//        public virtual void TestReflection()
//        {
//            IUnityContainer container = this.GetContainer();

//            ReflectionTestClass testClass = container.Resolve<ReflectionTestClass>();
//            Type interceptedType = testClass.GetType();

//            Assert.AreNotEqual(typeof(ReflectionTestClass), interceptedType);
//            Assert.IsTrue(interceptedType.IsSubclassOf(typeof(ReflectionTestClass)));
//        }

//        [TestMethod]
//        public virtual void TestMultipleGenerics() //bug 18947
//        {
//            IUnityContainer container = this.GetContainer();

//            GenericTestClass<ReflectionTestClass> testClass1 = container.Resolve<GenericTestClass<ReflectionTestClass>>();
//            GenericTestClass<ReflectionTestClass1> testClass2 = container.Resolve<GenericTestClass<ReflectionTestClass1>>();
//            GenericTestClass<ReflectionTestClass2> testClass3 = container.Resolve<GenericTestClass<ReflectionTestClass2>>();

//            Assert.IsNotNull(testClass1);
//            Assert.IsNotNull(testClass2);
//            Assert.IsNotNull(testClass3);
//        }

//        [TestMethod]
//        public virtual void TestGenerics() //bug 18952
//        {
//            IUnityContainer container = this.GetContainer();
//            GenericTestClass<ReflectionTestClass> testClass = container.Resolve<GenericTestClass<ReflectionTestClass>>();
//            Type interceptedType = testClass.GetType();
//            Type genericInterceptedType = interceptedType.GetGenericTypeDefinition();
//            Assert.IsFalse(interceptedType.ContainsGenericParameters);
//            Assert.IsTrue(interceptedType.IsGenericType);
//            Assert.IsFalse(interceptedType.IsGenericTypeDefinition);

//            Assert.IsTrue(genericInterceptedType.ContainsGenericParameters);
//            Assert.IsTrue(genericInterceptedType.IsGenericType);
//            Assert.IsTrue(genericInterceptedType.IsGenericTypeDefinition);

//            //verify class generic arguments
//            Assert.AreEqual(1, interceptedType.GetGenericArguments().Length);
//            Assert.AreEqual(typeof(ReflectionTestClass), interceptedType.GetGenericArguments()[0]);
//            Assert.AreEqual("T", genericInterceptedType.GetGenericArguments()[0].Name);
//            Assert.AreEqual(GenericParameterAttributes.ReferenceTypeConstraint, genericInterceptedType.GetGenericArguments()[0].GenericParameterAttributes);

//            MethodInfo methodInfo;
//            Type[] genericArgs;
//            Type[] constraints;

//            //verify testMethod1 generic arguments
//            this.GetMethodInfo(interceptedType, "TestMethod1", out methodInfo, out genericArgs, out constraints);
//            Assert.AreEqual(1, genericArgs.Length);
//            Assert.AreEqual("T1", genericArgs[0].Name);
//            Assert.AreEqual(GenericParameterAttributes.None, genericArgs[0].GenericParameterAttributes);

//            //verify testMethod2 generic arguments
//            this.GetMethodInfo(interceptedType, "TestMethod2", out methodInfo, out genericArgs, out constraints);
//            Assert.AreEqual(1, genericArgs.Length);
//            Assert.AreEqual("T2", genericArgs[0].Name);
//            Assert.AreEqual(GenericParameterAttributes.NotNullableValueTypeConstraint | GenericParameterAttributes.DefaultConstructorConstraint,
//                genericArgs[0].GenericParameterAttributes);

//            //verify testMethod3 generic arguments
//            this.GetMethodInfo(interceptedType, "TestMethod3", out methodInfo, out genericArgs, out constraints);
//            Assert.AreEqual(1, genericArgs.Length);
//            Assert.AreEqual("T3", genericArgs[0].Name);
//            Assert.AreEqual(GenericParameterAttributes.ReferenceTypeConstraint, genericArgs[0].GenericParameterAttributes);

//            //verify testMethod4 generic arguments
//            this.GetMethodInfo(interceptedType, "TestMethod4", out methodInfo, out genericArgs, out constraints);
//            Assert.AreEqual(1, genericArgs.Length);
//            Assert.AreEqual("T4", genericArgs[0].Name);
//            Assert.AreEqual(GenericParameterAttributes.ReferenceTypeConstraint | GenericParameterAttributes.DefaultConstructorConstraint,
//                genericArgs[0].GenericParameterAttributes);

//            //verify testMethod5 generic arguments
//            this.GetMethodInfo(interceptedType, "TestMethod5", out methodInfo, out genericArgs, out constraints);
//            Assert.AreEqual(1, genericArgs.Length);
//            Assert.AreEqual(1, constraints.Length);
//            Assert.AreEqual("T5", genericArgs[0].Name);
//            Assert.AreEqual(GenericParameterAttributes.None, genericArgs[0].GenericParameterAttributes);
//            Assert.IsTrue(constraints[0].IsClass);
//            Assert.AreEqual(typeof(InjectionPolicy), constraints[0]);

//            //verify testMethod6 generic arguments
//            this.GetMethodInfo(interceptedType, "TestMethod6", out methodInfo, out genericArgs, out constraints);
//            Assert.AreEqual(1, genericArgs.Length);
//            Assert.AreEqual(1, constraints.Length);
//            Assert.AreEqual("T6", genericArgs[0].Name);
//            Assert.IsTrue(constraints[0].IsInterface);
//            Assert.AreEqual(typeof(IMatchingRule), constraints[0]);
//        }

//        [TestMethod]
//        public virtual void TestNewVirtualOverride() //Bug 18950
//        {
//            IUnityContainer container = this.GetContainer();

//            NewVirtualOverrideTestClass testClass = container.Resolve<NewVirtualOverrideTestClass>();

//            Assert.IsTrue(testClass.TestMethod1(), "override");
//            Assert.IsTrue(testClass.TestMethod2(), "new virtual");
//            Assert.IsTrue(testClass.TestMethod3(), "always true");
//            Assert.IsTrue(testClass.TestMethod4(), "abstract");
//            Assert.AreEqual(4, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
//        }

//        protected override IUnityContainer GetContainer()
//        {
//            IUnityContainer container = base.GetContainer();

//            container.RegisterType<IdentityTestClass>(new ContainerControlledLifetimeManager());

//            container.Configure<Interception>()
//                .SetDefaultInterceptorFor<ParameterTestClass>(new VirtualMethodInterceptor())
//                .SetDefaultInterceptorFor<BasicInterceptionTestClass>(new VirtualMethodInterceptor())
//                .SetDefaultInterceptorFor<AttributeTestClass>(new VirtualMethodInterceptor())
//                .SetDefaultInterceptorFor<ReflectionTestClass>(new VirtualMethodInterceptor())
//                .SetDefaultInterceptorFor(typeof(GenericTestClass<>), new VirtualMethodInterceptor())
//                .SetDefaultInterceptorFor<NewVirtualOverrideTestClass>(new VirtualMethodInterceptor());

//            return container;
//        }

//        private void GetMethodInfo(Type t, string methodName, out MethodInfo methodInfo, out Type[] genericArguments, out Type[] genericConstraints)
//        {
//            methodInfo = t.GetMethod(methodName);
//            genericArguments = methodInfo.GetGenericArguments();
//            genericConstraints = genericArguments[0].GetGenericParameterConstraints();
//        }

//        #region Test Classes

//        public abstract class NewVirtualOverrideTestClassBase
//        {
//            public virtual bool TestMethod1()
//            {
//                return false;
//            }

//            public virtual bool TestMethod2()
//            {
//                return false;
//            }

//            public virtual bool TestMethod3()
//            {
//                return true;
//            }

//            public abstract bool TestMethod4();
//        }

//        public class NewVirtualOverrideTestClass : NewVirtualOverrideTestClassBase
//        {
//            public override bool TestMethod1()
//            {
//                return true;
//            }

//            public new virtual bool TestMethod2()
//            {
//                return true;
//            }

//            public override bool TestMethod4()
//            {
//                return true;
//            }
//        }

//        public class GenericTestClass<T>
//            where T : class
//        {
//            public virtual void TestMethod1<T1>()
//            { }

//            public virtual void TestMethod2<T2>()
//                where T2 : struct
//            { }

//            public virtual void TestMethod3<T3>()
//                where T3 : class
//            { }

//            public virtual void TestMethod4<T4>()
//                where T4 : class, new()
//            { }

//            public virtual void TestMethod5<T5>()
//                where T5 : InjectionPolicy
//            { }

//            public virtual void TestMethod6<T6>()
//                where T6 : IMatchingRule
//            { }
//        }

//        public class ReflectionTestClass
//        {
//            public virtual void TestMethod() { }
//        }

//        public class ReflectionTestClass1
//        {
//            public virtual void TestMethod() { }
//        }

//        public class ReflectionTestClass2
//        {
//            public virtual void TestMethod() { }
//        }

//        [AttributeUsage(AttributeTargets.All)]
//        public class TestAttribute : Attribute
//        {
//            public int MyProperty { get; set; }
//        }

//        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
//        public class MultipleTestAttribute : Attribute
//        {
//            private string name;
//            public string Name
//            {
//                get { return this.name; }
//            }

//            public MultipleTestAttribute(string name)
//            {
//                this.name = name;
//            }
//        }

//        [AttributeUsage(AttributeTargets.All, Inherited = true)]
//        public class InheritableTestAttribute : Attribute
//        {
//        }

//        [MultipleTest("A")]
//        [MultipleTest("B")]
//        [MultipleTest("C")]
//        [Test]
//        [InheritableTest]
//        public class AttributeTestClass
//        {
//            [MultipleTest("AA")]
//            [MultipleTest("BB")]
//            [MultipleTest("CC")]
//            [Test]
//            public virtual void TestMethod1() { }

//            [Test(MyProperty = 5)]
//            [InheritableTest]
//            public virtual void TestMethod2() { }

//            [MultipleTest("AAA")]
//            [MultipleTest("BBB")]
//            [MultipleTest("CCC")]
//            [InheritableTest]
//            [Test(MyProperty = 5)]
//            public virtual event EventHandler TestEvent;

//            [MultipleTest("AAAA")]
//            [MultipleTest("BBBB")]
//            [MultipleTest("CCCC")]
//            [InheritableTest]
//            [Test(MyProperty = 5)]
//            public virtual int this[int index]
//            {
//                get { return 5; }
//                set { }
//            }

//            [MultipleTest("AAAAA")]
//            [MultipleTest("BBBBB")]
//            [MultipleTest("CCCCC")]
//            [InheritableTest]
//            [Test(MyProperty = 5)]
//            public virtual object TestProperty
//            {
//                get { return new object(); }
//                set { }
//            }
//        }

//        public class BasicInterceptionTestClass
//        {
//            public virtual event EventHandler EventTest1;

//            public void FireEventTest1()
//            {
//                EventTest1(this, EventArgs.Empty);
//            }

//            private object indexerTest1;
//            public virtual object this[int item]
//            {
//                get { return indexerTest1; }
//                set { indexerTest1 = value; }
//            }

//            public virtual object MethodTest1()
//            {
//                return new object();
//            }

//            public virtual object PropertyTest1
//            {
//                get { return new object(); }
//            }

//            public virtual object PropertyTest2
//            {
//                set { }
//            }

//            private object propertyTest3;
//            public virtual object PropertyTest3
//            {
//                get { return propertyTest3; }
//                set { propertyTest3 = value; }
//            }

//            private object propertyTest4;
//            public virtual object PropertyTest4
//            {
//                get { return propertyTest4; }
//                protected set { propertyTest4 = value; }
//            }
//        }

//        public class IdentityTestClass
//        {
//        }

//        public sealed class SealedTestClass
//        {
//        }

//        public abstract class AbstractTestClass
//        {
//        }

//        public class ParameterTestClass
//        {
//            public virtual void RefParameterMethod1(ref int reference)
//            {
//                reference++;
//            }

//            public virtual void RefParameterMethod2(ref string reference)
//            {
//                reference = reference + "xmas";
//            }

//            public class FooClass { }

//            public virtual void RefParameterMethod3(ref FooClass reference)
//            {
//                Assert.IsNotNull(reference);
//                reference = new FooClass();
//            }

//            public struct FooStruct { }

//            public virtual void RefParameterMethod4(ref FooStruct reference)
//            {
//                Assert.IsNotNull(reference);
//                reference = new FooStruct();
//            }

//            public virtual void OutParameterMethod1(out int output)
//            {
//                output = 5;
//            }

//            public virtual void OutParameterMethod2(out string output)
//            {
//                output = "merry";
//            }

//            public virtual void OutParameterMethod3(out FooClass output)
//            {
//                output = new FooClass();
//            }

//            public virtual void OutParameterMethod4(out FooStruct output)
//            {
//                output = new FooStruct();
//            }
//        }
//        #endregion
//    }
//}
