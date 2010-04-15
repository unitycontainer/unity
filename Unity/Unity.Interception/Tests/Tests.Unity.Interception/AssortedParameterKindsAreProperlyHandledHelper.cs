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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    public static class AssortedParameterKindsAreProperlyHandledHelper
    {
        public static void PerformTest(IInterceptingProxy proxy)
        {
            Mock<IInterceptionBehavior> behavior = new Mock<IInterceptionBehavior>();
            behavior.Setup(p => p.WillExecute).Returns(true);
            behavior.Setup(p => p.GetRequiredInterfaces()).Returns(Type.EmptyTypes);

            int argumentsCount = 0;
            object[] argumentsValuesByIndex = null;
            string[] argumentsNames = null;
            object[] argumentsValuesByName = null;
            int inputsCount = 0;
            object[] inputsValuesByIndex = null;
            string[] inputsNames = null;
            object[] inputsValuesByName = null;
            int outputsCount = 0;
            object[] outputsValuesByIndex = null;
            string[] outputsNames = null;
            object[] outputsValuesByName = null;
            object originalReturnValue = null;

            behavior.Setup(p => p.Invoke(It.IsAny<IMethodInvocation>(), It.IsAny<GetNextInterceptionBehaviorDelegate>()))
                .Returns((IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext) =>
                {
                    argumentsCount = input.Arguments.Count;
                    argumentsValuesByIndex = Enumerable.Range(0, argumentsCount).Select(pi => input.Arguments[pi]).ToArray();
                    argumentsNames = Enumerable.Range(0, argumentsCount).Select(pi => input.Arguments.GetParameterInfo(pi).Name).ToArray();
                    argumentsValuesByName = argumentsNames.Select(an => input.Arguments[an]).ToArray();

                    inputsCount = input.Inputs.Count;
                    inputsValuesByIndex = Enumerable.Range(0, inputsCount).Select(pi => input.Inputs[pi]).ToArray();
                    inputsNames = Enumerable.Range(0, inputsCount).Select(pi => input.Inputs.GetParameterInfo(pi).Name).ToArray();
                    inputsValuesByName = inputsNames.Select(an => input.Inputs[an]).ToArray();

                    input.Inputs["param1"] = 11;
                    input.Inputs[1] = 13;
                    input.Inputs["param4"] = 14;
                    input.Inputs[3] = 15;

                    var result = getNext()(input, getNext);

                    outputsCount = result.Outputs.Count;
                    outputsValuesByIndex = Enumerable.Range(0, outputsCount).Select(pi => result.Outputs[pi]).ToArray();
                    outputsNames = Enumerable.Range(0, outputsCount).Select(pi => result.Outputs.GetParameterInfo(pi).Name).ToArray();
                    outputsValuesByName = outputsNames.Select(an => result.Outputs[an]).ToArray();

                    originalReturnValue = result.ReturnValue;

                    result.Outputs[0] = 82;
                    result.Outputs["param4"] = 84;

                    result.ReturnValue = 100;

                    return result;
                });

            proxy.AddInterceptionBehavior(behavior.Object);

            int param2, param4;
            param4 = 4;
            var returnValue = ((ITypeWithAssertedParameterKinds)proxy).DoSomething(1, out param2, 3, ref param4, 5);

            Assert.AreEqual(100, returnValue);
            Assert.AreEqual(82, param2);
            Assert.AreEqual(84, param4);

            Assert.AreEqual(5, argumentsCount);
            CollectionAssert.AreEqual(new[] { 1, 0, 3, 4, 5 }, argumentsValuesByIndex);
            CollectionAssert.AreEqual(new[] { "param1", "param2", "param3", "param4", "param5" }, argumentsNames);
            CollectionAssert.AreEqual(new[] { 1, 0, 3, 4, 5 }, argumentsValuesByName);

            Assert.AreEqual(4, inputsCount);
            CollectionAssert.AreEqual(new[] { 1, 3, 4, 5 }, inputsValuesByIndex);
            CollectionAssert.AreEqual(new[] { "param1", "param3", "param4", "param5" }, inputsNames);
            CollectionAssert.AreEqual(new[] { 1, 3, 4, 5 }, inputsValuesByName);

            Assert.AreEqual(2, outputsCount);
            CollectionAssert.AreEqual(new[] { 25, 39 }, outputsValuesByIndex);
            CollectionAssert.AreEqual(new[] { "param2", "param4" }, outputsNames);
            CollectionAssert.AreEqual(new[] { 25, 39 }, outputsValuesByName);

            Assert.AreEqual(11 + 25 + 13 + 39 + 15, originalReturnValue);
        }

        public interface ITypeWithAssertedParameterKinds
        {
            int DoSomething(int param1, out int param2, int param3, ref int param4, int param5);
        }

        public class TypeWithAssertedParameterKinds : MarshalByRefObject, ITypeWithAssertedParameterKinds
        {
            public virtual int DoSomething(int param1, out int param2, int param3, ref int param4, int param5)
            {
                param2 = param1 + param4;
                param4 = param1 + param3 + param5;

                return param1 + param2 + param3 + param4 + param5;
            }
        }
    }
}
