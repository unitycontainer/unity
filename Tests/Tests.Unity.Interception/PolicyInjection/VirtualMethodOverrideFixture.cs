// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.PolicyInjection
{
    /// <summary>
    /// Tests to verify behavior of call handlers in the presence of
    /// overridden virtual methods.
    /// </summary>
    [TestClass]
    public partial class VirtualMethodOverrideFixture
    {

        private static string GetHandlerId(IDictionary<string, List<string>> log)
        {
            return log["created"][0];
        }

        private static void AssertOneHandlerCreated(IDictionary<string, List<string>> log)
        {
            Assert.IsTrue(log.ContainsKey("created"));
            Assert.AreEqual(1, log["created"].Count);
        }

        [TestMethod]
        public void CallHandlerRunsWhenCallingThroughBaseClassReference()
        {
            var container = CreateContainer();
            container.RegisterType<RootClass, DoubleDerivedClass>();
            AddInterceptDoSomethingPolicy(container);

            InterceptWith<DoubleDerivedClass, VirtualMethodInterceptor>(container);

            var log = container.Resolve<Dictionary<string, List<string>>>();

            var target = container.Resolve<RootClass>();
            var baseRef = (RootClass)target;

            baseRef.DoSomething();

            Assert.IsTrue(log.ContainsKey("created"), "No handlers were created");
            
            Assert.IsTrue(log.Keys.Any(k => log["created"].Any(entry=> entry == k)),
                "Log doesn't contain any calls to handlers");
        }

        #region Classes to intercept

        public partial class RootClass
        {
            public virtual string DoSomething()
            {
                return "RootClass.DoSomething";
            }
        }

        public class DerivedClass : RootClass
        {
            public override string DoSomething()
            {
                return "Derived " + base.DoSomething();
            }
        }

        public class DoubleDerivedClass : DerivedClass
        {
            public override string DoSomething()
            {
                return "DoubleDerived.DoSomething";
            }
        }

        #endregion

        #region Dummy call handler for testing

        public class TestingCallHandler : ICallHandler
        {
            private readonly Dictionary<string, List<string>> log;

            public TestingCallHandler(Dictionary<string, List<string>> log)
            {
                this.log = log;
                Log("created", Id);
            }

            /// <summary>
            /// Implement this method to execute your handler processing.
            /// </summary>
            /// <param name="input">Inputs to the current call to the target.</param>
            /// <param name="getNext">Delegate to execute to get the next delegate in the handler
            /// chain.</param>
            /// <returns>Return value from the target.</returns>
            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                Log(Id, "invoked method " + input.MethodBase.DeclaringType.FullName + input.MethodBase.Name);
                return getNext()(input, getNext);
            }

            public int Order { get; set; }

            private string Id
            {
                get
                {
                    return GetHashCode().ToString();
                }
            }

            private void Log(string key, string value)
            {
                if (!log.ContainsKey(key))
                {
                    log[key] = new List<string>();
                }
                log[key].Add(value);
            }
        }

        #endregion

        private static IUnityContainer CreateContainer()
        {
            var container = new UnityContainer()
                .AddNewExtension<Interception>()
                .RegisterType<ICallHandler, TestingCallHandler>("Testing")
                .RegisterInstance(new Dictionary<string, List<string>>());
            return container;
        }

        private static void AddInterceptDoSomethingPolicy(IUnityContainer container)
        {
            container.Configure<Interception>()
                .AddPolicy("InterceptDoSomething")
                .AddMatchingRule(new MemberNameMatchingRule("DoSomething"))
                .AddCallHandler("Testing");
        }

        private static void InterceptWith<TTarget, TInterceptor>(IUnityContainer container)
            where TInterceptor : IInterceptor, new()
        {
            container.RegisterType<TTarget>(
                new Interceptor<TInterceptor>(),
                new InterceptionBehavior<PolicyInjectionBehavior>());
        }
    }
}
