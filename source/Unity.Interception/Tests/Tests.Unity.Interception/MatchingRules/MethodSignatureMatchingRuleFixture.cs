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
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules
{
    [TestClass]
    public class MethodSignatureMatchingRuleFixture
    {
        MethodBase objectToStringMethod;
        MethodBase objectCtor;
        MethodBase stringCopyToMethod;

        [TestInitialize]
        public void TestInitialize()
        {
            objectToStringMethod = typeof(object).GetMethod("ToString");
            objectCtor = typeof(object).GetConstructor(new Type[0]);
            stringCopyToMethod = typeof(string).GetMethod("CopyTo");
        }

        [TestMethod]
        public void MatchIsDeniedWhenParamterValuesCountDiffers()
        {
            List<string> oneParam = new List<string>();
            oneParam.Add("one");

            MethodSignatureMatchingRule matchingRule = new MethodSignatureMatchingRule(oneParam);
            Assert.IsFalse(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchOnParameterlessMethods()
        {
            List<string> parameterLess = new List<string>();
            MethodSignatureMatchingRule matchingRule = new MethodSignatureMatchingRule(parameterLess);
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void CanMatchOnMultipleParameterTypes()
        {
            List<string> parametersForCopyToMethod = new List<string>();
            parametersForCopyToMethod.Add("System.Int32");
            parametersForCopyToMethod.Add("System.Char[]");
            parametersForCopyToMethod.Add("System.Int32");
            parametersForCopyToMethod.Add("System.Int32");

            MethodSignatureMatchingRule matchingRule = new MethodSignatureMatchingRule(parametersForCopyToMethod);
            Assert.IsTrue(matchingRule.Matches(stringCopyToMethod));
        }

        [TestMethod]
        public void MatchIsDeniedWhenASingleParameterIsWrong()
        {
            List<string> parametersForCopyToMethod = new List<string>();
            parametersForCopyToMethod.Add("System.Int32");
            parametersForCopyToMethod.Add("System.Char[]");
            parametersForCopyToMethod.Add("System.NotAnInt32");
            parametersForCopyToMethod.Add("System.Int32");

            MethodSignatureMatchingRule matchingRule = new MethodSignatureMatchingRule(parametersForCopyToMethod);
            Assert.IsFalse(matchingRule.Matches(stringCopyToMethod));
        }
    }
}
