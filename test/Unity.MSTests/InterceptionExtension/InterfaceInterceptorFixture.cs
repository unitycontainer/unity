// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Security.Permissions;

using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Tests.TestObjects;

namespace Unity.Tests.InterceptionExtension
{
    [TestClass]
    public class InterfaceInterceptorFixture : InterceptorFixtureBase
    {
        /// <summary>
        /// Bug # 3972 http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=4608
        /// </summary>
        [TestMethod]
        public void InterfaceInterceptionEventHandlerTest()
        {
            IUnityContainer unity = new UnityContainer();
            unity.AddNewExtension<Interception>();
            unity.RegisterType<IMyInterface, MyClass>(
                 new Interceptor<InterfaceInterceptor>(),
                 new InterceptionBehavior<PolicyInjectionBehavior>());
            var o = unity.Resolve<IMyInterface>();
            bool testPassed = false;
            o.E1 += (sender, args) =>
            {
                MyArg arg = (MyArg)args;
                testPassed = arg.Status;
            };
            o.TargetMethod();
            
            Assert.IsTrue(testPassed);
        }

        /// <summary>
        /// Bug # 3972 http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=4608
        /// </summary>
        [TestMethod]
        public void VirtualMethodInterceptionEventHandlerTest()
        {
            IUnityContainer unity = new UnityContainer();
            unity.RegisterType<MyClass>(
              new Interceptor<VirtualMethodInterceptor>(),
              new InterceptionBehavior<PolicyInjectionBehavior>());

            var o = unity.Resolve<MyClass>();

            bool testPassed = false;
            o.E1 += (sender, args) =>
            {
                MyArg arg = (MyArg)args;
                testPassed = arg.Status;
            };
            o.TargetMethod();
            Assert.IsTrue(testPassed);

            o.TargetMethod();
        }

