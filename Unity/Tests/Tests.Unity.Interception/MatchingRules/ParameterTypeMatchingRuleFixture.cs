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
    public class ParameterTypeMatchingRuleFixture
    {
        MethodInfo targetMethodString;
        MethodInfo targetMethodInt;
        MethodInfo returnsAString;
        MethodInfo targetMethodIntString;
        MethodInfo targetMethodStringInt;
        MethodInfo targetWithOutParams;

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
            return "";
        }

        public void TargetMethodInt(int foo) {}

        public void TargetMethodIntString(int foo,
                                          string bar) {}

        public void TargetMethodString(string foo) {}

        public void TargetMethodStringInt(string foo,
                                          int bar) {}

        public string TargetWithOutParams(double foo,
                                          out int bar)
        {
            bar = 42;
            return "";
        }
    }
}