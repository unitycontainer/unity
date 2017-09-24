// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.InterceptionExtension;

namespace Unity.Tests.InterceptionExtension
{
    // TODO: Verify
    ///// <summary>
    ///// Summary description for ConvenienceInjectionAPI
    ///// </summary>
    //[TestClass]
    //public class ConvenienceInterceptionAPI
    //{
    //    [TestMethod]
    //    [ExpectedException(typeof(ArgumentException))]
    //    public void ConfigureContainerwithNoPolicy()
    //    {
    //        IUnityContainer container = new UnityContainer();
    //        container.AddNewExtension<Interception>();
    //        container.Configure<Interception>().AddPolicy(String.Empty);
    //    }

    //    [TestMethod]
    //    [ExpectedException(typeof(ArgumentNullException))]
    //    public void ConfigureContainerwithNullPolicy()
    //    {
    //        IUnityContainer container = new UnityContainer();
    //        container.AddNewExtension<Interception>();
    //        container.Configure<Interception>().AddPolicy(null);
    //    }

    //    [TestMethod]
    //    public void ConfigureContainerbyAddingExteralPolicy()
    //    {
    //        IUnityContainer container = new UnityContainer();
    //        container.AddNewExtension<Interception>();
    //        container.Configure<Interception>().AddPolicy("myRDP");
    //        List<InjectionPolicy> policy = new List<InjectionPolicy>(container.ResolveAll<InjectionPolicy>());
    //        Assert.AreEqual("myRDP", policy[1].Name);
    //    }

    //    [TestMethod]
    //    [ExpectedException(typeof(ResolutionFailedException))]
    //    public void ConfigureContainerbyAddingEmptyHandlerRuleThrowException()
    //    {
    //        IUnityContainer container = new UnityContainer();
    //        container.AddNewExtension<Interception>();
    //        container.Configure<Interception>().
    //            AddPolicy("myRDP").
    //            AddCallHandler(String.Empty).
    //            AddMatchingRule(String.Empty);
    //        List<InjectionPolicy> policy = new List<InjectionPolicy>(container.ResolveAll<InjectionPolicy>());
    //    }

    //    [TestMethod]
    //    public void ConfigureContainerbyAddingExteralPolicyHandlerRule()
    //    {
    //        IUnityContainer container = new UnityContainer();
    //        container.AddNewExtension<Interception>();
    //        container.Configure<Interception>().
    //            AddPolicy("myRDP").
    //            AddCallHandler("myHandler1").
    //            AddMatchingRule("myMatchingRule1")
    //            .Interception.Container.RegisterType<ICallHandler, CallCountHandler>("myHandler1")
    //            .RegisterType<IMatchingRule, AlwaysMatchingRule>("myMatchingRule1");
    //    }

    //    [TestMethod]
    //    public void ConfigureContainerbyAddingPolicyHanlderRule()
    //    {
    //        ICallHandler myHandler1 = new CallCountHandler();
    //        IMatchingRule myMatchingRule1 = new AlwaysMatchingRule();

    //        IUnityContainer container = new UnityContainer();
    //        container.AddNewExtension<Interception>();
    //        container.Configure<Interception>().
    //            AddPolicy("myRDP").
    //            AddCallHandler(myHandler1).
    //            AddMatchingRule(myMatchingRule1);
    //        container.Configure<Interception>().
    //            SetInterceptorFor<InterceptionClass>("interceptionclass", new TransparentProxyInterceptor());

    //        InterceptionClass ic = container.Resolve<InterceptionClass>("interceptionclass");
    //        ic.MethodA();
    //        Assert.AreEqual(1, ((CallCountHandler)myHandler1).CallCount);
    //    }

    //    [TestMethod]
    //    public void ConfigureContainerbyAddingPolicyHanlderRuleType()
    //    {
    //        IUnityContainer container = new UnityContainer();
    //        container.AddNewExtension<Interception>();
    //        GlobalCountCallHandler.Calls.Clear();
    //        container.Configure<Interception>().
    //            AddPolicy("myRDP").
    //            AddCallHandler(typeof(GlobalCountCallHandler)).
    //            AddMatchingRule(typeof(AlwaysMatchingRule));
    //        container.Configure<Interception>().
    //            SetInterceptorFor<InterceptionClass>("interceptionclass", new TransparentProxyInterceptor());
    //        InterceptionClass ic = container.Resolve<InterceptionClass>("interceptionclass");
    //        ic.MethodA();
    //        Assert.AreEqual(1, GlobalCountCallHandler.Calls["default"]);
    //    }

    //    [TestMethod]
    //    public void ConfigureContainerbyAddingPolicyHanlderRuleGenerics()
    //    {
    //        IUnityContainer container = new UnityContainer();
    //        container.AddNewExtension<Interception>();
    //        GlobalCountCallHandler.Calls.Clear();
    //        container.Configure<Interception>().
    //            AddPolicy("myRDP").
    //            AddCallHandler<GlobalCountCallHandler>().
    //            AddMatchingRule<AlwaysMatchingRule>();
    //        container.Configure<Interception>().
    //            SetInterceptorFor<InterceptionClass>("interceptionclass", new TransparentProxyInterceptor());
    //        InterceptionClass ic = container.Resolve<InterceptionClass>("interceptionclass");
    //        ic.MethodA();

