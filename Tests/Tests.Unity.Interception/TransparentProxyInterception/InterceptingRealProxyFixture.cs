// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.TransparaentProxyInterception.Tests
{
    [TestClass]
    public class InterceptingRealProxyFixture
    {
        [TestMethod]
        public void CanProxyMBROMethods()
        {
            MBROWithOneMethod original = new MBROWithOneMethod();
            MBROWithOneMethod proxy = new InterceptingRealProxy(original, typeof(MBROWithOneMethod))
                .GetTransparentProxy() as MBROWithOneMethod;

            Assert.IsNotNull(proxy);
        }

        [TestMethod]
        public void ProxyImplementsIInterceptingProxy()
        {
            MBROWithOneMethod original = new MBROWithOneMethod();
            MBROWithOneMethod proxy = new InterceptingRealProxy(original, typeof(MBROWithOneMethod))
                .GetTransparentProxy() as MBROWithOneMethod;

            Assert.IsNotNull(proxy as IInterceptingProxy);
        }

        [TestMethod]
        public void CanInterceptMethodsThroughProxy()
        {
            CallCountInterceptionBehavior interceptor = new CallCountInterceptionBehavior();

            MBROWithOneMethod original = new MBROWithOneMethod();
            MBROWithOneMethod intercepted = new InterceptingRealProxy(original, typeof(MBROWithOneMethod))
                .GetTransparentProxy() as MBROWithOneMethod;

            IInterceptingProxy proxy = (IInterceptingProxy)intercepted;
            proxy.AddInterceptionBehavior(interceptor);

            int result = intercepted.DoSomething(5);

            Assert.AreEqual(5 * 3, result);
            Assert.AreEqual(1, interceptor.CallCount);
        }

        [TestMethod]
        public void ProxyInterceptsAddingAHandlerToAnEvent()
        {
            // arrange
            CallCountInterceptionBehavior interceptor
                = new CallCountInterceptionBehavior();

            MBROWithAnEvent original = new MBROWithAnEvent();
            MBROWithAnEvent intercepted = new InterceptingRealProxy(original, typeof(MBROWithAnEvent))
                .GetTransparentProxy() as MBROWithAnEvent;

            ((IInterceptingProxy)intercepted).AddInterceptionBehavior(interceptor);

            // act
            intercepted.SomeEvent += (s, a) => { };

            // assert
            Assert.AreEqual(1, interceptor.CallCount);
        }

        [TestMethod]
        public void ProxySendsOriginalWhenRaisingEvent()
        {
            // arrange
            CallCountInterceptionBehavior interceptor = new CallCountInterceptionBehavior();

            MBROWithAnEvent original = new MBROWithAnEvent();
            MBROWithAnEvent intercepted = new InterceptingRealProxy(original, typeof(MBROWithAnEvent))
                .GetTransparentProxy() as MBROWithAnEvent;

            ((IInterceptingProxy)intercepted).AddInterceptionBehavior(interceptor);
            object sender = null;
            intercepted.SomeEvent += (s, a) => { sender = s; };

            // act
            intercepted.TriggerIt();

            // assert
            Assert.AreSame(original, sender);
            Assert.AreEqual(2, interceptor.CallCount);  // adding + calling TriggerIt
        }

        [TestMethod]
        public void CanCreateProxyWithAdditionalInterfaces()
        {
            MBROWithOneMethod original = new MBROWithOneMethod();
            MBROWithOneMethod proxy =
                new InterceptingRealProxy(original, typeof(MBROWithOneMethod), typeof(InterfaceOne))
                .GetTransparentProxy() as MBROWithOneMethod;

            Assert.IsTrue(proxy is InterfaceOne);
        }

        [TestMethod]
        public void InvokingMethodOnAdditionalInterfaceThrowsIfNotHandledByInterceptor()
        {
            MBROWithOneMethod original = new MBROWithOneMethod();
            InterfaceOne proxy =
                new InterceptingRealProxy(original, typeof(MBROWithOneMethod), typeof(InterfaceOne))
                .GetTransparentProxy() as InterfaceOne;

            try
            {
                proxy.Something();
                Assert.Fail("should have thrown");
            }
            catch (InvalidOperationException)
            {
                // expected
            }
        }

        [TestMethod]
        public void CanSuccessfullyInvokeAnAdditionalInterfaceMethodIfAnInterceptorDoesNotForwardTheCall()
        {
            MBROWithOneMethod original = new MBROWithOneMethod();
            InterfaceOne proxy =
                new InterceptingRealProxy(original, typeof(MBROWithOneMethod), typeof(InterfaceOne))
                .GetTransparentProxy() as InterfaceOne;
            bool invoked = false;
            ((IInterceptingProxy)proxy).AddInterceptionBehavior(
                new DelegateInterceptionBehavior(
                    (input, getNext) => { invoked = true; return input.CreateMethodReturn(null); }));

            proxy.Something();

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void CanImplementINotifyPropertyChanged()
        {
            MBROWithOneProperty target = new MBROWithOneProperty();
            MBROWithOneProperty proxy =
                new InterceptingRealProxy(target, typeof(MBROWithOneProperty), typeof(INotifyPropertyChanged))
                .GetTransparentProxy() as MBROWithOneProperty;
            ((IInterceptingProxy)proxy).AddInterceptionBehavior(new NaiveINotifyPropertyChangedInterceptionBehavior());

            string changeProperty;
            PropertyChangedEventHandler handler = (sender, args) => changeProperty = args.PropertyName;

            changeProperty = null;
            ((INotifyPropertyChanged)proxy).PropertyChanged += handler;

            proxy.TheProperty = 100;

            Assert.AreEqual(100, proxy.TheProperty);
            Assert.AreEqual("TheProperty", changeProperty);

            changeProperty = null;
            ((INotifyPropertyChanged)proxy).PropertyChanged -= handler;

            proxy.TheProperty = 200;

            Assert.AreEqual(200, proxy.TheProperty);
            Assert.AreEqual(null, changeProperty);
        }

        [TestMethod]
        public void CanImplementINotifyPropertyChangedThroughInterface()
        {
            ObjectWithOnePropertyForImplicitlyImplementedInterface target = new ObjectWithOnePropertyForImplicitlyImplementedInterface();
            IInterfaceWithOneProperty proxy =
                new InterceptingRealProxy(target, typeof(IInterfaceWithOneProperty), typeof(INotifyPropertyChanged))
                .GetTransparentProxy() as IInterfaceWithOneProperty;
            ((IInterceptingProxy)proxy).AddInterceptionBehavior(new NaiveINotifyPropertyChangedInterceptionBehavior());

            string changeProperty;
            PropertyChangedEventHandler handler = (sender, args) => changeProperty = args.PropertyName;

            changeProperty = null;
            ((INotifyPropertyChanged)proxy).PropertyChanged += handler;

            proxy.TheProperty = 100;

            Assert.AreEqual(100, proxy.TheProperty);
            Assert.AreEqual("TheProperty", changeProperty);

            changeProperty = null;
            ((INotifyPropertyChanged)proxy).PropertyChanged -= handler;

            proxy.TheProperty = 200;

            Assert.AreEqual(200, proxy.TheProperty);
            Assert.AreEqual(null, changeProperty);
        }

        [TestMethod]
        public void CanImplementINotifyPropertyChangedThroughExplicitInterface()
        {
            ObjectWithOnePropertyForExplicitlyImplementedInterface target = new ObjectWithOnePropertyForExplicitlyImplementedInterface();
            IInterfaceWithOneProperty proxy =
                new InterceptingRealProxy(target, typeof(IInterfaceWithOneProperty), typeof(INotifyPropertyChanged))
                .GetTransparentProxy() as IInterfaceWithOneProperty;
            ((IInterceptingProxy)proxy).AddInterceptionBehavior(new NaiveINotifyPropertyChangedInterceptionBehavior());

            string changeProperty;
            PropertyChangedEventHandler handler = (sender, args) => changeProperty = args.PropertyName;

            changeProperty = null;
            ((INotifyPropertyChanged)proxy).PropertyChanged += handler;

            proxy.TheProperty = 100;

            Assert.AreEqual(100, proxy.TheProperty);
            Assert.AreEqual("TheProperty", changeProperty);

            changeProperty = null;
            ((INotifyPropertyChanged)proxy).PropertyChanged -= handler;

            proxy.TheProperty = 200;

            Assert.AreEqual(200, proxy.TheProperty);
            Assert.AreEqual(null, changeProperty);
        }

        [TestMethod]
        public void CanInterceptGenericMethodOnInterface()
        {
            var interceptor = new CallCountInterceptionBehavior();

            var original = new ObjectWithGenericMethod();
            var intercepted = new InterceptingRealProxy(original, typeof(IInterfaceWithGenericMethod))
                .GetTransparentProxy() as IInterfaceWithGenericMethod;

            IInterceptingProxy proxy = (IInterceptingProxy)intercepted;
            proxy.AddInterceptionBehavior(interceptor);

            var result = intercepted.GetTypeName(6);

            Assert.AreEqual("Int32", result);
            Assert.AreEqual(1, interceptor.CallCount);
        }

        internal class MBROWithOneMethod : MarshalByRefObject
        {
            public int DoSomething(int i)
            {
                return i * 3;
            }
        }

        internal interface InterfaceOne
        {
            void Something();
        }

        internal class MBROWithInterface : MarshalByRefObject, InterfaceOne
        {
            public void Something()
            {
            }
        }

        public class MBROWithAnEvent : MarshalByRefObject
        {
            public event EventHandler<EventArgs> SomeEvent;

            public void TriggerIt()
            {
                this.SomeEvent(this, new EventArgs());
            }
        }

        internal class MBROWithOneProperty : MarshalByRefObject
        {
            public int TheProperty { get; set; }
        }

        internal class ObjectWithOnePropertyForImplicitlyImplementedInterface : IInterfaceWithOneProperty
        {
            public int TheProperty { get; set; }
        }

        internal class ObjectWithOnePropertyForExplicitlyImplementedInterface : IInterfaceWithOneProperty
        {
            int IInterfaceWithOneProperty.TheProperty { get; set; }
        }

        internal interface IInterfaceWithOneProperty
        {
            int TheProperty { get; set; }
        }

        internal interface IInterfaceWithGenericMethod
        {
            string GetTypeName<T>(T argument);
        }

        internal class ObjectWithGenericMethod : IInterfaceWithGenericMethod
        {
            #region IInterfaceWithGenericMethod Members

            public string GetTypeName<T>(T argument)
            {
                return argument.GetType().Name;
            }

            #endregion
        }
    }
}
