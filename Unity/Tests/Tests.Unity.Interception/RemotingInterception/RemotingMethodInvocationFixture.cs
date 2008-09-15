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
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    /// <summary>
    /// Tests for the RemotingMethodInvocation class, which wraps
    /// a IMethodCallMessage in an IMethodInvocation implementation.
    /// </summary>
    [TestClass]
    public class RemotingMethodInvocationFixture
    {
        #region Test Methods

        [TestMethod]
        public void ShouldBeCreatable()
        {
            MethodBase methodInfo = GetTargetMethodInfo("FirstTarget");
            InvocationTarget target = new InvocationTarget();
            IMethodInvocation invocation = GetInvocation(methodInfo, target);
        }

        [TestMethod]
        public void ShouldMapInputsCorrectly()
        {
            MethodBase methodInfo = GetTargetMethodInfo("FirstTarget");
            InvocationTarget target = new InvocationTarget();
            IMethodInvocation invocation = GetInvocation(methodInfo, target);

            Assert.AreEqual(2, invocation.Inputs.Count);
            Assert.AreEqual(1, invocation.Inputs[0]);
            Assert.AreEqual("two", invocation.Inputs[1]);
            Assert.AreEqual("two", invocation.Inputs["two"]);
            Assert.AreEqual(1, invocation.Inputs["one"]);
            Assert.AreEqual(methodInfo, invocation.MethodBase);
            Assert.AreSame(target, invocation.Target);
        }

        [TestMethod]
        public void ShouldBeAbleToAddToContext()
        {
            MethodBase methodInfo = GetTargetMethodInfo("FirstTarget");
            InvocationTarget target = new InvocationTarget();
            IMethodInvocation invocation = GetInvocation(methodInfo, target);

            invocation.InvocationContext["firstItem"] = 1;
            invocation.InvocationContext["secondItem"] = "hooray!";

            Assert.AreEqual(2, invocation.InvocationContext.Count);
            Assert.AreEqual(1, invocation.InvocationContext["firstItem"]);
            Assert.AreEqual("hooray!", invocation.InvocationContext["secondItem"]);
        }

        [TestMethod]
        public void ShouldBeAbleToChangeInputs()
        {
            MethodBase methodInfo = GetTargetMethodInfo("FirstTarget");
            InvocationTarget target = new InvocationTarget();
            IMethodInvocation invocation = GetInvocation(methodInfo, target);

            Assert.AreEqual(1, invocation.Inputs["one"]);
            invocation.Inputs["one"] = 42;
            Assert.AreEqual(42, invocation.Inputs["one"]);
        }

        #endregion

        #region Helper factories

        MethodBase GetTargetMethodInfo(string methodName)
        {
            return (MethodBase)(typeof(InvocationTarget).GetMember(methodName)[0]);
        }

        IMethodInvocation GetInvocation(MethodBase methodInfo,
                                        InvocationTarget target)
        {
            IMethodCallMessage remotingMessage = new FakeMethodCallMessage(methodInfo, new object[] { 1, "two" });

            return new RemotingMethodInvocation(remotingMessage, target);
        }

        #endregion
    }

    public class InvocationTarget : MarshalByRefObject
    {
        public string FirstTarget(int one,
                                  string two)
        {
            return "Boo!";
        }
    }
}