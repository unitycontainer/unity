// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.CompilerServices;

namespace Unity.InterceptionExtension.Tests.VirtualMethodInterceptorTests
{
    /// <summary>
    /// Tests for the <see cref="VirtualMethodInterceptor"/> class.
    /// </summary>
    [TestClass]
    public partial class VirtualMethodInterceptorFixture
    {
        [TestMethod]
        public void CanInterceptBasicClass()
        {
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Assert.IsTrue(interceptor.CanIntercept(typeof(ClassWithNoVirtuals)));
        }

        [TestMethod]
        public void CantInterceptSealedClass()
        {
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Assert.IsFalse(interceptor.CanIntercept(typeof(CantOverride)));
        }

        [TestMethod]
        public void InterceptableClassWithNoVirtualMethodsReturnsEmptyMethodList()
        {
            List<MethodImplementationInfo> methods =
                new List<MethodImplementationInfo>(
                    new VirtualMethodInterceptor().GetInterceptableMethods(typeof(ClassWithNoVirtuals), typeof(ClassWithNoVirtuals)));
            Assert.AreEqual(0, methods.Count);
        }

        [TestMethod]
        public void CanInterceptMethods()
        {
            CallCountHandler h1 = new CallCountHandler();
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Type proxyType = interceptor.CreateProxyType(typeof(TwoOverrideableMethods));

            TwoOverrideableMethods instance =
                (TwoOverrideableMethods)Activator.CreateInstance(proxyType);
            PipelineManager manager = new PipelineManager();
            ((IInterceptingProxy)instance).AddInterceptionBehavior(new PolicyInjectionBehavior(manager));
            SetPipeline(manager, instance, "DoSomething", h1);

            instance.DoSomething();

            Assert.IsTrue(instance.DidSomething);
            Assert.AreEqual(1, h1.CallCount);
        }

        [TestMethod]
        public void CanInterceptProperties()
        {
            CallCountHandler getHandler = new CallCountHandler();
            CallCountHandler setHandler = new CallCountHandler();

            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Assert.IsTrue(interceptor.CanIntercept(typeof(OverrideableProperies)));

            Type proxyType = interceptor.CreateProxyType(typeof(OverrideableProperies));
            OverrideableProperies instance = (OverrideableProperies)Activator.CreateInstance(proxyType);

            PipelineManager manager = new PipelineManager();
            ((IInterceptingProxy)instance).AddInterceptionBehavior(new PolicyInjectionBehavior(manager));
            SetPipeline(manager, instance, "get_IntProperty", getHandler);
            SetPipeline(manager, instance, "set_IntProperty", setHandler);

            instance.IntProperty = 12;
            instance.IntProperty = 15;

            int total = 0;
            for (int i = 0; i < 5; ++i)
            {
                total += instance.IntProperty;
            }

            Assert.AreEqual(5 * 15, total);

            Assert.AreEqual(5, getHandler.CallCount);
            Assert.AreEqual(2, setHandler.CallCount);
        }

        [TestMethod]
        public void ReflectingOverProxyTypesGivesOnlyBaseProperties()
        {
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Assert.IsTrue(interceptor.CanIntercept(typeof(OverrideableProperies)));

            Type proxyType = interceptor.CreateProxyType(typeof(OverrideableProperies));
            PropertyInfo[] properties = proxyType.GetProperties();

            Assert.AreEqual(2, properties.Length);

            Assert.IsTrue(properties.All(pi => pi.DeclaringType == typeof(OverrideableProperies)));
        }

