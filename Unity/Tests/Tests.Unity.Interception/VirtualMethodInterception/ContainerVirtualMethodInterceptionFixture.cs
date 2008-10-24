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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestSupport.Unity;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.VirtualMethodInterception
{
    /// <summary>
    /// Tests for the virtual method interception mechanism as invoked
    /// through the container.
    /// </summary>
    [TestClass]
    public class ContainerVirtualMethodInterceptionFixture
    {
        [TestMethod]
        public void InterceptedClassGetsReturned()
        {
            CallCountHandler h1 = new CallCountHandler();
            CallCountHandler h2 = new CallCountHandler();

            IUnityContainer container = GetConfiguredContainer(h1, h2);
            AddPoliciesToContainer(container);
            ConfigureInterceptionWithRegisterType(container);

            Interceptee foo = container.Resolve<Interceptee>();

            Assert.AreNotSame(typeof(Interceptee), foo.GetType());
        }

        [TestMethod]
        public void AttachedHandlersAreCalled()
        {
            CallCountHandler h1 = new CallCountHandler();
            CallCountHandler h2 = new CallCountHandler();

            IUnityContainer container = GetConfiguredContainer(h1, h2);
            AddPoliciesToContainer(container);
            ConfigureInterceptionWithRegisterType(container);

            Interceptee foo = container.Resolve<Interceptee>();

            int oneCount = 0;
            int twoCount = 0;

            for(oneCount = 0; oneCount < 2; ++oneCount)
            {
                foo.MethodOne();
            }

            for (twoCount = 0; twoCount < 3; ++twoCount)
            {
                foo.MethodTwo("hi", twoCount);
            }

            Assert.AreEqual(oneCount, h1.CallCount);
            Assert.AreEqual(twoCount, h2.CallCount);
        }

        [TestMethod]
        public void RegisteringInterceptionOnOpenGenericsLetsYouResolveMultipleClosedClasses()
        {
            IUnityContainer container = new UnityContainer()
                .AddNewExtension<Interception>();

            AddPoliciesToContainer(container);

            container.Configure<Interception>()
                .SetDefaultInterceptorFor(typeof (GenericFactory<>), new VirtualMethodInterceptor());

            GenericFactory<SubjectOne> resultOne = container.Resolve<GenericFactory<SubjectOne>>();
            GenericFactory<SubjectTwo> resultTwo = container.Resolve<GenericFactory<SubjectTwo>>();

            Assert.IsTrue(resultOne is IInterceptingProxy);
            Assert.IsTrue(resultTwo is IInterceptingProxy);

            Assert.AreEqual("**Hi**", resultTwo.MakeAT().GetAValue("Hi"));
        }
        [TestMethod]

        public virtual void TestNewVirtualOverride()
        {
            IUnityContainer container = GetContainer();

            NewVirtualOverrideTestClass testClass = container.Resolve<NewVirtualOverrideTestClass>();

            Assert.IsTrue(testClass.TestMethod1(), "override");
            Assert.IsTrue(testClass.TestMethod2(), "new virtual");
            Assert.IsTrue(testClass.TestMethod3(), "always true");
            Assert.IsTrue(testClass.TestMethod4(), "abstract");

            Assert.AreEqual(4, container.Resolve<CallCountHandler>("TestCallHandler").CallCount);
        }

        protected virtual IUnityContainer GetContainer()
        {
            IUnityContainer container = new UnityContainer()
                .AddNewExtension<Interception>()
                .RegisterType<IMatchingRule, AlwaysMatchingRule>("AlwaysMatchingRule")
                .RegisterType<ICallHandler, CallCountHandler>("TestCallHandler", new ContainerControlledLifetimeManager());

            container.Configure<Interception>()
                .SetDefaultInterceptorFor<NewVirtualOverrideTestClass>(new VirtualMethodInterceptor())
                .AddPolicy("Rules")
                .AddMatchingRule("AlwaysMatchingRule")
                .AddCallHandler("TestCallHandler");

            return container;
        }
        
        private IUnityContainer GetConfiguredContainer(ICallHandler h1, ICallHandler h2)
        {
            IUnityContainer container = new UnityContainer()
                .AddNewExtension<Interception>()
                .RegisterInstance<ICallHandler>("h1", h1)
                .RegisterInstance<ICallHandler>("h2", h2);

            return container;
        }

        private IUnityContainer AddPoliciesToContainer(IUnityContainer container)
        {
            container.Configure<Interception>()
                .AddPolicy("MethodOne")
                .AddMatchingRule<MemberNameMatchingRule>(new InjectionConstructor("MethodOne"))
                .AddCallHandler("h1")
                .Interception
                .AddPolicy("MethodTwo")
                .AddMatchingRule<MemberNameMatchingRule>(new InjectionConstructor("MethodTwo"))
                .AddCallHandler("h2");
            return container;
            
        }

        private IUnityContainer ConfigureInterceptionWithRegisterType(IUnityContainer container)
        {
            container.Configure<Interception>()
                .SetInterceptorFor<Interceptee>(null, new VirtualMethodInterceptor());
            return container;
        }
    }

    public class Interceptee
    {
        public virtual int MethodOne()
        {
            return 37;
        }

        public virtual string MethodTwo(string s, int i)
        {
            return s + ";" + i.ToString();
        }
    }

    public class GenericFactory<T> where T : class, new()
    {
        public T MakeAT()
        {
            return new T();
        }
    }

    public class SubjectOne
    {
        
    }

    public class SubjectTwo
    {
        public string GetAValue(string s)
        {
            return "**" + s + "**";
        }
    }

    public class NewVirtualOverrideTestClass : NewVirtualOverrideTestClassBase
    {
        public override bool TestMethod1()
        {
            return true;
        }

        public new virtual bool TestMethod2()
        {
            return true;
        }

        public override bool TestMethod4()
        {
            return true;
        }

        public new virtual void TestMethod5(int bb)
        {
            
        }
    }

    public abstract class NewVirtualOverrideTestClassBase
    {

        public virtual bool TestMethod1()
        {
            return false;
        }

        public virtual bool TestMethod2()
        {
            return false;
        }

        public virtual bool TestMethod3()
        {
            return true;
        }

        public abstract bool TestMethod4();

        public virtual void TestMethod5(int a)
        {

        }
    }

}