    //        Assert.AreEqual(1, GlobalCountCallHandler.Calls["default"]);
    //    }

    //    [TestMethod]
    //    public void ConfigureContainerbyInjectingPolicyHanlderRule()
    //    {
    //        IUnityContainer container = new UnityContainer();
    //        container.AddNewExtension<Interception>();
    //        GlobalCountCallHandler.Calls.Clear();
    //        container.Configure<Interception>().
    //            AddPolicy("myRDP").
    //            AddCallHandler<GlobalCountCallHandler>(new InjectionConstructor("myHandler1")).
    //            AddCallHandler<GlobalCountCallHandler>(new InjectionConstructor("myHandler2")).
    //            AddCallHandler<GlobalCountCallHandler>(new InjectionConstructor("myHandler3")).
    //            AddMatchingRule<AlwaysMatchingRule>();
    //        container.Configure<Interception>().
    //            SetInterceptorFor<InterceptionClass>("interceptionclass", new TransparentProxyInterceptor());
    //        InterceptionClass ic = container.Resolve<InterceptionClass>("interceptionclass");
    //        ic.MethodA();

    //        Assert.AreEqual(1, GlobalCountCallHandler.Calls["myHandler1"]);
    //    }

    //    [TestMethod]
    //    public void ConfigureContainerbyInjectingNamedHanlder()
    //    {
    //        IUnityContainer container = new UnityContainer();
    //        container.AddNewExtension<Interception>();
    //        GlobalCountCallHandler.Calls.Clear();
    //        container.Configure<Interception>().
    //            AddPolicy("myRDP").
    //            AddCallHandler<GlobalCountCallHandler>("handler1", new InjectionConstructor("myHandler1")).
    //            AddCallHandler<GlobalCountCallHandler>(new InjectionConstructor("myHandler2")).
    //            AddCallHandler<GlobalCountCallHandler>("hanlder3", new InjectionConstructor("myHandler3")).
    //            AddMatchingRule<AlwaysMatchingRule>();
    //        container.Configure<Interception>().
    //            SetInterceptorFor<InterceptionClass>("interceptionclass", new TransparentProxyInterceptor());
    //        InterceptionClass ic = container.Resolve<InterceptionClass>("interceptionclass");
    //        ic.MethodA();

    //        GlobalCountCallHandler handler1 = (GlobalCountCallHandler)container.Resolve<ICallHandler>("handler1");

    //        Assert.AreEqual(1, GlobalCountCallHandler.Calls["myHandler1"]);
    //        Assert.AreEqual(1, GlobalCountCallHandler.Calls["myHandler3"]);
    //        Assert.AreEqual(1, GlobalCountCallHandler.Calls["myHandler2"]);
    //    }

    //    [TestMethod]
    //    public void ConfigureContainerbyInjectingProperty()
    //    {
    //        IUnityContainer container = new UnityContainer();
    //        container.AddNewExtension<Interception>();
    //        GlobalCountCallHandler.Calls.Clear();
    //        container.Configure<Interception>().
    //            AddPolicy("myRDP").
    //            AddCallHandler<GlobalCountCallHandler>("handler1", new InjectionConstructor("myHandler1")).
    //            AddCallHandler<GlobalCountCallHandler>("hanlder3", new InjectionConstructor("myHandler3"), new InjectionProperty("Order", 1000)).
    //            AddMatchingRule<AlwaysMatchingRule>();
    //        container.Configure<Interception>().
    //            SetInterceptorFor<InterceptionClass>("interceptionclass", new TransparentProxyInterceptor());
    //        InterceptionClass ic = container.Resolve<InterceptionClass>("interceptionclass");
    //        ic.MethodA();

    //        GlobalCountCallHandler handler3 = (GlobalCountCallHandler)container.Resolve<ICallHandler>("hanlder3");

    //        Assert.AreEqual(1000, handler3.Order);

    //        Assert.AreEqual(1, GlobalCountCallHandler.Calls["myHandler1"]);
    //        Assert.AreEqual(1, GlobalCountCallHandler.Calls["myHandler3"]);
    //    }

    //    [TestMethod]
    //    public void ConfiguredContainerbyDefaultisContainerControlledLifeTimeManaged()
    //    {
    //        IUnityContainer container = new UnityContainer();
    //        container.AddNewExtension<Interception>();
    //        GlobalCountCallHandler.Calls.Clear();
    //        container.Configure<Interception>().
    //            AddPolicy("myRDP").
    //            AddCallHandler<GlobalCountCallHandler>(new ContainerControlledLifetimeManager(), new InjectionConstructor("myHandler1")).
    //            AddCallHandler<GlobalCountCallHandler>(new InjectionConstructor("myHandler3"), new InjectionProperty("Order", 1000)).
    //            AddMatchingRule<AlwaysMatchingRule>();
    //        container.Configure<Interception>().
    //            SetInterceptorFor<InterceptionClass>("interceptionclass", new TransparentProxyInterceptor());
    //        InterceptionClass ic = container.Resolve<InterceptionClass>("interceptionclass");
    //        ic.MethodA();
    //        Assert.AreEqual(1, GlobalCountCallHandler.Calls["myHandler1"]);
    //        Assert.AreEqual(1, GlobalCountCallHandler.Calls["myHandler3"]);
    //    }

