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

using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestSupport.Unity;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    /// <summary>
    /// Tests for the HandlerPipeline class
    /// </summary>
    [TestClass]
    public class PipelineFixture
    {
        CallCountHandler callCountHandler;
        StringReturnRewriteHandler returnHandler;

        [TestMethod]
        public void ShouldBeCreatable()
        {
            HandlerPipeline pipeline = new HandlerPipeline();
        }

        [TestMethod]
        public void ShouldBeCreateableWithHandlers()
        {
            IUnityContainer container = GetContainer();
            PolicySet policies = GetPolicies(container);
            HandlerPipeline pipeline 
                = new HandlerPipeline(policies.GetHandlersFor(GetTargetMemberInfo(), container));
        }

        [TestMethod]
        public void ShouldBeInvokable()
        {
            IUnityContainer container = GetContainer();
            PolicySet policies = GetPolicies(container);
            HandlerPipeline pipeline 
                = new HandlerPipeline(policies.GetHandlersFor(GetTargetMemberInfo(), container));

            IMethodReturn result = pipeline.Invoke(
                MakeCallMessage(),
                delegate(IMethodInvocation message,
                         GetNextHandlerDelegate getNext)
                {
                    return MakeReturnMessage(message);
                });
            Assert.IsNotNull(result);
            Assert.AreEqual(1, callCountHandler.CallCount);
            Assert.AreEqual(returnHandler.ValueToRewriteTo, (string)result.ReturnValue);
        }

        public string MyTargetMethod(int i)
        {
            return string.Format("i = {0}", i);
        }

        PolicySet GetPolicies(IUnityContainer container)
        {
            RuleDrivenPolicy p
                = new RuleDrivenPolicy(
                    "PipelineTestPolicy",
                    new IMatchingRule[] { new AlwaysMatchingRule() },
                    new string[] { "call count", "rewrite" });

            return new PolicySet(p);
        }

        IUnityContainer GetContainer()
        {
            callCountHandler = new CallCountHandler();
            returnHandler = new StringReturnRewriteHandler("REWRITE");

            IUnityContainer container
                = new UnityContainer()
                    .RegisterInstance<ICallHandler>("call count", callCountHandler)
                    .RegisterInstance<ICallHandler>("rewrite", returnHandler);

            return container;
        }

        MethodInfo GetTargetMemberInfo()
        {
            return (MethodInfo)(GetType().GetMember("MyTargetMethod")[0]);
        }

        IMethodInvocation MakeCallMessage()
        {
            FakeMethodCallMessage msg = new FakeMethodCallMessage(GetTargetMemberInfo(), 15);
            IMethodInvocation invocation = new TransparentProxyMethodInvocation(msg, null);
            return invocation;
        }

        IMethodReturn MakeReturnMessage(IMethodInvocation input)
        {
            IMethodReturn result = input.CreateMethodReturn(MyTargetMethod((int)input.Inputs[0]));
            return result;
        }
    }

    class StringReturnRewriteHandler : ICallHandler
    {
        int order = 0;
        string valueToRewriteTo;

        public StringReturnRewriteHandler(string valueToRewriteTo)
        {
            this.valueToRewriteTo = valueToRewriteTo;
        }

        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public string ValueToRewriteTo
        {
            get { return valueToRewriteTo; }
        }

        public IMethodReturn Invoke(IMethodInvocation input,
                                    GetNextHandlerDelegate getNext)
        {
            IMethodReturn retval = getNext()(input, getNext);
            retval.ReturnValue = valueToRewriteTo;
            return retval;
        }
    }
}