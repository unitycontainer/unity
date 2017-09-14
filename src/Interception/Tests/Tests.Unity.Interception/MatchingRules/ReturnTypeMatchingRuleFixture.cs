// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Tests.MatchingRules
{
    [TestClass]
    public class ReturnTypeMatchingRuleFixture
    {
        private MethodBase objectToStringMethod;
        private MethodBase objectCtor;
        private MethodBase stringCopyToMethod;

        [TestInitialize]
        public void TestInitialize()
        {
            objectToStringMethod = typeof(object).GetMethod("ToString");
            objectCtor = typeof(object).GetConstructor(new Type[0]);
            stringCopyToMethod = typeof(string).GetMethod("CopyTo");
        }

        [TestMethod]
        public void MatchIsDeniedWhenReturnTypeNameDiffers()
        {
            IMatchingRule matchingRule = new ReturnTypeMatchingRule("System.wichReturnType?");
            Assert.IsFalse(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void MatchIsAcceptedWhenReturnTypeNameIsExactMatch()
        {
            IMatchingRule matchingRule = new ReturnTypeMatchingRule("System.String");
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }

        [TestMethod]
        public void MatchIsDeniedWhenReturnTypeIsSpecifiedButNoReturnTypeExistsOnMethodBase()
        {
            IMatchingRule matchingRule = new ReturnTypeMatchingRule("void");
            Assert.IsFalse(matchingRule.Matches(objectCtor));
        }

        [TestMethod]
        public void MatchIsAcceptedWhenReturnTypeIsVoidAndMethodReturnsVoid()
        {
            IMatchingRule matchingRule = new ReturnTypeMatchingRule("System.Void");
            Assert.IsTrue(matchingRule.Matches(stringCopyToMethod));
        }

        [TestMethod]
        public void MatchIsAcceptedForTypeNameWithoutNamespace()
        {
            IMatchingRule matchingRule = new ReturnTypeMatchingRule("string", true);
            Assert.IsTrue(matchingRule.Matches(objectToStringMethod));
        }
    }
}
