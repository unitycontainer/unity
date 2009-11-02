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
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.VirtualMethodInterceptorTests
{
    /// <summary>
    /// Tests for the <see cref="VirtualMethodInterceptor"/> class.
    /// </summary>
    [TestClass]
    public class VirtualMethodInterceptorFixture
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
        public void GeneratedTypeForAbstractIsVerifiable()
        {
            PermissionSet permissionSet = new PermissionSet(PermissionState.None);
            permissionSet.AddPermission(
                new SecurityPermission(
                    SecurityPermissionFlag.Execution
                    | SecurityPermissionFlag.ControlPolicy
                    | SecurityPermissionFlag.ControlPrincipal));
            permissionSet.AddPermission(new FileIOPermission(PermissionState.Unrestricted));

            AppDomain domain =
                AppDomain.CreateDomain(
                    "isolated",
                    AppDomain.CurrentDomain.Evidence,
                    AppDomain.CurrentDomain.SetupInformation,
                    permissionSet);

            DerivedTypeCreator creator = (DerivedTypeCreator)
                domain.CreateInstanceAndUnwrap(
                    typeof(DerivedTypeCreator).Assembly.FullName,
                    typeof(DerivedTypeCreator).FullName);

            creator.CreateType(typeof(AbstractClassWithPublicConstructor));
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

        private void SetPipeline(PipelineManager manager, object instance, string methodName, params ICallHandler[] handlers)
        {
            HandlerPipeline pipeline = new HandlerPipeline(handlers);
            MethodInfo targetMethod = instance.GetType().BaseType.GetMethod(methodName);
            IInterceptingProxy proxy = (IInterceptingProxy)instance;
            manager.SetPipeline(targetMethod, pipeline);

        }
    }

    // Some test classes for interception


    public class DerivedTypeCreator : MarshalByRefObject
    {
        public void CreateType(Type baseType)
        {
            new VirtualMethodInterceptor().CreateProxyType(baseType);
        }
    }

    public class ClassWithNonDefaultCtor
    {
        string arg;
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
            get { return name; }
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
