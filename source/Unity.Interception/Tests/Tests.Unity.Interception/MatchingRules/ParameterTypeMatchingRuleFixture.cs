// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules
{
    [TestClass]
    public class ParameterTypeMatchingRuleFixture
    {
        private MethodInfo targetMethodString;
        private MethodInfo targetMethodInt;
        private MethodInfo returnsAString;
        private MethodInfo targetMethodIntString;
        private MethodInfo targetMethodStringInt;
        private MethodInfo targetWithOutParams;

        [TestInitialize]
        public void Setup()
        {
            Type targetType = typeof(ParameterTypeMatchingRuleTarget);
            targetMethodString = targetType.GetMethod("TargetMethodString");
            targetMethodInt = targetType.GetMethod("TargetMethodInt");
            returnsAString = targetType.GetMethod("ReturnsAString");
            targetMethodIntString = targetType.GetMethod("TargetMethodIntString");
            targetMethodStringInt = targetType.GetMethod("TargetMethodStringInt");
            targetWithOutParams = targetType.GetMethod("TargetWithOutParams");
        }

        [TestMethod]
        public void ShouldMatchOnSingleInputParameter()
        {
            ParameterTypeMatchingRule rule = new ParameterTypeMatchingRule(
                new ParameterTypeMatchingInfo[]
                    {
                        new ParameterTypeMatchingInfo("System.String", false, ParameterKind.Input)
                    });

            Assert.IsTrue(rule.Matches(targetMethodString));
            Assert.IsFalse(rule.Matches(targetMethodInt));
        }

        [TestMethod]
        public void ShouldMatchOnReturnType()
        {
            ParameterTypeMatchingRule rule = new ParameterTypeMatchingRule(
                new ParameterTypeMatchingInfo[]
                    {
                        new ParameterTypeMatchingInfo("System.String", false, ParameterKind.ReturnValue)
                    });
            Assert.IsFalse(rule.Matches(targetMethodString));
            Assert.IsTrue(rule.Matches(returnsAString));
        }

        [TestMethod]
        public void ShouldMatchOnOneOfManyParameters()
        {
            ParameterTypeMatchingRule rule = new ParameterTypeMatchingRule(
                new ParameterTypeMatchingInfo[]
                    {
                        new ParameterTypeMatchingInfo("System.Int32", false, ParameterKind.Input)
                    });

            Assert.IsTrue(rule.Matches(targetMethodInt));
            Assert.IsTrue(rule.Matches(targetMethodIntString));
            Assert.IsTrue(rule.Matches(targetMethodStringInt));
            Assert.IsFalse(rule.Matches(returnsAString));
        }

        [TestMethod]
        public void ShouldMatchOnOutParams()
        {
            ParameterTypeMatchingRule rule = new ParameterTypeMatchingRule(
                new ParameterTypeMatchingInfo[]
                    {
                        new ParameterTypeMatchingInfo("System.Int32", false, ParameterKind.Output)
                    });
            Assert.IsTrue(rule.Matches(targetWithOutParams));
            Assert.IsFalse(rule.Matches(targetMethodInt));
        }

        [TestMethod]
        public void ShouldMatchInOrOut()
        {
            ParameterTypeMatchingRule rule = new ParameterTypeMatchingRule(
                new ParameterTypeMatchingInfo[]
                    {
                        new ParameterTypeMatchingInfo("System.Int32", false, ParameterKind.InputOrOutput)
                    });
            Assert.IsTrue(rule.Matches(targetWithOutParams));
            Assert.IsTrue(rule.Matches(targetMethodInt));
        }

        [TestMethod]
        public void ShouldMatchOr()
        {
            ParameterTypeMatchingRule rule = new ParameterTypeMatchingRule(
                new ParameterTypeMatchingInfo[]
                    {
                        new ParameterTypeMatchingInfo("System.Int32", false, ParameterKind.InputOrOutput),
                        new ParameterTypeMatchingInfo("String", false, ParameterKind.InputOrOutput),
                    });

            Assert.IsTrue(rule.Matches(targetMethodString));
            Assert.IsTrue(rule.Matches(targetMethodInt));
            Assert.IsTrue(rule.Matches(targetMethodIntString));
            Assert.IsTrue(rule.Matches(targetWithOutParams));
            Assert.IsFalse(rule.Matches(returnsAString));
        }
    }

    public class ParameterTypeMatchingRuleTarget
    {
        public string ReturnsAString()
        {
            return String.Empty;
        }

        public void TargetMethodInt(int intParam) { }

        public void TargetMethodIntString(int intParam,
                                          string stringParam) { }

        public void TargetMethodString(string param1) { }

        public void TargetMethodStringInt(string stringParam,
                                          int intParam) { }

        public string TargetWithOutParams(double doubleParam,
                                          out int outIntParam)
        {
            outIntParam = 42;
            return String.Empty;
        }
    }
}
