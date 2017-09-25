// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ObjectBuilder2;
using Unity.InterceptionExtension.Tests.MatchingRules;
using Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.CompilerServices;

namespace Unity.InterceptionExtension.Tests.InterfaceInterception
{
    [TestClass]
    public partial class InterfaceInterceptorFixture
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

            CollectionAssertExtensions.AreEquivalent(expected, methods);
        }

        [TestMethod]
        public void InterceptorIncludesMethodsFromBaseInterfaceForInterface()
        {
            List<MethodImplementationInfo> methods =
                new InterfaceInterceptor().GetInterceptableMethods(typeof(Interface),
                    typeof(Interface)).ToList();

            List<MethodImplementationInfo> expected = Sequence.Collect(
                new MethodImplementationInfo(typeof(Interface).GetMethod("Method"),
                    typeof(Interface).GetMethod("Method")),
                    new MethodImplementationInfo(typeof(InterfaceBase).GetMethod("Method3"),
                        typeof(InterfaceBase).GetMethod("Method3")))
                .ToList();

            CollectionAssertExtensions.AreEquivalent(expected, methods);
        }

        [TestMethod]
        public void InterceptorIncludesMethodsFromBaseInterfaceForInterceptableType()
        {
            List<MethodImplementationInfo> methods =
                new InterfaceInterceptor().GetInterceptableMethods(typeof(Interface),
                    typeof(WrappableThroughInterface)).ToList();

            List<MethodImplementationInfo> expected = Sequence.Collect(
                new MethodImplementationInfo(typeof(Interface).GetMethod("Method"),
                    typeof(WrappableThroughInterface).GetMethod("Method")),
                    new MethodImplementationInfo(typeof(InterfaceBase).GetMethod("Method3"),
                        typeof(WrappableThroughInterface).GetMethod("Method3")))
                .ToList();

            CollectionAssertExtensions.AreEquivalent(expected, methods);
        }

        [TestMethod]
        public void InterceptorMapsGenericMethodsOnClosedGenericInterfaces()
        {
            List<MethodImplementationInfo> methods =
                new InterfaceInterceptor().GetInterceptableMethods(typeof(IGenericOne<string>), typeof(GenericImplementationOne<string>)).ToList();

            List<MethodImplementationInfo> expected =
                GetExpectedMethods<IGenericOne<string>, GenericImplementationOne<string>>("DoSomething")
                .ToList();

            CollectionAssertExtensions.AreEquivalent(expected, methods);
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

        private class ImplementsInterfaceOne : IInterfaceOne
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

        private class ImplementsHaveSomeRefsAndOuts : IHaveSomeRefsAndOuts
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
                get { return this.name; }
            }
        }

        public interface IHaveSomeProperties
        {
            string AMethod(int x);

            int IntProperty { get; set; }
            [Sample("A name")]
            string Name { get; set; }
        }

        private class HasSomeProperties : IHaveSomeProperties
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

        private class MyDal : IDal
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
                        typeof(Exception));
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
            var target = new ClassWithGenericMethod();

            bool behaviorWasCalled = false;
            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                behaviorWasCalled = true;
                return getNext()(inputs, getNext);
            });

            var proxy = Intercept.ThroughProxy<IInterfaceWithGenericMethod>(
                target, new InterfaceInterceptor(),
                new[] { behavior });

            proxy.DoSomething<string>();

            Assert.IsTrue(behaviorWasCalled);
        }

        public interface IConstrainedInterface<T>
            where T : IBaseInterface
        {
            void SomeMethod();

            //void SomeGenericMethod<TParam>()
            //    where TParam : IBaseInterface;
        }

        public class ConstrainedImpl : IConstrainedInterface<IBaseInterface>
        {
            public void SomeMethod() { }
            //public void SomeGenericMethod<TParam>() where TParam : IBaseInterface
            //{
            //}
        }

