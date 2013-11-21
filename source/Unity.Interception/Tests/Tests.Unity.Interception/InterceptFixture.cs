// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
                Intercept.ThroughProxyWithAdditionalInterfaces(
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
                Intercept.ThroughProxyWithAdditionalInterfaces(
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
                Intercept.ThroughProxyWithAdditionalInterfaces(
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
                Intercept.ThroughProxyWithAdditionalInterfaces<IInterface>(
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
            Intercept.ThroughProxyWithAdditionalInterfaces(
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
            Intercept.ThroughProxyWithAdditionalInterfaces(
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
            Intercept.ThroughProxyWithAdditionalInterfaces(
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
            Intercept.ThroughProxyWithAdditionalInterfaces(
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
            Intercept.ThroughProxyWithAdditionalInterfaces(
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
            Intercept.ThroughProxyWithAdditionalInterfaces(
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
            Intercept.ThroughProxyWithAdditionalInterfaces(
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
                Intercept.NewInstanceWithAdditionalInterfaces(
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
                Intercept.NewInstanceWithAdditionalInterfaces<BaseClass>(
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
                new IInterceptionBehavior[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterceptingNewInstanceWithANullInterceptorThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                null,
                new IInterceptionBehavior[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceOfTypeNotCompatibleWithTheInterceptorThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new RejectingInterceptor(),
                new IInterceptionBehavior[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterceptingNewInstanceWithANullSetOfInterceptionBehaviorsThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InterceptingNewInstanceWithANullSetOfAdditionalInterfacesThrows()
        {
            Intercept.NewInstanceWithAdditionalInterfaces(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[0],
                null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceWithASetOfAdditionalInterfacesWithNullElementsThrows()
        {
            Intercept.NewInstanceWithAdditionalInterfaces(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[0],
                new Type[] { null });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceWithASetOfAdditionalInterfacesWithNonInterfaceElementsThrows()
        {
            Intercept.NewInstanceWithAdditionalInterfaces(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[0],
                new Type[] { typeof(object) });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceWithASetOfAdditionalInterfacesWithGenericInterfaceElementsThrows()
        {
            Intercept.NewInstanceWithAdditionalInterfaces(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[0],
                new Type[] { typeof(IComparable<>) });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceWithASetOfBehaviorsWithNullElementsThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[] { null });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceWithASetOfBehaviorsWithElementReturningNullRequiredInterfacesThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[] 
                {
                    new DelegateInterceptionBehavior(null, () => null)
                });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceWithASetOfBehaviorsWithElementReturningRequiredInterfacesWithNullElementThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[] 
                {
                    new DelegateInterceptionBehavior(null, () => new Type[] { null })
                });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceWithASetOfBehaviorsWithElementReturningRequiredInterfacesWithNonInterfaceElementThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[] 
                {
                    new DelegateInterceptionBehavior(null, () => new Type[] { typeof(object) })
                });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterceptingNewInstanceWithASetOfBehaviorsWithElementReturningRequiredInterfacesWithGenericInterfaceElementThrows()
        {
            Intercept.NewInstance(
                typeof(BaseClass),
                new VirtualMethodInterceptor(),
                new IInterceptionBehavior[] 
                {
                    new DelegateInterceptionBehavior(null, () => new Type[] { typeof(IEnumerable<>) })
                });
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
                    new[] { interceptionBehavior });

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
