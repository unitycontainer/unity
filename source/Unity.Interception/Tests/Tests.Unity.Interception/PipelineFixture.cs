// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    /// <summary>
    /// Tests for the HandlerPipeline class
    /// </summary>
    [TestClass]
    public class PipelineFixture
    {
        private CallCountHandler callCountHandler;
        private StringReturnRewriteHandler returnHandler;

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

        public virtual string MyTargetMethod(int i)
        {
            return string.Format("i = {0}", i);
        }

        private PolicySet GetPolicies(IUnityContainer container)
        {
            RuleDrivenPolicy p
                = new RuleDrivenPolicy(
                    "PipelineTestPolicy",
                    new IMatchingRule[] { new AlwaysMatchingRule() },
                    new string[] { "call count", "rewrite" });

            return new PolicySet(p);
        }

        private IUnityContainer GetContainer()
        {
            callCountHandler = new CallCountHandler();
            returnHandler = new StringReturnRewriteHandler("REWRITE");

            IUnityContainer container
                = new UnityContainer()
                    .RegisterInstance<ICallHandler>("call count", callCountHandler)
                    .RegisterInstance<ICallHandler>("rewrite", returnHandler);

            return container;
        }

        private MethodImplementationInfo GetTargetMemberInfo()
        {
            return
                new MethodImplementationInfo(null, (MethodInfo)(GetType().GetMember("MyTargetMethod")[0]));
        }

        private IMethodInvocation MakeCallMessage()
        {
            IMethodInvocation invocation = new VirtualMethodInvocation(this, GetTargetMemberInfo().ImplementationMethodInfo, 15);
            return invocation;
        }

        private IMethodReturn MakeReturnMessage(IMethodInvocation input)
        {
            IMethodReturn result = input.CreateMethodReturn(MyTargetMethod((int)input.Inputs[0]));
            return result;
        }
    }

    internal class StringReturnRewriteHandler : ICallHandler
    {
        private int order = 0;
        private string valueToRewriteTo;

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
