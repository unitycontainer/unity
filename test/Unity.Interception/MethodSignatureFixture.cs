// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Tests
{
    [TestClass]
    public class MethodSignatureFixture
    {
        private IUnityContainer container;

        [TestInitialize]
        public void SetUp()
        {
            container = new UnityContainer();
        }

        [TestMethod]
        public void ShouldPassWithNoParameters()
        {
            ISignatureTestTarget target = GetTarget();
            target.MethodWithNoParameters();
        }

        [TestMethod]
        public void ShouldPassWithSimpleInputs()
        {
            ISignatureTestTarget target = GetTarget();
            target.MethodWithSimpleInputs(1, "two");
        }

        [TestMethod]
        public void ShouldPassWithOutParams()
        {
            ISignatureTestTarget target = GetTarget();
            int first;
            string second;
            target.MethodWithOutParams(out first, out second);
            Assert.AreEqual(1, first);
            Assert.AreEqual("two", second);
        }

        [TestMethod]
        public void ShouldPassWithInOutRef()
        {
            ISignatureTestTarget target = GetTarget();
            string two;
            float three = 1.0f;

            target.MethodWithInOutByrefParams(1, out two, ref three, 5.0M);
            Assert.AreEqual("owt", two);
            Assert.AreEqual(3.0f, three);
        }

        [TestMethod]
        public void ShouldPassWithVarArgs()
        {
            ISignatureTestTarget target = GetTarget();
            target.MethodWithVarArgs(1, "two", "two and a half", "two and three quarters");
        }

        private ISignatureTestTarget GetTarget()
        {
            var interceptor = new InterfaceInterceptor();
            PolicySet policySet = GetPolicies();
            var target = new SignatureTestTarget();
            IInterceptingProxy proxy = interceptor.CreateProxy(typeof(ISignatureTestTarget), target);

            PipelineManager manager = new PipelineManager();
            foreach (MethodImplementationInfo method in interceptor.GetInterceptableMethods(typeof(ISignatureTestTarget), typeof(SignatureTestTarget)))
            {
                HandlerPipeline pipeline = new HandlerPipeline(
                    policySet.GetHandlersFor(method, container));
                manager.SetPipeline(method.ImplementationMethodInfo, pipeline);
            }
            proxy.AddInterceptionBehavior(new PolicyInjectionBehavior(manager));

            return (ISignatureTestTarget)proxy;
        }

        private PolicySet GetPolicies()
        {
            PolicySet policies = new PolicySet();

            RuleDrivenPolicy noParamsPolicy
                = new RuleDrivenPolicy(
                    "noParamsPolicy",
                    new IMatchingRule[] { new MatchByNameRule("MethodWithNoParameters") },
                    new string[] { "Handler1" });
            container.RegisterInstance<ICallHandler>("Handler1", new SignatureCheckingHandler(new Type[] { }));
            policies.Add(noParamsPolicy);

            RuleDrivenPolicy simpleInputsPolicy
                = new RuleDrivenPolicy(
                    "simpleInputsPolicy",
                    new IMatchingRule[] { new MatchByNameRule("MethodWithSimpleInputs") },
                    new string[] { "Handler2" });
            container.RegisterInstance<ICallHandler>(
                "Handler2",
                new SignatureCheckingHandler(new Type[] { typeof(int), typeof(string) }));
            policies.Add(simpleInputsPolicy);

            RuleDrivenPolicy outParamsPolicy
                = new RuleDrivenPolicy(
                    "outParamsPolicy",
                    new IMatchingRule[] { new MatchByNameRule("MethodWithOutParams") },
                    new string[] { "Handler3" });
            container.RegisterInstance<ICallHandler>(
                "Handler3",
                new SignatureCheckingHandler(new Type[] { typeof(int).MakeByRefType(), typeof(string).MakeByRefType() }));
            policies.Add(outParamsPolicy);

            RuleDrivenPolicy mixedParamsPolicy
                = new RuleDrivenPolicy(
                    "mixedParamsPolicy",
                    new IMatchingRule[] { new MatchByNameRule("MethodWithInOutByrefParams") },
                    new string[] { "Handler4" });
            container.RegisterInstance<ICallHandler>(
                "Handler4",
                new SignatureCheckingHandler(
                   new Type[]
                           {
                               typeof(int),
                               typeof(string).MakeByRefType(),
                               typeof(float).MakeByRefType(),
                               typeof(decimal)
                           }));
            policies.Add(mixedParamsPolicy);

            RuleDrivenPolicy varargsParamsPolicy
                = new RuleDrivenPolicy(
                    "varargsParamsPolicy",
                    new IMatchingRule[] { new MatchByNameRule("MethodWithVarArgs") },
                    new string[] { "Handler5" });
            container.RegisterInstance<ICallHandler>(
                "Handler5",
                new SignatureCheckingHandler(new Type[] { typeof(int), typeof(string).MakeArrayType() }));
            policies.Add(varargsParamsPolicy);

            return policies;
        }
    }

    public interface ISignatureTestTarget
    {
        void MethodWithInOutByrefParams(int one,
            out string two,
            ref float three,
            decimal four);

        void MethodWithNoParameters();

        void MethodWithOutParams(out int first,
            out string second);

        void MethodWithSimpleInputs(int one,
            string two);

        void MethodWithVarArgs(int one,
            params string[] two);
    }

    internal class SignatureTestTarget : ISignatureTestTarget
    {
        public void MethodWithInOutByrefParams(int one,
                                               out string two,
                                               ref float three,
                                               decimal four)
        {
            two = "owt";
            three = 3.0f;
        }

        public void MethodWithNoParameters() { }

        public void MethodWithOutParams(out int first,
                                        out string second)
        {
            first = 1;
            second = "two";
        }

        public void MethodWithSimpleInputs(int one,
                                           string two) { }

        public void MethodWithVarArgs(int one,
                                      params string[] two) { }
    }

    internal class MatchAllRule : IMatchingRule
    {
        public bool Matches(MethodBase member)
        {
            return true;
        }
    }

    internal class MatchByNameRule : IMatchingRule
    {
        private string name;

        public MatchByNameRule(string name)
        {
            this.name = name;
        }

        public bool Matches(MethodBase member)
        {
            return member.Name == name;
        }
    }

    internal class SignatureCheckingHandler : ICallHandler
    {
        private Type[] expectedSignature;
        private int order = 0;

        public SignatureCheckingHandler(Type[] expectedSignature)
        {
            this.expectedSignature = expectedSignature;
        }

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
            Assert.IsNotNull(expectedSignature);

            Type[] messageSignature = input.MethodBase.GetParameters().Select(info => info.ParameterType).ToArray();

            Assert.AreEqual(expectedSignature.Length, messageSignature.Length);
            for (int i = 0; i < messageSignature.Length; ++i)
            {
                Assert.AreSame(expectedSignature[i], messageSignature[i],
                               "Signature for method {0} failed to match for parameter {1}",
                               input.MethodBase.Name, i);
            }

            return getNext()(input, getNext);
        }
    }
}