#if false
        /// <summary>
        /// Test class used to reverse engineer required generated code.
        /// </summary>
        class ConstraintedInterfaceInterceptor<T> : IInterceptingProxy, IConstrainedInterface<T>
            //where T : IBaseInterface
        {
            readonly PipelineManager pipelines;
            private readonly IConstrainedInterface<T> target;

            public ConstraintedInterfaceInterceptor(IConstrainedInterface<T> target)
            {
                this.target = target;
                pipelines = new PipelineManager();
            }

            public void SomeMethod()
            {
                MethodInfo targetMethod = typeof(IConstrainedInterface<T>).GetMethod("SomeMethod");

                VirtualMethodInvocation input = new VirtualMethodInvocation(target, targetMethod);
                HandlerPipeline pipeline = pipelines.GetPipeline(targetMethod);
                IMethodReturn returnMessage = pipeline.Invoke(input, delegate(IMethodInvocation inputs, GetNextHandlerDelegate getNext)
                {
                    try
                    {
                        target.SomeMethod();
                        return inputs.CreateMethodReturn(null);
                    }
                    catch (Exception ex)
                    {

                        return inputs.CreateExceptionMethodReturn(ex);
                    }
                });

                if (returnMessage.Exception != null)
                {
                    throw returnMessage.Exception;
                }
            }

            public void SomeGenericMethod<TParam>() where TParam : IBaseInterface
            {
                MethodInfo mi = typeof (IConstrainedInterface<T>).GetMethod("SomeGenericMethod");
                var t1 = typeof (IConstrainedInterface<T>);
                var t2 = typeof (IConstrainedInterface<>);
                var mh = mi.MethodHandle;
                var th = mi.DeclaringType.TypeHandle;
                var tm = MethodBase.GetMethodFromHandle(mh, th);

                MethodInfo targetMethod = typeof(IConstrainedInterface<T>).GetMethod("SomeGenericMethod");

                VirtualMethodInvocation input = new VirtualMethodInvocation(target, targetMethod);
                HandlerPipeline pipeline = pipelines.GetPipeline(targetMethod);
                IMethodReturn returnMessage = pipeline.Invoke(input, delegate(IMethodInvocation inputs, GetNextHandlerDelegate getNext)
                {
                    try
                    {
                        target.SomeGenericMethod<TParam>();
                        return inputs.CreateMethodReturn(null);
                    }
                    catch (Exception ex)
                    {

                        return inputs.CreateExceptionMethodReturn(ex);
                    }
                });

                if (returnMessage.Exception != null)
                {
                    throw returnMessage.Exception;
                }
            }

            public void AddInterceptionBehavior(IInterceptionBehavior interceptor)
            {
                throw new NotImplementedException();
            }
        }
