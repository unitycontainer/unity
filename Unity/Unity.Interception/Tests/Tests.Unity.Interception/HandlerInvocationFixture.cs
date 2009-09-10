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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    /// <summary>
    /// Tests for various kinds of things handlers can do.
    /// </summary>
    [TestClass]
    public class HandlerInvocationFixture
    {
        private IUnityContainer container;

        [TestInitialize]
        public void SetUp()
        {
            container = new UnityContainer();
        }

        [TestMethod]
        public void HandlersCanChangeInputsBeforeTargetIsCalled()
        {
            TransparentProxyInterceptor factory = new TransparentProxyInterceptor();
            PolicySet policies = GetPolicies();

            CanChangeParametersTarget target = new CanChangeParametersTarget();
            IInterceptingProxy proxy = factory.CreateProxy(typeof(CanChangeParametersTarget), target);

            ApplyPolicies(factory, proxy, target, policies);

            CanChangeParametersTarget intercepted = (CanChangeParametersTarget)proxy;
            Assert.AreEqual(0, intercepted.MostRecentInput);
            intercepted.DoSomething(2);
            Assert.AreEqual(4, intercepted.MostRecentInput);
        }

        [TestMethod]
        public void HandlersCanChangeOutputsAfterTargetReturns()
        {
            TransparentProxyInterceptor factory = new TransparentProxyInterceptor();
            PolicySet policies = GetPolicies();


            CanChangeParametersTarget target = new CanChangeParametersTarget();
            IInterceptingProxy proxy = factory.CreateProxy(typeof(CanChangeParametersTarget), target);
            ApplyPolicies(factory, proxy, target, policies);
            CanChangeParametersTarget intercepted = (CanChangeParametersTarget)proxy;
            int output;

            intercepted.DoSomethingElse(2, out output);

            Assert.AreEqual((2 + 5) * 3, output);
        }

        PolicySet GetPolicies()
        {
            RuleDrivenPolicy doubleInputPolicy
                = new RuleDrivenPolicy(
                    "Double your inputs",
                    new IMatchingRule[] { new MemberNameMatchingRule("DoSomething") },
                    new string[] { "Handler1" });
            container.RegisterInstance<ICallHandler>("Handler1", new DoubleInputHandler());

            RuleDrivenPolicy tripleOutputPolicy
                = new RuleDrivenPolicy(
                    "Triple an output parameter",
                    new IMatchingRule[] { new MemberNameMatchingRule("DoSomethingElse") },
                    new string[] { "Handler2" });
            container.RegisterInstance<ICallHandler>("Handler2", new TripleOutputHandler());

            return new PolicySet(doubleInputPolicy, tripleOutputPolicy);
        }

        private void ApplyPolicies(IInterceptor interceptor, IInterceptingProxy proxy, object target, PolicySet policies)
        {
            PipelineManager manager = new PipelineManager();

            foreach(MethodImplementationInfo method in interceptor.GetInterceptableMethods(target.GetType(), target.GetType()))
            {
                HandlerPipeline pipeline = new HandlerPipeline(policies.GetHandlersFor(method, container));
                manager.SetPipeline(method.ImplementationMethodInfo.MetadataToken, pipeline);
            }

            proxy.AddInterceptionBehavior(new PolicyInjectionBehavior(manager));
        }
    }

    class DoubleInputHandler : ICallHandler
    {
        int order = 0;

        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public IMethodReturn Invoke(IMethodInvocation input,
                                    GetNextHandlerDelegate getNext)
        {
            int i = (int)(input.Inputs[0]) * 2;
            input.Inputs[0] = i;
            return getNext()(input, getNext);
        }
    }

    class TripleOutputHandler : ICallHandler
    {
        int order = 0;

        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public IMethodReturn Invoke(IMethodInvocation input,
                                    GetNextHandlerDelegate getNext)
        {
            IMethodReturn result = getNext()(input, getNext);
            if (result.Exception == null)
            {
                int output = (int)result.Outputs[0] * 3;
                result.Outputs[0] = output;
            }
            return result;
        }
    }

    class CanChangeParametersTarget : MarshalByRefObject
    {
        int mostRecentInput = 0;

        public int MostRecentInput
        {
            get { return mostRecentInput; }
        }

        public void DoSomething(int i)
        {
            mostRecentInput = i;
        }

        public void DoSomethingElse(int i,
                                    out int j)
        {
            j = i + 5;
        }
    }
}
