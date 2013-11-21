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
            container = new UnityContainer()
                .AddNewExtension<Interception>()
                .RegisterType<ICanChangeParametersTarget, CanChangeParametersTarget>(
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<PolicyInjectionBehavior>());

            container.Configure<Interception>()
                .AddPolicy("Double your inputs")
                .AddMatchingRule<MemberNameMatchingRule>(
                    new InjectionConstructor("DoSomething"))
                .AddCallHandler("Handler1");

            container.Configure<Interception>()
                .AddPolicy("Triple an output parameter")
                .AddMatchingRule(new MemberNameMatchingRule(new[] {"DoSomethingElse", "DoSomethingElseWithRef"}))
                .AddCallHandler("Handler2");

            container.RegisterInstance<ICallHandler>("Handler1", new DoubleInputHandler());
            container.RegisterInstance<ICallHandler>("Handler2", new TripleOutputHandler());
        }

        [TestMethod]
        public void HandlersCanChangeInputsBeforeTargetIsCalled()
        {
            var intercepted = container.Resolve<ICanChangeParametersTarget>();
            Assert.AreEqual(0, intercepted.MostRecentInput);
            intercepted.DoSomething(2);
            Assert.AreEqual(4, intercepted.MostRecentInput);
        }

        [TestMethod]
        public void HandlersCanChangeOutputsAfterTargetReturns()
        {
            var intercepted = container.Resolve<ICanChangeParametersTarget>();
            int output;

            intercepted.DoSomethingElse(2, out output);

            Assert.AreEqual((2 + 5) * 3, output);
        }

        [TestMethod]
        public void HandlersCanChangeRefsAfterTargetReturns()
        {
            var intercepted = container.Resolve<ICanChangeParametersTarget>();
            int output = 3;

            intercepted.DoSomethingElseWithRef(2, ref output);

            Assert.AreEqual((2 + 3 + 5) * 3, output);
        }
    }

    public class DoubleInputHandler : ICallHandler
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

    public class TripleOutputHandler : ICallHandler
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

    public interface ICanChangeParametersTarget
    {
        int MostRecentInput { get; }
        void DoSomething(int i);
        void DoSomethingElse(int i, out int j);
        void DoSomethingElseWithRef(int i, ref int j);
    }

    public class CanChangeParametersTarget : ICanChangeParametersTarget
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

        public void DoSomethingElse(int i, out int j)
        {
            j = i + 5;
        }

        public void DoSomethingElseWithRef(int i, ref int j)
        {
            j = i + j + 5;
        }
    }
}
