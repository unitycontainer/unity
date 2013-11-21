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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules
{
    [TestClass]
    public class ReturnTypeMatchingRuleFixture
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