        [TestMethod]
        public void EventsAreIntercepted()
        {
            CallCountHandler fireHandler = new CallCountHandler();
            CallCountHandler addHandler = new CallCountHandler();

            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Assert.IsTrue(interceptor.CanIntercept(typeof(OverrideableProperies)));

            Type proxyType = interceptor.CreateProxyType(typeof(ClassWithEvent));
            ClassWithEvent instance = (ClassWithEvent)Activator.CreateInstance(proxyType);
            PipelineManager manager = new PipelineManager();
            ((IInterceptingProxy)instance).AddInterceptionBehavior(new PolicyInjectionBehavior(manager));
            SetPipeline(manager, instance, "add_MyEvent", addHandler);
            SetPipeline(manager, instance, "FireMyEvent", fireHandler);

            bool raised = false;
            instance.MyEvent += delegate { raised = true; };

            instance.FireMyEvent();
            instance.FireMyEvent();

            Assert.IsTrue(raised);

            Assert.AreEqual(2, fireHandler.CallCount);
            Assert.AreEqual(1, addHandler.CallCount);
        }

        [TestMethod]
        public void ReflectionOverInheritedClassesReturnsProperAttributes()
        {
            Type targetType = typeof(OverriddenProperties);

            PropertyInfo[] baseProperties = typeof(OverrideableProperies).GetProperties();
            PropertyInfo[] properties = targetType.GetProperties();

            Assert.AreEqual(baseProperties.Length, properties.Length);

            PropertyInfo stringProperty = targetType.GetProperty("StringProperty");
            Attribute[] attrs = Attribute.GetCustomAttributes(stringProperty, typeof(MultiAttribute));
            Assert.AreEqual(2, attrs.Length);
        }

        [TestMethod]
        public void CanInterceptTypeWithNonDefaultCtor()
        {
            CallCountHandler h1 = new CallCountHandler();
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Type proxyType = interceptor.CreateProxyType(typeof(ClassWithNonDefaultCtor));

            ClassWithNonDefaultCtor instance =
                (ClassWithNonDefaultCtor)Activator.CreateInstance(proxyType, "arg-value");

            PipelineManager manager = new PipelineManager();
            ((IInterceptingProxy)instance).AddInterceptionBehavior(new PolicyInjectionBehavior(manager));
            SetPipeline(manager, instance, "GetArg", h1);

            Assert.AreEqual("arg-value", instance.GetArg());

            Assert.AreEqual(1, h1.CallCount);
        }

        [TestMethod]
        public void CanGenerateDerivedTypeForAbstractType()
        {
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Type proxyType = interceptor.CreateProxyType(typeof(AbstractClassWithPublicConstructor));

            Assert.AreSame(typeof(AbstractClassWithPublicConstructor), proxyType.BaseType);
        }

        [TestMethod]
        public void CanCreateInstanceForGeneratedTypeForAbstractType()
        {
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Type proxyType = interceptor.CreateProxyType(typeof(AbstractClassWithPublicConstructor));

            Activator.CreateInstance(proxyType);
        }

