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
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestSupport.Unity;

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
            Assert.IsTrue(interceptor.CanIntercept(typeof (ClassWithNoVirtuals)));
        }

        [TestMethod]
        public void CantInterceptSealedClass()
        {
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Assert.IsFalse(interceptor.CanIntercept(typeof (CantOverride)));
            
        }

        [TestMethod]
        public void InterceptableClassWithNoVirtualMethodsReturnsEmptyMethodList()
        {
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            List<MethodImplementationInfo> methods =
                new List<MethodImplementationInfo>(interceptor.GetInterceptableMethods(typeof(ClassWithNoVirtuals), typeof(ClassWithNoVirtuals)));
            Assert.AreEqual(0, methods.Count);
        }

        [TestMethod]
        public void CanInterceptMethods()
        {
            CallCountHandler h1 = new CallCountHandler();
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Type proxyType = interceptor.CreateProxyType(typeof (TwoOverrideableMethods));

            TwoOverrideableMethods instance =
                (TwoOverrideableMethods) Activator.CreateInstance(proxyType);
            SetPipeline(instance, "DoSomething", h1);

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

            Type proxyType = interceptor.CreateProxyType(typeof (OverrideableProperies));
            OverrideableProperies instance = (OverrideableProperies) Activator.CreateInstance(proxyType);

            SetPipeline(instance, "get_IntProperty", getHandler);
            SetPipeline(instance, "set_IntProperty", setHandler);

            instance.IntProperty = 12;
            instance.IntProperty = 15;

            int total = 0;
            for(int i = 0; i < 5; ++i)
            {
                total += instance.IntProperty;
            }

            Assert.AreEqual(5*15, total);

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
            Assert.IsTrue(Sequence.ForAll(properties, delegate(PropertyInfo pi) { return pi.DeclaringType == typeof(OverrideableProperies); }));
        }

        [TestMethod]
        public void EventsAreNotIntercepted()
        {
            CallCountHandler fireHandler = new CallCountHandler();
            CallCountHandler addHandler = new CallCountHandler();

            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Assert.IsTrue(interceptor.CanIntercept(typeof(OverrideableProperies)));

            Type proxyType = interceptor.CreateProxyType(typeof (ClassWithEvent));
            ClassWithEvent instance = (ClassWithEvent) Activator.CreateInstance(proxyType);
            SetPipeline(instance, "add_MyEvent", addHandler);
            SetPipeline(instance, "FireMyEvent", fireHandler);


            bool raised = false;
            instance.MyEvent += delegate { raised = true; };

            instance.FireMyEvent();
            instance.FireMyEvent();

            Assert.IsTrue(raised);

            Assert.AreEqual(2, fireHandler.CallCount);
            Assert.AreEqual(0, addHandler.CallCount);

        }

        [TestMethod]
        public void ReflectionOverInheritedClassesReturnsProperAttributes()
        {
            Type targetType = typeof (OverriddenProperties);

            PropertyInfo[] baseProperties = typeof (OverrideableProperies).GetProperties();
            PropertyInfo[] properties = targetType.GetProperties();

            Assert.AreEqual(baseProperties.Length, properties.Length);

            PropertyInfo stringProperty = targetType.GetProperty("StringProperty");
            Attribute[] attrs = Attribute.GetCustomAttributes(stringProperty, typeof (MultiAttribute));
            Assert.AreEqual(2, attrs.Length);
        }


        private void SetPipeline(object instance, string methodName, params ICallHandler[] handlers)
        {
            HandlerPipeline pipeline = new HandlerPipeline(handlers);
            MethodInfo targetMethod = instance.GetType().BaseType.GetMethod(methodName);
            IInterceptingProxy proxy = (IInterceptingProxy) instance;
            proxy.SetPipeline(targetMethod, pipeline);

        }
    }

    // Some test classes for interception

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
            if(evt != null)
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
}