        [TestMethod]
        public virtual void RefParameterMethod1()
        {
            IUnityContainer container = this.GetContainer();

            IParameterTestClass testClass = container.Resolve<IParameterTestClass>();
            int reference = 5;
            testClass.RefParameterMethod1(ref reference);

            Assert.AreEqual(6, reference);
            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        [TestMethod]
        public virtual void RefParameterMethod2()
        {
            IUnityContainer container = this.GetContainer();

            IParameterTestClass testClass = container.Resolve<IParameterTestClass>();
            string reference = "merry";
            testClass.RefParameterMethod2(ref reference);

            Assert.AreEqual("merryxmas", reference);
            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        [TestMethod]
        public virtual void RefParameterMethod3()
        {
            IUnityContainer container = this.GetContainer();

            IParameterTestClass testClass = container.Resolve<IParameterTestClass>();
            ParameterTestClass.FooClass reference = new ParameterTestClass.FooClass();
            ParameterTestClass.FooClass referenceOriginal = reference;
            testClass.RefParameterMethod3(ref reference);

            Assert.AreNotSame(referenceOriginal, reference);
            Assert.IsNotNull(reference);
            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        [TestMethod]
        public virtual void RefParameterMethod4()
        {
            IUnityContainer container = this.GetContainer();

            IParameterTestClass testClass = container.Resolve<IParameterTestClass>();
            ParameterTestClass.FooStruct reference = new ParameterTestClass.FooStruct();
            ParameterTestClass.FooStruct referenceOriginal = reference;
            testClass.RefParameterMethod4(ref reference);

            Assert.AreNotSame(referenceOriginal, reference);
            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        [TestMethod]
        public virtual void InterceptionWorksIfConcreteClassImplementsMethodImplicitly()
        {
            IUnityContainer container = this.GetContainer();

            IImplicitExplicitTestClass testClass = container.Resolve<IImplicitExplicitTestClass>();
            object o = testClass.ImplicitMethod();

            Assert.IsNotNull(o);
            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        [TestMethod]
        public virtual void InterceptionWorksIfConcreteClassImplementsMethodExplicitly()
        {
            IUnityContainer container = this.GetContainer();

            IImplicitExplicitTestClass testClass = container.Resolve<IImplicitExplicitTestClass>();
            object o = testClass.ExplicitMethod();

            Assert.IsNotNull(o);
            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        [TestMethod]
        public virtual void OutParameterMethod1()
        {
            IUnityContainer container = this.GetContainer();

            IParameterTestClass testClass = container.Resolve<IParameterTestClass>();
            int reference;
            testClass.OutParameterMethod1(out reference);

            Assert.AreEqual(5, reference);
            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        [TestMethod]
        public virtual void OutParameterMethod2()
        {
            IUnityContainer container = this.GetContainer();

            IParameterTestClass testClass = container.Resolve<IParameterTestClass>();
            string reference;
            testClass.OutParameterMethod2(out reference);

            Assert.AreEqual("merry", reference);
            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        [TestMethod]
        public virtual void OutParameterMethod3()
        {
            IUnityContainer container = this.GetContainer();

            IParameterTestClass testClass = container.Resolve<IParameterTestClass>();
            ParameterTestClass.FooClass reference;
            testClass.OutParameterMethod3(out reference);

            Assert.IsNotNull(reference);
            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        [TestMethod]
        public virtual void OutParameterMethod4()
        {
            IUnityContainer container = this.GetContainer();

            IParameterTestClass testClass = container.Resolve<IParameterTestClass>();
            ParameterTestClass.FooStruct reference = new ParameterTestClass.FooStruct();
            ParameterTestClass.FooStruct referenceOriginal = reference;
            testClass.OutParameterMethod4(out reference);

            Assert.AreNotSame(referenceOriginal, reference);
            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        [TestMethod]
        public virtual void SealedConcreteClassesCanBeIntercepted()
        {
            IUnityContainer container = this.GetContainer();

            ISealedTestClass testClass = container.Resolve<ISealedTestClass>();
            object o = testClass.TestMethod();

            Assert.IsNotNull(o);
            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        [TestMethod]
        public void WhenInterceptingClassImplementingDerivedInterface()
        {
            var container = new UnityContainer();
            container.AddNewExtension<Interception>();

            container.RegisterType<IDerivedInterface, FromDerivedInterface>();
            container.RegisterType<ICallHandler, MyCallHandler>();

            container.Configure<Interception>()
                        .SetInterceptorFor<IDerivedInterface>(new InterfaceInterceptor())
                        .AddPolicy("PolicyName")
                            .AddMatchingRule(new TypeMatchingRule(typeof(IDerivedInterface)))
                            .AddCallHandler(new MyCallHandler());

            var instance = container.Resolve<IDerivedInterface>();

            string result = instance.AnotherMethod("calling...");

            Assert.AreEqual("calling... I've been here!", result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public virtual void AbstractConcreteClassesCannotBeIntercepted()
        {
            IUnityContainer container = this.GetContainer();

            container.RegisterType<IAbstractTestClass, AbstractTestClass>();

            container.Configure<Interception>()
                .SetDefaultInterceptorFor<AbstractTestClass>(new TransparentProxyInterceptor());
        }

        

        [TestMethod]
        public virtual void TestBasicInterceptionEvents()
        {
            IUnityContainer container = this.GetContainer();

            IBasicInterceptionTestClass testClass = container.Resolve<IBasicInterceptionTestClass>();
            testClass.EventTest1 += new EventHandler(this.TestClass_EventTest1);
            testClass.FireEventTest1();
            testClass.EventTest1 -= new EventHandler(this.TestClass_EventTest1);

            Assert.AreEqual(3, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        private void TestClass_EventTest1(object sender, EventArgs e)
        {
        }

        [TestMethod]
        public virtual void TestBasicInterceptionMethods()
        {
            IUnityContainer container = this.GetContainer();

            IBasicInterceptionTestClass testClass = container.Resolve<IBasicInterceptionTestClass>();
            testClass.MethodTest1();

            Assert.AreEqual(1, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        [TestMethod]
        public virtual void TestBasicInterceptionIndexers()
        {
            IUnityContainer container = this.GetContainer();

            IBasicInterceptionTestClass testClass = container.Resolve<IBasicInterceptionTestClass>();
            testClass[5] = new object();
            object o = testClass[3];

            Assert.AreEqual(2, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        [TestMethod]
        public virtual void TestBasicInterceptionProperties()
        {
            IUnityContainer container = this.GetContainer();

            IBasicInterceptionTestClass testClass = container.Resolve<IBasicInterceptionTestClass>();
            object o1 = testClass.PropertyTest1;
            testClass.PropertyTest2 = new object();
            testClass.PropertyTest3 = new object();
            object o2 = testClass.PropertyTest3;
            object o3 = testClass.PropertyTest4;

            Assert.AreEqual(5, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        [TestMethod]
        public virtual void TestReflection()
        {
            IUnityContainer container = this.GetContainer();

            IReflectionTestClass testClass = container.Resolve<IReflectionTestClass>();
            Type interceptedType = testClass.GetType();

            Assert.IsTrue(typeof(IReflectionTestClass).IsAssignableFrom(interceptedType));
        }

       
        

        
        [TestMethod]
        public virtual void TestMultipleGenerics()
        {
            IUnityContainer container = this.GetContainer();

            IGenericTestClass<ReflectionTestClass> testClass1 = container.Resolve<IGenericTestClass<ReflectionTestClass>>();
            IGenericTestClass<ReflectionTestClass1> testClass2 = container.Resolve<IGenericTestClass<ReflectionTestClass1>>();
            IGenericTestClass<ReflectionTestClass2> testClass3 = container.Resolve<IGenericTestClass<ReflectionTestClass2>>();

            Assert.IsNotNull(testClass1);
            Assert.IsNotNull(testClass2);
            Assert.IsNotNull(testClass3);
        }

       

        [TestMethod]
        public virtual void TestNewVirtualOverride()
        {
            IUnityContainer container = this.GetContainer();

            INewVirtualOverrideTestClass testClass = container.Resolve<INewVirtualOverrideTestClass>();

            Assert.IsTrue(testClass.TestMethod1(), "override");
            Assert.IsTrue(testClass.TestMethod2(), "new virtual");
            Assert.IsTrue(testClass.TestMethod3(), "always true");
            Assert.IsTrue(testClass.TestMethod4(), "abstract");
            Assert.AreEqual(4, container.Resolve<TestCallHandler>("TestCallHandler").InterceptionCount);
        }

        [TestMethod]
        public void WhenTransparentProxyIsInterceptedUsingInterfaceInterceptor()
        {
            bool intercepted = false;
            InterfaceInterceptor interceptor = new InterfaceInterceptor();

            TestProxy<ITest, TestClass> myProxy = new TestProxy<ITest, TestClass>();

            ITest instance = myProxy.CreateProxy();

            var interceptedMethodList = from i in interceptor.GetInterceptableMethods(typeof(ITest), instance.GetType())
                                        where i.InterfaceMethodInfo.Name == "TestMethod"
                                        select i;

            bool containsMethod = interceptedMethodList.Count() == 1;

            Assert.IsTrue(containsMethod);

            var interceptedProxy = interceptor.CreateProxy(typeof(ITest), instance);

            interceptedProxy.AddInterceptionBehavior(
                                                    new ActionInterceptionBehavior(() => intercepted = true));

            int result = ((ITest)interceptedProxy).TestMethod("sample");

            Assert.IsTrue(intercepted);
        }

        protected override IUnityContainer GetContainer()
        {
            IUnityContainer container = base.GetContainer();

            container.RegisterType<IParameterTestClass, ParameterTestClass>();
            container.RegisterType<IImplicitExplicitTestClass, ImplicitExplicitTestClass>();
            container.RegisterType<ISealedTestClass, SealedTestClass>();
            container.RegisterType<IIdentityTestClass, IdentityTestClass>();
            container.RegisterType<IBasicInterceptionTestClass, BasicInterceptionTestClass>();
            container.RegisterType<IReflectionTestClass, ReflectionTestClass>();
            container.RegisterType<IAttributeTestClass, AttributeTestClass>();
            container.RegisterType(typeof(IGenericTestClass<>), typeof(GenericTestClass<>));
            container.RegisterType<INewVirtualOverrideTestClass, NewVirtualOverrideTestClass>();

            container.Configure<Interception>()
                .SetDefaultInterceptorFor<IParameterTestClass>(new TransparentProxyInterceptor())
                .SetDefaultInterceptorFor<IImplicitExplicitTestClass>(new TransparentProxyInterceptor())
                .SetDefaultInterceptorFor<ISealedTestClass>(new TransparentProxyInterceptor())
                .SetDefaultInterceptorFor<IIdentityTestClass>(new TransparentProxyInterceptor())
                .SetDefaultInterceptorFor<IBasicInterceptionTestClass>(new TransparentProxyInterceptor())
                .SetDefaultInterceptorFor<IReflectionTestClass>(new TransparentProxyInterceptor())
                .SetDefaultInterceptorFor<IAttributeTestClass>(new TransparentProxyInterceptor())
                .SetDefaultInterceptorFor(typeof(IGenericTestClass<>), new TransparentProxyInterceptor())
                .SetDefaultInterceptorFor<INewVirtualOverrideTestClass>(new TransparentProxyInterceptor());

            return container;
        }

        public interface INewVirtualOverrideTestClass
        {
            bool TestMethod1();
            bool TestMethod2();
            bool TestMethod3();
            bool TestMethod4();
        }

        public abstract class NewVirtualOverrideTestClassBase
        {
            public virtual bool TestMethod1()
            {
                return false;
            }

            public virtual bool TestMethod2()
            {
                return false;
            }

            public virtual bool TestMethod3()
            {
                return true;
            }

            public abstract bool TestMethod4();
        }

        public class NewVirtualOverrideTestClass : NewVirtualOverrideTestClassBase, INewVirtualOverrideTestClass
        {
            public override bool TestMethod1()
            {
                return true;
            }

            public new virtual bool TestMethod2()
            {
                return true;
            }

            public override bool TestMethod4()
            {
                return true;
            }
        }

        public interface IGenericTestClass<T>
            where T : class
        {
            void TestMethod1<T1>();

            void TestMethod2<T2>()
                where T2 : struct;

            void TestMethod3<T3>()
                where T3 : class;

            void TestMethod4<T4>()
                where T4 : class, new();

            void TestMethod5<T5>()
                where T5 : InjectionPolicy;

            void TestMethod6<T6>()
                where T6 : IMatchingRule;
        }

        public class GenericTestClass<T> : IGenericTestClass<T>
            where T : class
        {
            public void TestMethod1<T1>() { }

            public void TestMethod2<T2>() where T2 : struct { }

            public void TestMethod3<T3>() where T3 : class { }

            public void TestMethod4<T4>() where T4 : class, new() { }

            public void TestMethod5<T5>() where T5 : InjectionPolicy { }

            public void TestMethod6<T6>() where T6 : IMatchingRule { }
        }

        public interface IReflectionTestClass
        {
            void TestMethod();
        }

        public class ReflectionTestClass : IReflectionTestClass
        {
            public void TestMethod()
            { }
        }

        public class ReflectionTestClass1 : IReflectionTestClass
        {
            public void TestMethod()
            { }
        }

        public class ReflectionTestClass2 : IReflectionTestClass
        {
            public void TestMethod()
            { }
        }

        [AttributeUsage(AttributeTargets.All)]
        public class TestAttribute : Attribute
        {
            public int MyProperty { get; set; }
        }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        public class MultipleTestAttribute : Attribute
        {
            private string name;
            public string Name
            {
                get { return this.name; }
            }

            public MultipleTestAttribute(string name)
            {
                this.name = name;
            }
        }

        [AttributeUsage(AttributeTargets.All, Inherited = true)]
        public class InheritableTestAttribute : Attribute
        {
        }

        [MultipleTest("A")]
        [MultipleTest("B")]
        [MultipleTest("C")]
        [Test]
        [InheritableTest]
        public interface IAttributeTestClass
        {
            [MultipleTest("AA")]
            [MultipleTest("BB")]
            [MultipleTest("CC")]
            [Test]
            void TestMethod1();

            [Test(MyProperty = 5)]
            [InheritableTest]
            void TestMethod2();

            [MultipleTest("AAA")]
            [MultipleTest("BBB")]
            [MultipleTest("CCC")]
            [InheritableTest]
            [Test(MyProperty = 5)]
            event EventHandler TestEvent;

            [MultipleTest("AAAA")]
            [MultipleTest("BBBB")]
            [MultipleTest("CCCC")]
            [InheritableTest]
            [Test(MyProperty = 5)]
            int this[int index]
            {
                get;
                set;
            }

            [MultipleTest("AAAAA")]
            [MultipleTest("BBBBB")]
            [MultipleTest("CCCCC")]
            [InheritableTest]
            [Test(MyProperty = 5)]
            object TestProperty
            {
                get;
                set;
            }
        }

        public class AttributeTestClass : IAttributeTestClass
        {
            public void TestMethod1() { }

            public void TestMethod2() { }

            public event EventHandler TestEvent;

            public int this[int index]
            {
                get { return 5; }
                set { }
            }

            public object TestProperty
            {
                get { return new object(); }
                set { }
            }
        }

        public interface IBasicInterceptionTestClass
        {
            event EventHandler EventTest1;

            void FireEventTest1();

            object this[int item] { get; set; }

            object MethodTest1();

            object PropertyTest1 { get; }

            object PropertyTest2 { set; }

            object PropertyTest3 { get; set; }

            object PropertyTest4 { get; }
        }

        public class BasicInterceptionTestClass : IBasicInterceptionTestClass
        {
            private object indexerTest1;
            private object propertyTest3;
            private object propertyTest4;

            public event EventHandler EventTest1;

            public void FireEventTest1()
            {
                EventTest1(this, EventArgs.Empty);
            }

            public object this[int item]
            {
                get { return indexerTest1; }
                set { indexerTest1 = value; }
            }

            public object MethodTest1()
            {
                return new object();
            }

            public object PropertyTest1
            {
                get { return new object(); }
            }

            public object PropertyTest2
            {
                set { }
            }

            public object PropertyTest3
            {
                get { return propertyTest3; }
                set { propertyTest3 = value; }
            }

            public object PropertyTest4
            {
                get { return propertyTest4; }
                protected set { propertyTest4 = value; }
            }
        }

        public interface IIdentityTestClass
        {
            [Dependency]
            IIdentityTestClass InterfaceSelfInterface { get; set; }

            [Dependency]
            IdentityTestClass InterfaceSelfConcrete { get; set; }
        }

        public class IdentityTestClass : IIdentityTestClass
        {
            private IdentityTestClass selfConcrete;
            private IIdentityTestClass selfInterface;

            [Dependency]
            public virtual IdentityTestClass SelfConcrete
            {
                get { return selfConcrete; }
                set { selfConcrete = value; }
            }

            [Dependency]
            public virtual IIdentityTestClass SelfInterface
            {
                get { return selfInterface; }
                set { selfInterface = value; }
            }

            private IdentityTestClass interfaceSelfConcrete;
            private IIdentityTestClass interfaceSelfInterface;

            public IIdentityTestClass InterfaceSelfInterface
            {
                get { return interfaceSelfInterface; }
                set { interfaceSelfInterface = value; }
            }

            public IdentityTestClass InterfaceSelfConcrete
            {
                get { return interfaceSelfConcrete; }
                set { interfaceSelfConcrete = value; }
            }
        }

        public interface IAbstractTestClass
        {
            object TestMethod();
        }

        public abstract class AbstractTestClass : IAbstractTestClass
        {
            public object TestMethod()
            {
                return new object();
            }
        }

        public interface ISealedTestClass
        {
            object TestMethod();
        }

        public sealed class SealedTestClass : ISealedTestClass
        {
            public object TestMethod()
            {
                return new object();
            }
        }

        public interface IImplicitExplicitTestClass
        {
            object ImplicitMethod();
            object ExplicitMethod();
        }

        public class ImplicitExplicitTestClass : IImplicitExplicitTestClass
        {
            public object ImplicitMethod()
            {
                return new object();
            }

            object IImplicitExplicitTestClass.ExplicitMethod()
            {
                return new object();
            }
        }

        public interface IParameterTestClass
        {
            void RefParameterMethod1(ref int reference);
            void RefParameterMethod2(ref string reference);
            void RefParameterMethod3(ref ParameterTestClass.FooClass reference);
            void RefParameterMethod4(ref ParameterTestClass.FooStruct reference);
            void OutParameterMethod1(out int reference);
            void OutParameterMethod2(out string reference);
            void OutParameterMethod3(out ParameterTestClass.FooClass reference);
            void OutParameterMethod4(out ParameterTestClass.FooStruct reference);
        }

        public class ParameterTestClass : IParameterTestClass
        {
            void IParameterTestClass.RefParameterMethod1(ref int reference)
            {
                reference++;
            }

            void IParameterTestClass.RefParameterMethod2(ref string reference)
            {
                reference = reference + "xmas";
            }

            public class FooClass { }

            public void RefParameterMethod3(ref FooClass reference)
            {
                Assert.IsNotNull(reference);
                reference = new FooClass();
            }

            public struct FooStruct { }

            public void RefParameterMethod4(ref FooStruct reference)
            {
                Assert.IsNotNull(reference);
                reference = new FooStruct();
            }

            public void OutParameterMethod1(out int output)
            {
                output = 5;
            }

            public void OutParameterMethod2(out string output)
            {
                output = "merry";
            }

            void IParameterTestClass.OutParameterMethod3(out FooClass output)
            {
                output = new FooClass();
            }

            void IParameterTestClass.OutParameterMethod4(out FooStruct output)
            {
                output = new FooStruct();
            }
        }

        public interface IBaseInterface
        {
            void MyBaseMethod();
        }

        public interface IDerivedInterface : IBaseInterface
        {
            string AnotherMethod(string param);
        }

        public class FromDerivedInterface : IDerivedInterface
        {
            public string AnotherMethod(string param)
            {
                return param;
            }

            public void MyBaseMethod()
            {
            }
        }

        public interface ITest
        {
            int TestMethod(string param);
        }

        public class TestClass : ITest
        {
            public int TestMethod(string param)
            {
                return param.Length;
            }
        }

        public class TestProxy<TInterface, TType> : RealProxy
        {
            private TType instance;
            public TestProxy() :
                base(typeof(TInterface))
            {
                this.instance = Activator.CreateInstance<TType>();
            }

            public TInterface CreateProxy()
            {
                return (TInterface)this.GetTransparentProxy();
            }

            public override IMessage Invoke(IMessage msg)
            {
                if (msg is IMethodCallMessage)
                {
                    IMethodCallMessage callMessage = msg as IMethodCallMessage;

                    object result = callMessage.MethodBase.Invoke(this.instance, callMessage.Args);

                    return new ReturnMessage(result, new object[0], 0, null, callMessage);
                }

                return null;
            }
        }
    }
}