        [TestMethod]
        public void CanInvokeVirtualMethodOnInterceptedAbstractTypeInstance()
        {
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Type proxyType = interceptor.CreateProxyType(typeof(AbstractClassWithPublicConstructor));

            AbstractClassWithPublicConstructor instance =
                (AbstractClassWithPublicConstructor)Activator.CreateInstance(proxyType);
            bool invoked = false;
            ((IInterceptingProxy)instance).AddInterceptionBehavior(
                new DelegateInterceptionBehavior((mi, gn) => { invoked = true; return gn()(mi, gn); }));

            int value = instance.VirtualMethod();

            Assert.AreEqual(10, value);
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void InvokingAbstractMethodFromInterceptedAbstracTypeInstanceThrows()
        {
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Type proxyType = interceptor.CreateProxyType(typeof(AbstractClassWithPublicConstructor));

            AbstractClassWithPublicConstructor instance =
                (AbstractClassWithPublicConstructor)Activator.CreateInstance(proxyType);

            try
            {
                instance.AbstractMethod();
                Assert.Fail("should have thrown");
            }
            catch (NotImplementedException)
            {
            }
        }

        [TestMethod]
        public void CanCreateInstanceForGeneratedTypeForAbstractTypeWithProtectedConstructor()
        {
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Type proxyType = interceptor.CreateProxyType(typeof(AbstractClassWithProtectedConstructor));

            Activator.CreateInstance(proxyType, 100);
        }

        [TestMethod]
        public void CanInvokeVirtualMethodOnInterceptedAbstractTypeWithProtectedConstructorInstance()
        {
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Type proxyType = interceptor.CreateProxyType(typeof(AbstractClassWithProtectedConstructor));

            AbstractClassWithProtectedConstructor instance =
                (AbstractClassWithProtectedConstructor)Activator.CreateInstance(proxyType, 200);
            bool invoked = false;
            ((IInterceptingProxy)instance).AddInterceptionBehavior(
                new DelegateInterceptionBehavior((mi, gn) => { invoked = true; return gn()(mi, gn); }));

            int value = instance.VirtualMethod();

            Assert.AreEqual(200, value);
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void CanInvokeOverridenAbstractMethodMethodOnInterceptedDerivedFromAbstractTypeInstance()
        {
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Type proxyType = interceptor.CreateProxyType(typeof(DerivedFromAbstractClassWithPublicConstructor));

            DerivedFromAbstractClassWithPublicConstructor instance =
                (DerivedFromAbstractClassWithPublicConstructor)Activator.CreateInstance(proxyType);
            bool invoked = false;
            ((IInterceptingProxy)instance).AddInterceptionBehavior(
                new DelegateInterceptionBehavior((mi, gn) => { invoked = true; return gn()(mi, gn); }));

            int value = instance.AbstractMethod();

            Assert.AreEqual(200, value);
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void CanInvokeOverridenAbstractMethodMethodOnInterceptedAbstractDerivedFromAbstractTypeInstance()
        {
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Type proxyType = interceptor.CreateProxyType(typeof(AbstractDerivedFromAbstractClassWithPublicConstructor));

            AbstractDerivedFromAbstractClassWithPublicConstructor instance =
                (AbstractDerivedFromAbstractClassWithPublicConstructor)Activator.CreateInstance(proxyType);
            bool invoked = false;
            ((IInterceptingProxy)instance).AddInterceptionBehavior(
                new DelegateInterceptionBehavior((mi, gn) => { invoked = true; return gn()(mi, gn); }));

            int value = instance.AbstractMethod();

            Assert.AreEqual(200, value);
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void AssortedParameterKindsAreProperlyHandled()
        {
            var interceptor = new VirtualMethodInterceptor();

            var type = interceptor.CreateProxyType(typeof(AssortedParameterKindsAreProperlyHandledHelper.TypeWithAssertedParameterKinds));

            IInterceptingProxy proxy = Activator.CreateInstance(type) as IInterceptingProxy;

            AssortedParameterKindsAreProperlyHandledHelper.PerformTest(proxy);
        }

        private void SetPipeline(PipelineManager manager, object instance, string methodName, params ICallHandler[] handlers)
        {
            HandlerPipeline pipeline = new HandlerPipeline(handlers);
            MethodInfo targetMethod = instance.GetType().BaseType.GetMethod(methodName);
            IInterceptingProxy proxy = (IInterceptingProxy)instance;
            manager.SetPipeline(targetMethod, pipeline);
        }

        //[TestMethod]
        //public void CanInterceptGenericInterfaceWithInterfaceConstraint()
        //{
        //    var target = new ConstrainedImpl();
        //
        //    bool behaviorWasCalled = false;
        //
        //    var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
        //    {
        //        behaviorWasCalled = true;
        //        return getNext()(inputs, getNext);
        //    });
        //
        //    var proxy = Intercept.ThroughProxy<IConstrainedInterface<IBaseInterface>>(
        //        target, new InterfaceInterceptor(),
        //        new[] { behavior });
        //
        //    proxy.SomeMethod();
        //
        //    Assert.IsTrue(behaviorWasCalled);
        //
        //}

        [TestMethod]
        public void CanInterceptNonGenericMethodOnNonGenericInterface()
        {
            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var instance =
                Intercept.NewInstance<NonGenericClass>(
                    new VirtualMethodInterceptor(),
                    new[] { behavior });

            instance.NonGenericMethod(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodOnNonGenericInterface()
        {
            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var instance =
                Intercept.NewInstance<NonGenericClass>(
                    new VirtualMethodInterceptor(),
                    new[] { behavior });

            instance.GenericMethod<string>(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodWithConstraintOnNonGenericInterface()
        {
            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var instance =
                Intercept.NewInstance<NonGenericClass>(
                    new VirtualMethodInterceptor(),
                    new[] { behavior });

            instance.GenericMethodWithConstraints<string>(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptNonGenericMethodOnGenericInterface()
        {
            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var instance =
                Intercept.NewInstance<GenericClass<IEnumerable>>(
                    new VirtualMethodInterceptor(),
                    new[] { behavior });

            instance.NonGenericMethod(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodOnGenericInterface()
        {
            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var instance =
                Intercept.NewInstance<GenericClass<IEnumerable>>(
                    new VirtualMethodInterceptor(),
                    new[] { behavior });

            instance.GenericMethod<string>(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodWithConstraintOnGenericInterface()
        {
            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var instance =
                Intercept.NewInstance<GenericClass<IEnumerable>>(
                    new VirtualMethodInterceptor(),
                    new[] { behavior });

            instance.GenericMethodWithConstraints<string>(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodWithConstraintRelatedToInterfaceOnGenericInterface()
        {
            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var instance =
                Intercept.NewInstance<GenericClass<IEnumerable>>(
                    new VirtualMethodInterceptor(),
                    new[] { behavior });

            instance.GenericMethodWithConstraintsOnTheInterfaceParameter<string>(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptNonGenericMethodOnGenericInterfaceWithConstraint()
        {
            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var instance =
                Intercept.NewInstance<GenericClassWithConstraint<IEnumerable>>(
                    new VirtualMethodInterceptor(),
                    new[] { behavior });

            instance.NonGenericMethod(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodOnGenericInterfaceWithConstraint()
        {
            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var instance =
                Intercept.NewInstance<GenericClassWithConstraint<IEnumerable>>(
                    new VirtualMethodInterceptor(),
                    new[] { behavior });

            instance.GenericMethod<string>(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodWithConstraintOnGenericInterfaceWithConstraint()
        {
            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var instance =
                Intercept.NewInstance<GenericClassWithConstraint<IEnumerable>>(
                    new VirtualMethodInterceptor(),
                    new[] { behavior });

            instance.GenericMethodWithConstraints<string>(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        [TestMethod]
        public void CanInterceptGenericMethodWithConstraintRelatedToInterfaceOnGenericInterfaceWithConstraint()
        {
            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var instance =
                Intercept.NewInstance<GenericClassWithConstraint<IEnumerable>>(
                    new VirtualMethodInterceptor(),
                    new[] { behavior });

            instance.GenericMethodWithConstraintsOnTheInterfaceParameter<string>(null, null);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(string), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(IEnumerable), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(string), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);
        }

        public class NonGenericClass
        {
            public virtual string NonGenericMethod(IEnumerable param1, string param2)
            {
                return null;
            }

            public virtual T GenericMethod<T>(IEnumerable param1, T param2)
            {
                return default(T);
            }

            public virtual T GenericMethodWithConstraints<T>(IEnumerable param1, T param2) where T : class
            {
                return default(T);
            }
        }

        public class GenericClass<T1>
        {
            public virtual string NonGenericMethod(T1 param1, string param2)
            {
                return null;
            }

            public virtual T GenericMethod<T>(T1 param1, T param2)
            {
                var handle = MethodBase.GetCurrentMethod().MethodHandle;

                return default(T);
            }

            public virtual T GenericMethodWithConstraints<T>(T1 param1, T param2) where T : class
            {
                return default(T);
            }

            public virtual T GenericMethodWithConstraintsOnTheInterfaceParameter<T>(T1 param1, T param2) where T : T1
            {
                return default(T);
            }
        }

        public class GenericClassWithConstraint<T1>
            where T1 : class
        {
            public virtual string NonGenericMethod(T1 param1, string param2)
            {
                return null;
            }

            public virtual T GenericMethod<T>(T1 param1, T param2)
            {
                return default(T);
            }

            public virtual T GenericMethodWithConstraints<T>(T1 param1, T param2) where T : class
            {
                return default(T);
            }

            public virtual T GenericMethodWithConstraintsOnTheInterfaceParameter<T>(T1 param1, T param2) where T : T1
            {
                return default(T);
            }
        }

        [TestMethod]
        public void CanInterceptConstrainedInheritedInterfaceMethod()
        {
            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var instance =
                Intercept.NewInstance<DerivedNonGenericClass>(
                    new VirtualMethodInterceptor(),
                    new[] { behavior });

            instance.Test<List<string>>();

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(List<string>), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(0, invocation.MethodBase.GetParameters().Count());
        }

        public class BaseGenericClass<TBaseType>
        {
            public virtual TDerivedType Test<TDerivedType>() where TDerivedType : TBaseType
            {
                return default(TDerivedType);
            }
        }

        public class DerivedNonGenericClass : BaseGenericClass<ICollection<string>>
        {
        }

        [TestMethod]
        public void CanInterceptConstrainedInheritedInterfaceMethod2()
        {
            IMethodInvocation invocation = null;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var instance =
                Intercept.NewInstance<Class3<ICollection<string>>>(
                    new VirtualMethodInterceptor(),
                    new[] { behavior });

            instance.TestMethod<List<string>, DerivedType>(
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

        public class Class1<T1, U1, V1>
        {
            public virtual W1 TestMethod<W1, X1>(T1 a, U1 b, IEnumerable<X1> c, W1[] d)
                where W1 : V1
                where X1 : T1
            {
                return default(W1);
            }
        }

        public class Class2<T2, V2> : Class1<T2, IEnumerable<T2>, V2>
            where T2 : new()
        { }

        public class Class3<V3> : Class2<BaseType, V3>
            where V3 : class
        { }

        public class BaseType { }

        public class DerivedType : BaseType { }

        [TestMethod]
        public void CanInterceptConstrainedInheritedInterfaceMethod3()
        {
            IMethodInvocation invocation;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var instance =
                Intercept.NewInstance<ClassA2<BaseType>>(
                    new VirtualMethodInterceptor(),
                    new[] { behavior });

            invocation = null;

            instance.Test<HashSet<BaseType>, List<Guid>>(new ISet<BaseType>[0], new List<Guid>());

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(KeyValuePair<HashSet<BaseType>, IEnumerable<ISet<BaseType>>[]>), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(2, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(ISet<BaseType>[]), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
            Assert.AreSame(typeof(List<Guid>), invocation.MethodBase.GetParameters().ElementAt(1).ParameterType);

            invocation = null;

            instance.CompareTo((object)this);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(int), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(1, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(object), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);

            invocation = null;

            ((IComparable<Guid>)instance).CompareTo(Guid.Empty);

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(int), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(1, invocation.MethodBase.GetParameters().Count());
            Assert.AreSame(typeof(Guid), invocation.MethodBase.GetParameters().ElementAt(0).ParameterType);
        }

        public class ClassA1<TA1, TB1> : IComparable<TB1>
        {
            public virtual KeyValuePair<M1, IEnumerable<TA1>[]> Test<M1, M2>(TA1[] p1, M2 p2)
                where M1 : TA1
                where M2 : IEnumerable<TB1>
            {
                return default(KeyValuePair<M1, IEnumerable<TA1>[]>);
            }

            public virtual int CompareTo(TB1 other)
            {
                return 0;
            }
        }

        public class ClassA2<TC2> : ClassA1<ISet<TC2>, Guid>, IComparable
            where TC2 : class, new()
        {
            public virtual int CompareTo(object obj)
            {
                return 0;
            }
        }

        [TestMethod]
        public void CanInterceptVirtualMethodsOnGenericNestedClassInGenericClass()
        {
            IMethodInvocation invocation;

            var behavior = new DelegateInterceptionBehavior((inputs, getNext) =>
            {
                invocation = inputs;
                return getNext()(inputs, getNext);
            });

            var instance =
                Intercept.NewInstance<GenericClassWithNestedGenericClass<BaseType>.GenericNestedClass<DerivedType>>(
                    new VirtualMethodInterceptor(),
                    new[] { behavior });

            invocation = null;

            instance.Test<List<DerivedType>>();

            Assert.IsNotNull(invocation);
            Assert.AreSame(typeof(List<DerivedType>[]), ((MethodInfo)invocation.MethodBase).ReturnType);
            Assert.AreEqual(0, invocation.MethodBase.GetParameters().Count());
        }

        public class GenericClassWithNestedGenericClass<T>
        {
            public class GenericNestedClass<U> where U : T
            {
                public virtual X[] Test<X>() where X : IEnumerable<U>
                {
                    return default(X[]);
                }
            }
        }

        [TestMethod]
        public void InterceptorCorrectlyRethrowsExceptions()
        {
            IUnityContainer container = new UnityContainer()
                .AddNewExtension<Interception>()
                .RegisterType(typeof(IFoo), typeof(Foo))
                .Configure<Interception>()
                    .SetInterceptorFor(typeof(Foo), new VirtualMethodInterceptor())
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

    public partial class DerivedTypeCreator
    {
        public void CreateType(Type baseType)
        {
            new VirtualMethodInterceptor().CreateProxyType(baseType);
        }
    }

    public class ClassWithNonDefaultCtor
    {
        private string arg;
        
        public ClassWithNonDefaultCtor(string arg)
        {
            this.arg = arg;
        }

        public virtual string GetArg()
        {
            return arg;
        }
    }

    public class ClassWithNoVirtuals
    {
        public void CannotOverrideMe()
        {
        }
    }

    public sealed class CantOverride
    {
    }

    public class TwoOverrideableMethods
    {
        public bool DidSomething;

        public virtual void DoSomething()
        {
            DidSomething = true;
        }

        public virtual string Swizzle(int a, float b, decimal c)
        {
            return ((decimal)(a * b) + c).ToString();
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    internal sealed class MultiAttribute : Attribute
    {
        private readonly string name;

        public MultiAttribute(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return this.name; }
        }
    }

    public class OverrideableProperies
    {
        private int intProperty;
        private string stringProperty;

        public virtual int IntProperty
        {
            get { return intProperty; }
            set { intProperty = value; }
        }

        [Multi("One")]
        [Multi("Two")]
        public virtual string StringProperty
        {
            get { return stringProperty; }
            set { stringProperty = value; }
        }
    }

    public class ClassWithEvent
    {
        public virtual event EventHandler MyEvent;

        public virtual void FireMyEvent()
        {
            EventHandler evt = MyEvent;
            if (evt != null)
            {
                evt(this, EventArgs.Empty);
            }
        }
    }

    public class OverriddenProperties : OverrideableProperies
    {
        public override string StringProperty
        {
            get
            {
                return base.StringProperty;
            }
            set
            {
                base.StringProperty = value;
            }
        }

        public override int IntProperty
        {
            get
            {
                return base.IntProperty;
            }
            set
            {
                base.IntProperty = value;
            }
        }
    }

    public abstract class AbstractClassWithPublicConstructor
    {
        public AbstractClassWithPublicConstructor()
        {
        }

        public abstract int AbstractMethod();

        public virtual int VirtualMethod()
        {
            return 10;
        }
    }

    public class DerivedFromAbstractClassWithPublicConstructor : AbstractClassWithPublicConstructor
    {
        public override int AbstractMethod()
        {
            return 200;
        }
    }

    public abstract class AbstractDerivedFromAbstractClassWithPublicConstructor : AbstractClassWithPublicConstructor
    {
        public override int AbstractMethod()
        {
            return 200;
        }
    }

    public abstract class AbstractClassWithProtectedConstructor
    {
        private int value;

        protected AbstractClassWithProtectedConstructor(int value)
        {
            this.value = value;
        }

        public abstract int AbstractMethod();

        public virtual int VirtualMethod()
        {
            return value;
        }
    }
}
