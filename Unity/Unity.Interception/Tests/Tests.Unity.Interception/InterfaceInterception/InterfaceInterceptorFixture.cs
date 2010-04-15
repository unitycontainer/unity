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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.TransparentProxyInterception;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.InterfaceInterception
{
    [TestClass]
    public class InterfaceInterceptorFixture
    {
        [TestMethod]
        public void InterceptorDoesNotInterceptClass()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            Assert.IsFalse(interceptor.CanIntercept(GetType()));
        }

        [TestMethod]
        public void InterceptorCanInterceptInterface()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            Assert.IsTrue(interceptor.CanIntercept(typeof(IMatchingRule)));
        }

        [TestMethod]
        public void InterceptorReturnsCorrectMethodsForInterceptableType()
        {
            List<MethodImplementationInfo> methods =
                new InterfaceInterceptor().GetInterceptableMethods(typeof(IInterfaceOne), typeof(ImplementationOne)).ToList();

            List<MethodImplementationInfo> expected = Sequence.Collect(
                new MethodImplementationInfo(typeof(IInterfaceOne).GetMethod("TargetMethod"),
                    typeof(ImplementationOne).GetMethod("TargetMethod"))).ToList();

            CollectionAssert.AreEquivalent(expected, methods);
        }

        [TestMethod]
        public void InterceptorMapsGenericMethodsOnClosedGenericInterfaces()
        {
            List<MethodImplementationInfo> methods =
                new InterfaceInterceptor().GetInterceptableMethods(typeof(IGenericOne<string>), typeof(GenericImplementationOne<string>)).ToList();

            List<MethodImplementationInfo> expected =
                GetExpectedMethods<IGenericOne<string>, GenericImplementationOne<string>>("DoSomething")
                .ToList();

            CollectionAssert.AreEquivalent(expected, methods);
        }

        [TestMethod]
        public void InterceptorCreatesProxyInstance()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            ImplementsInterfaceOne target = new ImplementsInterfaceOne();

            IInterceptingProxy proxy = interceptor.CreateProxy(typeof(IInterfaceOne), target);

            IInterfaceOne intercepted = (IInterfaceOne)proxy;
            intercepted.TargetMethod();

            Assert.IsTrue(target.TargetMethodCalled);
        }

        [TestMethod]
        public void GeneratedProxyCallsInterceptionBehaviors()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            ImplementsInterfaceOne target = new ImplementsInterfaceOne();

            IInterceptingProxy proxy = interceptor.CreateProxy(typeof(IInterfaceOne), target);
            CallCountInterceptionBehavior interceptionBehavior = new CallCountInterceptionBehavior();
            proxy.AddInterceptionBehavior(interceptionBehavior);

            IInterfaceOne intercepted = (IInterfaceOne)proxy;

            intercepted.TargetMethod();
            intercepted.TargetMethod();
            intercepted.TargetMethod();

            Assert.AreEqual(3, interceptionBehavior.CallCount);

        }

        [TestMethod]
        public void ParametersPassProperlyToTarget()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            MyDal target = new MyDal();

            CallCountHandler depositHandler = new CallCountHandler();
            CallCountHandler withdrawHandler = new CallCountHandler();
            PipelineManager manager = new PipelineManager();
            manager.SetPipeline(typeof(IDal).GetMethod("Deposit"),
                new HandlerPipeline(new ICallHandler[] { depositHandler }));
            manager.SetPipeline(typeof(IDal).GetMethod("Withdraw"),
                new HandlerPipeline(new ICallHandler[] { withdrawHandler }));
            IInterceptingProxy proxy = interceptor.CreateProxy(typeof(IDal), target);
            proxy.AddInterceptionBehavior(new PolicyInjectionBehavior(manager));

            IDal intercepted = (IDal)proxy;

            intercepted.Deposit(100.0);
            intercepted.Deposit(25.95);
            intercepted.Deposit(19.95);

            intercepted.Withdraw(15.00);
            intercepted.Withdraw(6.25);

            Assert.AreEqual(3, depositHandler.CallCount);
            Assert.AreEqual(2, withdrawHandler.CallCount);

            Assert.AreEqual(100.0 + 25.95 + 19.95 - 15.00 - 6.25, target.Balance);
        }

        [TestMethod]
        public void CanGenerateProxyForClosedGeneric()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            GenericImplementationOne<DateTime> target = new GenericImplementationOne<DateTime>();

            IInterceptingProxy proxy = interceptor.CreateProxy(typeof(IGenericOne<DateTime>), target);
            CallCountInterceptionBehavior interceptionBehavior = new CallCountInterceptionBehavior();
            proxy.AddInterceptionBehavior(interceptionBehavior);

            IGenericOne<DateTime> intercepted = (IGenericOne<DateTime>)proxy;
            DateTime now = DateTime.Now;

            DateTime result = intercepted.DoSomething(now);

            Assert.AreEqual(now, result);
            Assert.IsTrue(target.DidSomething);
            Assert.AreEqual(1, interceptionBehavior.CallCount);
        }

        [TestMethod]
        public void RefsAndOutsAreProperlyHandled()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            ImplementsHaveSomeRefsAndOuts target = new ImplementsHaveSomeRefsAndOuts();

            IInterceptingProxy proxy = interceptor.CreateProxy(typeof(IHaveSomeRefsAndOuts), target);
            CallCountInterceptionBehavior interceptionBehavior = new CallCountInterceptionBehavior();
            proxy.AddInterceptionBehavior(interceptionBehavior);

            IHaveSomeRefsAndOuts intercepted = (IHaveSomeRefsAndOuts)proxy;

            int a;
            string s = "something";
            intercepted.DoSomething(out a, ref s);

            Assert.AreEqual(37, a);
            Assert.AreEqual("+++something***", s);
            Assert.AreEqual(1, interceptionBehavior.CallCount);
        }

        [TestMethod]
        public void AssortedParameterKindsAreProperlyHandled()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            AssortedParameterKindsAreProperlyHandledHelper.TypeWithAssertedParameterKinds target =
                new AssortedParameterKindsAreProperlyHandledHelper.TypeWithAssertedParameterKinds();

            IInterceptingProxy proxy =
                interceptor.CreateProxy(
                    typeof(AssortedParameterKindsAreProperlyHandledHelper.ITypeWithAssertedParameterKinds),
                    target);

            AssortedParameterKindsAreProperlyHandledHelper.PerformTest(proxy);
        }

        [TestMethod]
        public void ReflectingOverProxyTypeReturnsProperties()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();
            IInterceptingProxy proxy = interceptor.CreateProxy(typeof(IHaveSomeProperties), target);

            List<PropertyInfo> interfaceProperties = new List<PropertyInfo>(typeof(IHaveSomeProperties).GetProperties());
            List<PropertyInfo> proxyProperties = new List<PropertyInfo>(proxy.GetType().GetProperties());

            Assert.AreEqual(interfaceProperties.Count, proxyProperties.Count);
        }

        [TestMethod]
        public void ProxyInterceptsEvents()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            ClassWithEvents target = new ClassWithEvents();
            IInterceptingProxy proxy = interceptor.CreateProxy(typeof(IDoEvents), target);
            CallCountInterceptionBehavior interceptionBehavior = new CallCountInterceptionBehavior();
            proxy.AddInterceptionBehavior(interceptionBehavior);

            ((IDoEvents)proxy).SomeEvent += (s, a) => { };

            Assert.AreEqual(1, interceptionBehavior.CallCount);
        }

        [TestMethod]
        public void ProxySendsOriginalWhenRaisingEvent()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            ClassWithEvents target = new ClassWithEvents();
            IInterceptingProxy proxy = interceptor.CreateProxy(typeof(IDoEvents), target);
            CallCountInterceptionBehavior interceptionBehavior = new CallCountInterceptionBehavior();
            proxy.AddInterceptionBehavior(interceptionBehavior);
            object sender = null;
            ((IDoEvents)proxy).SomeEvent += (s, a) => { sender = s; };

            // act
            ((IDoEvents)proxy).TriggerIt();

            // assert
            Assert.AreSame(target, sender);
            Assert.AreEqual(2, interceptionBehavior.CallCount);  // adding + calling TriggerIt
        }


        [TestMethod]
        public void InterceptorCanInterceptProxyInstances()
        {
            CallCountInterceptionBehavior callCounter = new CallCountInterceptionBehavior();

            ProxiedInterfaceImpl impl = new ProxiedInterfaceImpl();
            IProxiedInterface instance = (IProxiedInterface)new MyProxy(typeof(IProxiedInterface), impl).GetTransparentProxy();

            IInstanceInterceptor interceptor = new InterfaceInterceptor();

            IInterceptingProxy proxy = (IInterceptingProxy)interceptor.CreateProxy(typeof(IProxiedInterface), (IProxiedInterface)instance);
            proxy.AddInterceptionBehavior(callCounter);

            IProxiedInterface inter = (IProxiedInterface)proxy;

            Assert.AreEqual("hello world", inter.DoSomething());
            Assert.AreEqual(1, callCounter.CallCount);
        }


        [TestMethod]
        public void CanGetInterceptableMethodsForProxy()
        {
            ProxiedInterfaceImpl impl = new ProxiedInterfaceImpl();
            IProxiedInterface instance = (IProxiedInterface)new MyProxy(typeof(IProxiedInterface), impl).GetTransparentProxy();

            IInstanceInterceptor interceptor = new InterfaceInterceptor();

            var interceptableMethods = interceptor.GetInterceptableMethods(typeof(IProxiedInterface), instance.GetType()).ToArray();

            interceptableMethods.Where(x => x.InterfaceMethodInfo.Name == "DoSomething").AssertHasItems();
        }

        public interface IProxiedInterface
        {
            string DoSomething();
        }

        public class ProxiedInterfaceImpl : IProxiedInterface
        {
            public string DoSomething()
            {
                return "hello world";
            }
        }

        private class MyProxy : RealProxy
        {
            Type t;
            object impl;
            public MyProxy(Type t, object impl)
                : base(t)
            {
                this.t = t;
                this.impl = impl;
            }

            public override IMessage Invoke(IMessage msg)
            {
                IMethodCallMessage method = msg as IMethodCallMessage;
                if (method != null)
                {
                    if (method.MethodName == "GetType")
                    {
                        return new ReturnMessage(t, new object[0], 0, method.LogicalCallContext, method);
                    }
                    object result = method.MethodBase.Invoke(impl, method.InArgs);
                    return new ReturnMessage(result, new object[0], 0, method.LogicalCallContext, method);
                }

                return null;
            }
        }


        [TestMethod]
        public void CanInterceptDerivedInterface()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IOrderService, OrderService>();

            container.AddNewExtension<Interception>()
                .Configure<Interception>()
                    .SetDefaultInterceptorFor<IOrderService>(new InterfaceInterceptor());

            var svc = container.Resolve<IOrderService>();
            svc.Save(new Order());

            //doesnt throw
        }

        public interface IService<T>
        {
            void Save(T entity);
        }

        public interface IOrderService : IService<Order>
        {
            void GetCustomerOrders(Customer customer);
        }

        public abstract class ServiceBase<T> : IService<T>
        {
            public void Save(T entity)
            {
                //Some code that saves a generic entity
            }
        }

        public class OrderService : ServiceBase<Order>, IOrderService
        {
            public void GetCustomerOrders(Customer customer)
            {
                //Some code that gets customer orders
            }
        }

        public class Customer
        {
        }
        public class Order
        {
        }

        public class CallCountAttribute : HandlerAttribute
        {
            public static CallCountHandler Handler = new CallCountHandler();

            public override ICallHandler CreateHandler(IUnityContainer container)
            {
                return Handler;
            }
        }

        private static IEnumerable<MethodImplementationInfo> GetExpectedMethods<TInterface, TImplementation>(params string[] methodNames)
        {
            return GetExpectedMethods(typeof(TInterface), typeof(TImplementation), methodNames);
        }

        private static IEnumerable<MethodImplementationInfo> GetExpectedMethods(Type interfaceType, Type implementationType, params string[] methodNames)
        {
            foreach (string name in methodNames)
            {
                yield return new MethodImplementationInfo(
                    interfaceType.GetMethod(name),
                    implementationType.GetMethod(name));

            }
        }

        internal class ImplementationOne : IInterfaceOne
        {
            public void TargetMethod()
            {

            }
        }

        public interface IGenericOne<T>
        {
            T DoSomething(T something);
        }

        public class GenericImplementationOne<T> : IGenericOne<T>
        {
            public bool DidSomething;

            public T DoSomething(T something)
            {
                DidSomething = true;
                return something;
            }
        }

        class ImplementsInterfaceOne : IInterfaceOne
        {
            public bool TargetMethodCalled;

            public void TargetMethod()
            {
                TargetMethodCalled = true;
            }
        }

        public interface IHaveSomeRefsAndOuts
        {
            void DoSomething(out int a, ref string s);
        }

        class ImplementsHaveSomeRefsAndOuts : IHaveSomeRefsAndOuts
        {
            public void DoSomething(out int a, ref string s)
            {
                a = 37;
                s = "+++" + s + "***";
            }
        }

        [AttributeUsage(AttributeTargets.All)]
        public class SampleAttribute : Attribute
        {
            private readonly string name;

            public SampleAttribute(string name)
            {
                this.name = name;
            }

            public string Name
            {
                get { return name; }
            }
        }

        public interface IHaveSomeProperties
        {
            string AMethod(int x);


            int IntProperty { get; set; }
            [Sample("A name")]
            string Name { get; set; }
        }

        class HasSomeProperties : IHaveSomeProperties
        {
            private int intValue;
            private string stringValue;

            public string AMethod(int x)
            {
                return "**" + x + "++";
            }

            public int IntProperty
            {
                get { return intValue; }
                set { intValue = value; }
            }

            public string Name
            {
                get { return stringValue; }
                set { stringValue = value; }
            }
        }

        class MyDal : IDal
        {
            private double balance;

            public double Balance { get { return balance; } }

            public void Deposit(double amount)
            {
                balance += amount;
            }

            public void Withdraw(double amount)
            {
                balance -= amount;
            }
        }

        public interface IDoEvents
        {
            event EventHandler<EventArgs> SomeEvent;
            void TriggerIt();
        }

        public class ClassWithEvents : IDoEvents
        {
            public event EventHandler<EventArgs> SomeEvent;

            public void TriggerIt()
            {
                SomeEvent(this, new EventArgs());
            }
        }

        public interface IInterfaceTwo
        {
            void TargetMethod(int parameter);
        }

        [TestMethod]
        public void CanCreateProxyForMultipleInterfaces()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();

            // act
            object proxy = interceptor.CreateProxy(typeof(IHaveSomeProperties), target, typeof(IInterfaceOne));

            // assert
            Assert.IsTrue(proxy is IHaveSomeProperties);
            Assert.IsTrue(proxy is IInterfaceOne);
        }

        [TestMethod]
        public void InvokingMethodOnAdditionalInterfaceThrowsIfNotHandledByInterceptor()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();
            object proxy = interceptor.CreateProxy(typeof(IHaveSomeProperties), target, typeof(IInterfaceOne));

            // act
            Exception exception = null;
            try
            {
                ((IInterfaceOne)proxy).TargetMethod();
                Assert.Fail("should have thrown");
            }
            catch (NotImplementedException e)
            {
                exception = e;
            }

            // assert
            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public void CanSuccessfullyInvokeAnAdditionalInterfaceMethodIfAnInterceptorDoesNotForwardTheCall()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();
            object proxy = interceptor.CreateProxy(typeof(IHaveSomeProperties), target, typeof(IInterfaceOne));
            bool invoked = false;
            ((IInterceptingProxy)proxy).AddInterceptionBehavior(
                new DelegateInterceptionBehavior(
                    (input, getNext) => { invoked = true; return input.CreateMethodReturn(null); }));

            // act
            ((IInterfaceOne)proxy).TargetMethod();

            // assert
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void CanImplementINotifyPropertyChanged()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();
            object proxy = interceptor.CreateProxy(typeof(IHaveSomeProperties), target, typeof(INotifyPropertyChanged));
            string changeProperty = null;
            PropertyChangedEventHandler handler = (sender, args) => changeProperty = args.PropertyName;

            ((IInterceptingProxy)proxy).AddInterceptionBehavior(new NaiveINotifyPropertyChangedInterceptionBehavior());
            ((INotifyPropertyChanged)proxy).PropertyChanged += handler;

            ((IHaveSomeProperties)proxy).IntProperty = 100;

            Assert.AreEqual(100, ((IHaveSomeProperties)proxy).IntProperty);
            Assert.AreEqual("IntProperty", changeProperty);

            changeProperty = null;
            ((INotifyPropertyChanged)proxy).PropertyChanged -= handler;

            ((IHaveSomeProperties)proxy).IntProperty = 200;

            Assert.AreEqual(200, ((IHaveSomeProperties)proxy).IntProperty);
            Assert.AreEqual(null, changeProperty);
        }

        [TestMethod]
        public void ProxiedInterfaceIsImplementedImplicitly()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();

            // act
            object proxy = interceptor.CreateProxy(typeof(IHaveSomeProperties), target, typeof(IInterfaceTwo));
            InterfaceMapping interfaceMapping = proxy.GetType().GetInterfaceMap(typeof(IHaveSomeProperties));

            // assert
            Assert.IsNull(interfaceMapping.TargetMethods.FirstOrDefault(mi => !mi.IsPublic));
        }

        [TestMethod]
        public void AdditionalInterfaceIsImplementedExplicitly()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();

            // act
            object proxy = interceptor.CreateProxy(typeof(IHaveSomeProperties), target, typeof(IInterfaceTwo));
            InterfaceMapping interfaceMapping = proxy.GetType().GetInterfaceMap(typeof(IInterfaceTwo));

            // assert
            Assert.IsNull(interfaceMapping.TargetMethods.FirstOrDefault(mi => mi.IsPublic));
        }

        [TestMethod]
        public void CanImplementMultipleInterfacesWithSameMethodSignatures()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            BaseInterfaceImplementation target = new BaseInterfaceImplementation();
            object proxy = interceptor.CreateProxy(typeof(IBaseInterface), target, typeof(IBaseInterface2));
            object[] returnValues = new object[] { this, "return value" };
            int returnValue = 0;
            ((IInterceptingProxy)proxy).AddInterceptionBehavior(
                new DelegateInterceptionBehavior(
                    (mi, getNext) =>
                    {
                        return mi.CreateMethodReturn(returnValues[returnValue++]);
                    }));

            // act
            object value1 = ((IBaseInterface)proxy).TargetMethod();
            string value2 = ((IBaseInterface2)proxy).TargetMethod();

            // assert
            Assert.AreEqual(this, value1);
            Assert.AreEqual("return value", value2);
        }

        [TestMethod]
        public void CanImplementMultipleAdditionalInterfacesWithCommonAncestors()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();

            // act
            object proxy =
                interceptor.CreateProxy(
                    typeof(IHaveSomeProperties),
                    target,
                    typeof(IDerivedInterface1),
                    typeof(IDerivedInterface2));

            // assert
            Assert.IsTrue(proxy is IHaveSomeProperties);
            Assert.IsTrue(proxy is IDerivedInterface1);
            Assert.IsTrue(proxy is IDerivedInterface2);
        }

        [TestMethod]
        public void CanImplementAdditionalClosedGenericInterface()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();

            // act
            object proxy =
                interceptor.CreateProxy(
                    typeof(IHaveSomeProperties),
                    target,
                    typeof(IGenericOne<string>));

            // assert
            Assert.IsTrue(proxy is IHaveSomeProperties);
            Assert.IsTrue(proxy is IGenericOne<string>);
        }

        [TestMethod]
        public void CanImplementAdditionalClosedGenericInterfaceMultipleTimesWithDifferentParameters()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();

            // act
            object proxy =
                interceptor.CreateProxy(
                    typeof(IHaveSomeProperties),
                    target,
                    typeof(IGenericOne<string>),
                    typeof(IGenericOne<int>));

            // assert
            Assert.IsTrue(proxy is IHaveSomeProperties);
            Assert.IsTrue(proxy is IGenericOne<string>);
            Assert.IsTrue(proxy is IGenericOne<int>);
        }

        [TestMethod]
        public void SupplyingOpenGenericInterfacesAsAdditionalInterfacesThrows()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();

            // act
            Exception exception = null;
            try
            {
                object proxy =
                    interceptor.CreateProxy(
                        typeof(IHaveSomeProperties),
                        target,
                        typeof(IGenericOne<>));
            }
            catch (ArgumentException e)
            {
                exception = e;
            }

            // assert
            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public void SupplyingNonInterfacesAsAdditionalInterfacesThrows()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();

            // act
            Exception exception = null;
            try
            {
                object proxy =
                    interceptor.CreateProxy(
                        typeof(IHaveSomeProperties),
                        target,
                        typeof(Component));
            }
            catch (ArgumentException e)
            {
                exception = e;
            }

            // assert
            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public void SupplyingNullInterfacesAsAdditionalInterfacesThrows()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();

            // act
            Exception exception = null;
            try
            {
                object proxy =
                    interceptor.CreateProxy(
                        typeof(IHaveSomeProperties),
                        target,
                        (Type)null);
            }
            catch (ArgumentException e)
            {
                exception = e;
            }

            // assert
            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public void SupplyingANullArrayOfAdditionalInterfacesThrows()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();

            // act
            Exception exception = null;
            try
            {
                object proxy =
                    interceptor.CreateProxy(
                        typeof(IHaveSomeProperties),
                        target,
                        (Type[])null);
            }
            catch (ArgumentNullException e)
            {
                exception = e;
            }

            // assert
            Assert.IsNotNull(exception);
        }

        public interface IBaseInterface
        {
            object TargetMethod();
        }

        public interface IBaseInterface2
        {
            string TargetMethod();
        }

        public interface IDerivedInterface1 : IBaseInterface
        {
            object TargetMethod(string param1);
        }

        public interface IDerivedInterface2 : IBaseInterface
        {
            void TargetMethod(string param1);
        }

        public class BaseInterfaceImplementation : IBaseInterface
        {
            public object TargetMethod()
            {
                return null;
            }
        }

        [TestMethod]
        public void MultipleProxiesForTheSameInterfaceHaveTheSameType()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();

            // act
            object proxy1 = interceptor.CreateProxy(typeof(IHaveSomeProperties), target);
            object proxy2 = interceptor.CreateProxy(typeof(IHaveSomeProperties), target);

            // assert
            Assert.AreSame(proxy1.GetType(), proxy2.GetType());
        }

        [TestMethod]
        public void ProxiesForTheSameInterfaceButDifferentAdditionalInterfacesDoNotHaveTheSameType()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();

            // act
            object proxy1 = interceptor.CreateProxy(typeof(IHaveSomeProperties), target, typeof(IBaseInterface));
            object proxy2 = interceptor.CreateProxy(typeof(IHaveSomeProperties), target, typeof(IBaseInterface2));

            // assert
            Assert.AreNotSame(proxy1.GetType(), proxy2.GetType());
            Assert.IsTrue(proxy1 is IBaseInterface);
            Assert.IsTrue(proxy2 is IBaseInterface2);
        }

        [TestMethod]
        public void ProxiesForTheSameAdditionalInterfacesButInDifferentOrderDoNotHaveTheSameType()
        {
            // arrange
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            HasSomeProperties target = new HasSomeProperties();

            // act
            object proxy1 = interceptor.CreateProxy(typeof(IHaveSomeProperties), target, typeof(IBaseInterface), typeof(IBaseInterface2));
            object proxy2 = interceptor.CreateProxy(typeof(IHaveSomeProperties), target, typeof(IBaseInterface2), typeof(IBaseInterface));

            // assert
            Assert.AreNotSame(proxy1.GetType(), proxy2.GetType());
            Assert.IsTrue(proxy1 is IBaseInterface);
            Assert.IsTrue(proxy1 is IBaseInterface2);
            Assert.IsTrue(proxy2 is IBaseInterface);
            Assert.IsTrue(proxy2 is IBaseInterface2);
        }

        [TestMethod]
        public void InterfaceInterceptorSetsTargetToTargetObject()
        {
            object suppliedTarget = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
                {
                    suppliedTarget = inputs.Target;
                    return getNext()(inputs, getNext);
                });

            var actualTarget = new ImplementsInterfaceOne();

            var proxy = Intercept.ThroughProxy<IInterfaceOne>(
                actualTarget, new InterfaceInterceptor(),
                new[] { behavior });

            proxy.TargetMethod();

            Assert.IsTrue(actualTarget.TargetMethodCalled);
            Assert.AreSame(actualTarget, suppliedTarget);
        }

        [TestMethod]
        public void CanInterceptInterfaceWithGenericMethod()
        {
            var target = new IntegrationFixture.ClassWithGenericMethod();

            bool behaviorWasCalled = false;
            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                behaviorWasCalled = true;
                return getNext()(inputs, getNext);
            });

            var proxy = Intercept.ThroughProxy<IntegrationFixture.IInterfaceWithGenericMethod>(
                target, new InterfaceInterceptor(),
                new[] { behavior });

            proxy.DoSomething<string>();

            Assert.IsTrue(behaviorWasCalled);
        }
    }
}
