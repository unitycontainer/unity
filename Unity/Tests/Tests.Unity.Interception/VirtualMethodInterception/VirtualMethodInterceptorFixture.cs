using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
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
}
