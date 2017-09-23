// TODO: Verify
//// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
//using System;
//using System.Collections.Generic;
//using System.Text;

//using Microsoft.Practices.Unity.InterceptionExtension;
//using Unity.InterceptionExtension;

//namespace Unity.Tests.InterceptionExtension
//{
//    public abstract class InterceptorFixtureBase
//    {
//        protected virtual IUnityContainer GetContainer()
//        {
//            IUnityContainer container = new UnityContainer();
//            container.AddNewExtension<Interception>();

//            container.RegisterType<IMatchingRule, AlwaysMatchingRule>("AlwaysMatchingRule");
//            container.RegisterType<ICallHandler, TestCallHandler>("TestCallHandler", new ContainerControlledLifetimeManager());
//            container.RegisterType<InjectionPolicy, RuleDrivenPolicy>("RuleDrivenPolicy")
//                .Configure<InjectedMembers>()
//                    .ConfigureInjectionFor<RuleDrivenPolicy>("RuleDrivenPolicy",
//                        new InjectionConstructor(
//                            new ResolvedParameter<IMatchingRule[]>(),
//                            new ResolvedArrayParameter<string>("TestCallHandler")));

//            return container;
//        }

//        public class AlwaysMatchingRule : IMatchingRule
//        {
//            public bool Matches(System.Reflection.MethodBase member)
//            {
//                return true;
//            }
//        }

//        public class TestCallHandler : ICallHandler
//        {
//            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
//            {
//                InterceptionCount++;
//                return getNext.Invoke().Invoke(input, getNext);
//            }

//            public int Order { get; set; }

//            public int InterceptionCount { get; set; }
//        }
//    }
//}