    //    [TestMethod]
    //    public void ConfigureContainerReturnEmptyIfNoInterception()
    //    {
    //        ICallHandler myHandler1 = new CallCountHandler();
    //        IMatchingRule myMatchingRule1 = new AlwaysMatchingRule();

    //        IUnityContainer container = new UnityContainer();
    //        container.AddNewExtension<Interception>();
    //        container.Configure<Interception>().
    //            AddPolicy("myRDP").
    //            AddCallHandler(myHandler1).
    //            AddMatchingRule(myMatchingRule1);
    //        Assert.AreEqual(0, ((CallCountHandler)myHandler1).CallCount);
    //    }

    //    internal interface InterfaceA
    //    {
    //        void MethodA();
    //    }

    //    internal interface InterfaceB
    //    {
    //        void MethodB();
    //    }

    //    internal interface InterfaceC : InterfaceA
    //    {
    //        void MethodC();
    //    }

    //    internal class TestClass
    //    {
    //        public bool IntConstructorCalled = false;
    //        public bool StringConstructorCalled = false;
            
    //        public TestClass()
    //        {
    //        }

    //        public TestClass(int i)
    //        {
    //            IntConstructorCalled = true;
    //        }

    //        public TestClass(string s)
    //        {
    //            StringConstructorCalled = true;
    //        }
    //    }

    //    internal class GuineaPig
    //    {
    //        public bool DefaultConstructorCalled = false;
    //        public bool OneArgumentConstructorCalled = false;
    //        public bool ThreeArgumentConstructorCalled = false;

    //        public object O;
    //        public int I;
    //        public string S;
    //        public double D;

    //        public GuineaPig()
    //        {
    //            DefaultConstructorCalled = true;
    //        }

    //        public GuineaPig(object o)
    //        {
    //            OneArgumentConstructorCalled = true;
    //            O = o;
    //        }

    //        public GuineaPig(int i, string s, double d)
    //        {
    //            ThreeArgumentConstructorCalled = true;
    //            I = i;
    //            S = s;
    //            D = d;
    //        }

    //        public int IntProperty
    //        {
    //            get { return I; }
    //            set { I = value; }
    //        }

    //        public object ObjectProperty
    //        {
    //            get { return O; }
    //            set { O = value; }
    //        }

    //        public void InjectMeHerePlease(string s)
    //        {
    //            S = s;
    //        }

    //        public void InjectMeHerePleaseOnceMore(int i)
    //        {
    //            I = i;
    //        }
    //    }

    //    internal class InterceptionClass : MarshalByRefObject, InterfaceB, InterfaceC
    //    {
    //        public void MethodA() { }
    //        public void MethodB() { }
    //        public void MethodC() { }
    //    }

    //    internal class AlwaysMatchingRule : IMatchingRule
    //    {
    //        [InjectionConstructor]
    //        public AlwaysMatchingRule()
    //        {
    //        }

    //        public AlwaysMatchingRule(NameValueCollection attributes)
    //        {
    //        }

    //        public bool Matches(MethodBase member)
    //        {
    //            return true;
    //        }
    //    }

    //    internal class CallCountHandler : ICallHandler
    //    {
    //        private int callCount;

    //        [InjectionConstructor]
    //        public CallCountHandler()
    //        {
    //        }

    //        public CallCountHandler(NameValueCollection attributes)
    //        {
    //        }

    //        public int Order { get; set; }

    //        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
    //        {
    //            ++callCount;
    //            return getNext()(input, getNext);
    //        }

    //        public int CallCount
    //        {
    //            get { return callCount; }
    //        }
    //    }

    //    internal class GlobalCountCallHandler : ICallHandler
    //    {
    //        public static Dictionary<string, int> Calls = new Dictionary<string, int>();
    //        private string callHandlerName;

    //        [InjectionConstructor]
    //        public GlobalCountCallHandler()
    //            : this("default")
    //        {
    //        }

    //        public GlobalCountCallHandler(NameValueCollection attributes)
    //        {
    //            callHandlerName = attributes["callhandler"];
    //        }

    //        public GlobalCountCallHandler(string callHandlerName)
    //        {
    //            this.callHandlerName = callHandlerName;
    //        }

    //        public int Order { get; set; }

    //        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
    //        {
    //            if (!Calls.ContainsKey(callHandlerName))
    //            {
    //                Calls.Add(callHandlerName, 0);
    //            }
    //            Calls[callHandlerName]++;

    //            return getNext().Invoke(input, getNext);
    //        }
    //    }

    //    internal class GlobalCountCallHandlerAttribute : HandlerAttribute
    //    {
    //        public override ICallHandler CreateHandler(IUnityContainer ignored)
    //        {
    //            return new GlobalCountCallHandler(this.HandlerName);
    //        }

    //        public string HandlerName { get; set; }
    //    }
    //}
}
