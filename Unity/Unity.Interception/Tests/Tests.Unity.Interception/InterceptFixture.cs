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
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    [TestClass]
    public class InterceptFixture
    {
        [TestMethod]
        public void CanInterceptTargetWithInstanceInterceptor()
        {
            bool invoked = false;
            IInterceptionBehavior interceptionBehavior =
                new DelegateInterceptionBehavior((mi, next) => { invoked = true; return mi.CreateMethodReturn(100); });

            IInterface proxy = (IInterface)
                Intercept.ThroughProxy(
                    typeof(IInterface),
                    new BaseClass(10),
                    new InterfaceInterceptor(),
                    new[] { interceptionBehavior },
                    Type.EmptyTypes);

            int value = proxy.DoSomething();

            Assert.AreEqual(100, value);
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void GeneratedProxyImplementsUserProvidedAdditionalInterfaces()
        {
            bool invoked = false;
            IInterceptionBehavior interceptionBehavior =
                new DelegateInterceptionBehavior((mi, next) => { invoked = true; return mi.CreateMethodReturn(100); });

            IInterface proxy = (IInterface)
                Intercept.ThroughProxy(
                    typeof(IInterface),
                    new BaseClass(10),
                    new InterfaceInterceptor(),
                    new[] { interceptionBehavior },
                    new[] { typeof(ISomeInterface) });

            int value = ((ISomeInterface)proxy).DoSomethingElse();

            Assert.AreEqual(100, value);
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void GeneratedProxyImplementsInterceptionBehaviorProvidedAdditionalInterfaces()
        {
            bool invoked = false;
            IInterceptionBehavior interceptionBehavior =
                new DelegateInterceptionBehavior(
                    (mi, next) => { invoked = true; return mi.CreateMethodReturn(100); },
                    () => new[] { typeof(ISomeInterface) });

            IInterface proxy = (IInterface)
                Intercept.ThroughProxy(
                    typeof(IInterface),
                    new BaseClass(10),
                    new InterfaceInterceptor(),
                    new[] { interceptionBehavior },
                    Type.EmptyTypes);

            int value = ((ISomeInterface)proxy).DoSomethingElse();

            Assert.AreEqual(100, value);
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void CanInterceptTargetWithInstanceInterceptorUsingGenericVersion()
        {
            bool invoked = false;
            IInterceptionBehavior interceptionBehavior =
                new DelegateInterceptionBehavior((mi, next) => { invoked = true; return mi.CreateMethodReturn(100); });

            IInterface proxy =
                Intercept.ThroughProxy<IInterface>(
                    new BaseClass(10),
                    new InterfaceInterceptor(),
                    new[] { interceptionBehavior },
                    Type.EmptyTypes);

            int value = proxy.DoSomething();

            Assert.AreEqual(100, value);
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterceptingANullTypeThrows()
        {
            Intercept.ThroughProxy(
                null,
                new BaseClass(),
                new InterfaceInterceptor(),
                new IInterceptionBehavior[0],
                Type.EmptyTypes);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterceptingANullTargetThrows()
        {
            Intercept.ThroughProxy(
                typeof(IInterface),
                null,
                new InterfaceInterceptor(),
                new IInterceptionBehavior[0],
                Type.EmptyTypes);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterceptingWithANullInterceptorThrows()
        {
            Intercept.ThroughProxy(
                typeof(IInterface),
                new BaseClass(),
                null,
                new IInterceptionBehavior[0],
                Type.EmptyTypes);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingTypesNotCompatibleWithTheInterceptorThrows()
        {
            Intercept.ThroughProxy(
                typeof(IInterface),
                new BaseClass(),
                new RejectingInterceptor(),
                new IInterceptionBehavior[0],
                Type.EmptyTypes);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterceptingWithANullSetOfInterceptionBehaviorsThrows()
        {
            Intercept.ThroughProxy(
                typeof(IInterface),
                new BaseClass(),
                new InterfaceInterceptor(),
                null,
                Type.EmptyTypes);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterceptingWithANullSetOfAdditionalInterfacesThrows()
        {
            Intercept.ThroughProxy(
                typeof(IInterface),
                new BaseClass(),
                new InterfaceInterceptor(),
                new IInterceptionBehavior[0],
                null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingWithASetOfAdditionalInterfacesIncludingNonInterfaceTypeThrows()
        {
            Intercept.ThroughProxy(
                typeof(IInterface),
                new BaseClass(),
                new InterfaceInterceptor(),
                new IInterceptionBehavior[0],
                new[] { typeof(object) });
        }

        [TestMethod]
        public void CanInterceptNewInstanceWithTypeInterceptor()
        {
            bool invoked = false;
            IInterceptionBehavior interceptionBehavior =
                new DelegateInterceptionBehavior((mi, next) => { invoked = true; return mi.CreateMethodReturn(100); });

            BaseClass instance = (BaseClass)
                Intercept.NewInstance(
                    typeof(BaseClass),
                    new VirtualMethodInterceptor(),
                    new[] { interceptionBehavior },
                    Type.EmptyTypes,
                    10);

            int value = instance.DoSomething();

            Assert.AreEqual(100, value);
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void CanInterceptNewInstanceWithTypeInterceptorUsingGenericVersion()
        {
            bool invoked = false;
            IInterceptionBehavior interceptionBehavior =
                new DelegateInterceptionBehavior((mi, next) => { invoked = true; return mi.CreateMethodReturn(100); });

            BaseClass instance =
                Intercept.NewInstance<BaseClass>(
                    new VirtualMethodInterceptor(),
                    new[] { interceptionBehavior },
                    Type.EmptyTypes,
                    10);

            int value = instance.DoSomething();

            Assert.AreEqual(100, value);
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterceptingNewInstanceOfANullTypeThrows()
        {
            Intercept.NewInstance(
                null,
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[0],
                Type.EmptyTypes);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterceptingNewInstanceWithANullInterceptorThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                null,
                new IInterceptionBehavior[0],
                Type.EmptyTypes);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceOfTypeNotCompatibleWithTheInterceptorThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new RejectingInterceptor(),
                new IInterceptionBehavior[0],
                Type.EmptyTypes);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterceptingNewInstanceWithANullSetOfInterceptionBehaviorsThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                null,
                Type.EmptyTypes);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterceptingNewInstanceWithANullSetOfAdditionalInterfacesThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[0],
                null);
        }

        [TestMethod]
        public void CanInterceptAbstractClassWithVirtualMethodInterceptor()
        {
            bool invoked = false;
            IInterceptionBehavior interceptionBehavior =
                new DelegateInterceptionBehavior((mi, next) => { invoked = true; return mi.CreateMethodReturn(100); });

            AbstractClass instance =
                Intercept.NewInstance<AbstractClass>(
                    new VirtualMethodInterceptor(),
                    new[] { interceptionBehavior },
                    Type.EmptyTypes);

            int value = instance.DoSomething();

            Assert.AreEqual(100, value);
            Assert.IsTrue(invoked);
        }

        public class BaseClass : IInterface
        {
            private readonly int value;

            public BaseClass()
                : this(0)
            { }

            public BaseClass(int value)
            {
                this.value = value;
            }

            public virtual int DoSomething()
            {
                return value;
            }
        }

        public interface IInterface
        {
            int DoSomething();
        }

        public interface ISomeInterface
        {
            int DoSomethingElse();
        }

        public class RejectingInterceptor : IInstanceInterceptor, ITypeInterceptor
        {
            public bool CanIntercept(Type t)
            {
                return false;
            }

            public IEnumerable<MethodImplementationInfo> GetInterceptableMethods(Type interceptedType, Type implementationType)
            {
                throw new NotImplementedException();
            }

            public IInterceptingProxy CreateProxy(Type t, object target, params Type[] additionalInterfaces)
            {
                throw new NotImplementedException();
            }

            public Type CreateProxyType(Type t, params Type[] additionalInterfaces)
            {
                throw new NotImplementedException();
            }
        }

        public abstract class AbstractClass
        {
            public abstract int DoSomething();
        }
    }
}