#endif

        [TestMethod]
        public void CanInterceptGenericInterfaceWithInterfaceConstraint()
        {
            var target = new ConstrainedImpl();

            bool behaviorWasCalled = false;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                behaviorWasCalled = true;
                return getNext()(inputs, getNext);
            });

            var proxy = Intercept.ThroughProxy<IConstrainedInterface<IBaseInterface>>(
                target, new InterfaceInterceptor(),
                new[] { behavior });

            proxy.SomeMethod();

            Assert.IsTrue(behaviorWasCalled);
        }

        [TestMethod]
        public void CanInterceptNonGenericMethodOnNonGenericInterface()
        {
            var target = new NonGenericClass();

            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var proxy =
                Intercept.ThroughProxy<INonGenericInterface>(
                    target,
                    new InterfaceInterceptor(),
                    new[] { behavior });

            proxy.NonGenericMethod(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodOnNonGenericInterface()
        {
            var target = new NonGenericClass();

            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var proxy =
                Intercept.ThroughProxy<INonGenericInterface>(
                    target,
                    new InterfaceInterceptor(),
                    new[] { behavior });

            proxy.GenericMethod<string>(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodWithConstraintOnNonGenericInterface()
        {
            var target = new NonGenericClass();

            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var proxy =
                Intercept.ThroughProxy<INonGenericInterface>(
                    target,
                    new InterfaceInterceptor(),
                    new[] { behavior });

            proxy.GenericMethodWithConstraints<string>(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptNonGenericMethodOnGenericInterface()
        {
            var target = new GenericClass<IEnumerable>();

            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var proxy =
                Intercept.ThroughProxy<IGenericInterface<IEnumerable>>(
                    target,
                    new InterfaceInterceptor(),
                    new[] { behavior });

            proxy.NonGenericMethod(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodOnGenericInterface()
        {
            var target = new GenericClass<IEnumerable>();

            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var proxy =
                Intercept.ThroughProxy<IGenericInterface<IEnumerable>>(
                    target,
                    new InterfaceInterceptor(),
                    new[] { behavior });

            proxy.GenericMethod<string>(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodOnGenericInterfaceUsingValueType()
        {
            var target = new GenericClass<int>();

            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var proxy =
                Intercept.ThroughProxy<IGenericInterface<int>>(
                    target,
                    new InterfaceInterceptor(),
                    new[] { behavior });

            var result = proxy.GenericMethodDifferentReturnType<string>(100, null);

            Assert.AreEqual(100, result);
            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(int), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(int), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodWithConstraintOnGenericInterface()
        {
            var target = new GenericClass<IEnumerable>();

            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var proxy =
                Intercept.ThroughProxy<IGenericInterface<IEnumerable>>(
                    target,
                    new InterfaceInterceptor(),
                    new[] { behavior });

            proxy.GenericMethodWithConstraints<string>(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodWithConstraintRelatedToInterfaceOnGenericInterface()
        {
            var target = new GenericClass<IEnumerable>();

            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var proxy =
                Intercept.ThroughProxy<IGenericInterface<IEnumerable>>(
                    target,
                    new InterfaceInterceptor(),
                    new[] { behavior });

            proxy.GenericMethodWithConstraintsOnTheInterfaceParameter<string>(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptNonGenericMethodOnGenericInterfaceWithConstraint()
        {
            var target = new GenericClassWithConstraint<IEnumerable>();

            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var proxy =
                Intercept.ThroughProxy<IGenericInterfaceWithConstraint<IEnumerable>>(
                    target,
                    new InterfaceInterceptor(),
                    new[] { behavior });

            proxy.NonGenericMethod(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodOnGenericInterfaceWithConstraint()
        {
            var target = new GenericClassWithConstraint<IEnumerable>();

            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var proxy =
                Intercept.ThroughProxy<IGenericInterfaceWithConstraint<IEnumerable>>(
                    target,
                    new InterfaceInterceptor(),
                    new[] { behavior });

            proxy.GenericMethod<string>(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodWithConstraintOnGenericInterfaceWithConstraint()
        {
            var target = new GenericClassWithConstraint<IEnumerable>();

            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var proxy =
                Intercept.ThroughProxy<IGenericInterfaceWithConstraint<IEnumerable>>(
                    target,
                    new InterfaceInterceptor(),
                    new[] { behavior });

            proxy.GenericMethodWithConstraints<string>(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodWithConstraintRelatedToInterfaceOnGenericInterfaceWithConstraint()
        {
            var target = new GenericClassWithConstraint<IEnumerable>();

            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var proxy =
                Intercept.ThroughProxy<IGenericInterfaceWithConstraint<IEnumerable>>(
                    target,
                    new InterfaceInterceptor(),
                    new[] { behavior });

            proxy.GenericMethodWithConstraintsOnTheInterfaceParameter<string>(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        public interface INonGenericInterface
        {
            string NonGenericMethod(IEnumerable param1, string param2);

            T GenericMethod<T>(IEnumerable param1, T param2);

            T GenericMethodWithConstraints<T>(IEnumerable param1, T param2) where T : class;
        }

        public interface IGenericInterface<T1>
        {
            string NonGenericMethod(T1 param1, string param2);

            T GenericMethod<T>(T1 param1, T param2);

            T1 GenericMethodDifferentReturnType<T>(T1 param1, T param2);

            T GenericMethodWithConstraints<T>(T1 param1, T param2) where T : class;

            T GenericMethodWithConstraintsOnTheInterfaceParameter<T>(T1 param1, T param2) where T : T1;
        }

        public interface IGenericInterfaceWithConstraint<T1>
            where T1 : class
        {
            string NonGenericMethod(T1 param1, string param2);

            T GenericMethod<T>(T1 param1, T param2);

            T GenericMethodWithConstraints<T>(T1 param1, T param2) where T : class;

            T GenericMethodWithConstraintsOnTheInterfaceParameter<T>(T1 param1, T param2) where T : T1;
        }

        public class NonGenericClass : INonGenericInterface
        {
            public string NonGenericMethod(IEnumerable param1, string param2)
            {
                return null;
            }

            public T GenericMethod<T>(IEnumerable param1, T param2)
            {
                return default(T);
            }

            public T GenericMethodWithConstraints<T>(IEnumerable param1, T param2) where T : class
            {
                return default(T);
            }
        }

        public class GenericClass<T1> : IGenericInterface<T1>
        {
            public string NonGenericMethod(T1 param1, string param2)
            {
                return null;
            }

            public T GenericMethod<T>(T1 param1, T param2)
            {
                var handle = MethodBase.GetCurrentMethod().MethodHandle;

                return default(T);
            }

            public T GenericMethodWithConstraints<T>(T1 param1, T param2) where T : class
            {
                return default(T);
            }

            public T GenericMethodWithConstraintsOnTheInterfaceParameter<T>(T1 param1, T param2) where T : T1
            {
                return default(T);
            }

            public T1 GenericMethodDifferentReturnType<T>(T1 param1, T param2)
            {
                return param1;
            }
        }

        public class GenericClassWithConstraint<T1> : IGenericInterfaceWithConstraint<T1>
            where T1 : class
        {
            public string NonGenericMethod(T1 param1, string param2)
            {
                return null;
            }

            public T GenericMethod<T>(T1 param1, T param2)
            {
                return default(T);
            }

            public T GenericMethodWithConstraints<T>(T1 param1, T param2) where T : class
            {
                return default(T);
            }

            public T GenericMethodWithConstraintsOnTheInterfaceParameter<T>(T1 param1, T param2) where T : T1
            {
                return default(T);
            }
        }

        [TestMethod]
        public void CanInterceptConstrainedInheritedInterfaceMethod()
        {
            var target = new DerivedNonGenericClass();

            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var proxy =
                Intercept.ThroughProxy<IDerivedNonGenericInterface>(
                    target,
                    new InterfaceInterceptor(),
                    new[] { behavior });

            proxy.Test<List<string>>();

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(List<string>), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(0, invocation.MethodBase.GetParameters().Count());
        }

        public interface IBaseGenericInterface<TBase>
        {
            TDerived Test<TDerived>() where TDerived : TBase;
        }

        public class BaseGenericClass<TBaseType> : IBaseGenericInterface<TBaseType>
        {
            public virtual TDerivedType Test<TDerivedType>() where TDerivedType : TBaseType
            {
                return default(TDerivedType);
            }
        }

        public interface IDerivedNonGenericInterface : IBaseGenericInterface<ICollection<string>>
        {
        }

        public class DerivedNonGenericClass : BaseGenericClass<ICollection<string>>, IDerivedNonGenericInterface
        {
        }

        [TestMethod]
        public void CanInterceptConstrainedInheritedInterfaceMethod2()
        {
            var target = new Class3<ICollection<string>>();

            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var proxy =
                Intercept.ThroughProxy<IInterface3<ICollection<string>>>(
                    target,
                    new InterfaceInterceptor(),
                    new[] { behavior });

            proxy.TestMethod<List<string>, DerivedType>(
                new DerivedType(),
                new BaseType[0],
                Enumerable.Empty<DerivedType>(),
                new List<string>[0]);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(List<string>), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(4, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(BaseType), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(IEnumerable<BaseType>), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
            Assert.AreSame(typeof(IEnumerable<DerivedType>), invocation.MethodBase.GetParameters().ElementAt(2).ParameterType);
            Assert.AreSame(typeof(List<string>[]), invocation.MethodBase.GetParameters().ElementAt(3).ParameterType);
        }

        public interface IInterface1<T1, U1, V1>
        {
            W1 TestMethod<W1, X1>(T1 a, U1 b, IEnumerable<X1> c, W1[] d)
                where W1 : V1
                where X1 : T1;
        }

        public interface IInterface2<T2, V2> : IInterface1<T2, IEnumerable<T2>, V2>
            where T2 : new()
        { }

        public interface IInterface3<V3> : IInterface2<BaseType, V3>
            where V3 : class
        { }

        public class Class3<Y> : IInterface3<Y>
            where Y : class
        {
            public Za TestMethod<Za, Zb>(BaseType a, IEnumerable<BaseType> b, IEnumerable<Zb> c, Za[] d)
                where Za : Y
                where Zb : BaseType
            {
                return default(Za);
            }
        }

        public class BaseType { }

        public class DerivedType : BaseType { }

        [TestMethod]
        public void CanInterceptConstrainedInheritedInterfaceMethod3()
        {
            var target = new ClassA2<BaseType>();

            IMethodInvocation invocation;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var proxy =
                Intercept.ThroughProxy<IInterfaceA2<BaseType>>(
                    target,
                    new InterfaceInterceptor(),
                    new[] { behavior });

            invocation = null;

            proxy.Test<HashSet<BaseType>, List<Guid>>(new ISet<BaseType>[0], new List<Guid>());

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(KeyValuePair<HashSet<BaseType>, IEnumerable<ISet<BaseType>>[]>), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(ISet<BaseType>[]), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(List<Guid>), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);

            invocation = null;

            proxy.CompareTo((object)this);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(int), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(1, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(object), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);

            invocation = null;

            proxy.CompareTo(Guid.Empty);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(int), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(1, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(Guid), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
        }

        public interface IInterfaceA1<TA1, TB1> : IComparable<TB1>
        {
            KeyValuePair<M1, IEnumerable<TA1>[]> Test<M1, M2>(TA1[] p1, M2 p2)
                where M1 : TA1
                where M2 : IEnumerable<TB1>;
        }

        public interface IInterfaceA2<TC2> : IInterfaceA1<ISet<TC2>, Guid>, IComparable
            where TC2 : class, new()
        { }

        public class ClassA2<TC2> : IInterfaceA2<TC2>
            where TC2 : class, new()
        {
            public KeyValuePair<M1, IEnumerable<ISet<TC2>>[]> Test<M1, M2>(ISet<TC2>[] p1, M2 p2)
                where M1 : ISet<TC2>
                where M2 : IEnumerable<Guid>
            {
                return default(KeyValuePair<M1, IEnumerable<ISet<TC2>>[]>);
            }

            public int CompareTo(object obj)
            {
                return 0;
            }

            public int CompareTo(Guid other)
            {
                return 0;
            }
        }

        [TestMethod]
        public void InterceptorCorrectlyRethrowsExceptions()
        {
            IUnityContainer container = new UnityContainer()
                .AddNewExtension<Interception>()
                .RegisterType(typeof(IFoo), typeof(Foo))
                .Configure<Interception>()
                    .SetInterceptorFor(typeof(IFoo), new InterfaceInterceptor())
                    .AddPolicy("AlwaysMatches")
                    .AddMatchingRule<AlwaysMatchingRule>()
                    .AddCallHandler<CallCountHandler>("callCount", new ContainerControlledLifetimeManager())
                .Interception
                .Container;

            IFoo myFoo = container.Resolve<IFoo>();

            try
            {
                myFoo.DoSomething();
                Assert.Fail("Should have thrown");
            }
            catch (Exception ex)
            {
                CallCountHandler handler = (CallCountHandler)(container.Resolve<ICallHandler>("callCount"));
                Assert.AreEqual(1, handler.CallCount);
                Assert.IsInstanceOfType(ex, typeof(FooCrashedException));

                var stackTrace = ex.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                Assert.IsTrue(stackTrace[0].Contains("DoSomethingLocal"), "stack trace is not full");
            }
        }

        public interface IFoo
        {
            void DoSomething();
        }

        public class Foo : IFoo
        {
            public virtual void DoSomething()
            {
                DoSomethingLocal();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            protected void DoSomethingLocal()
            {
                throw new FooCrashedException("oops");
            }
        }

        public class FooCrashedException : Exception
        {
            public FooCrashedException(string message) : base(message)
            {
            }
        }
    }
}
