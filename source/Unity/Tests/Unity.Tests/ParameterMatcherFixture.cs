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
using Microsoft.Practices.Unity.Utility;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.Tests
{
    /// <summary>
    /// Tests around the 
    /// </summary>
    [TestClass]
    public class ParameterMatcherFixture
    {
        [TestMethod]
        public void EmptyParameterListMatches()
        {
            ParameterMatcher matcher = new ParameterMatcher(Parameters());

            Assert.IsTrue(matcher.Matches(Types()));
        }

        [TestMethod]
        public void MismatchedParameterListsDontMatch()
        {
            ParameterMatcher matcher = new ParameterMatcher(Parameters());
            Assert.IsFalse(matcher.Matches(Types(typeof(int))));
        }

        [TestMethod]
        public void SameLengthDifferentTypesDontMatch()
        {
            ParameterMatcher matcher = new ParameterMatcher(Parameters(typeof (int)));
            Assert.IsFalse(matcher.Matches(Types(typeof(string))));
        }

        [TestMethod]
        public void SameLengthSameTypesMatch()
        {
            ParameterMatcher matcher = new ParameterMatcher(Parameters(typeof(int), typeof(string)));
            Assert.IsTrue(matcher.Matches(Types(typeof(int), typeof(string))));
        }

        [TestMethod]
        public void OpenGenericTypesMatch()
        {
            ParameterMatcher matcher = new ParameterMatcher(Parameters(typeof (ICommand<>), typeof (ICommand<>)));
            Assert.IsTrue(matcher.Matches(Types(typeof(ICommand<>), typeof(ICommand<>))));
        }

        private static InjectionParameterValue[] Parameters(params Type[] types)
        {
            List<InjectionParameterValue> values = new List<InjectionParameterValue>();
            foreach(Type t in types)
            {
                values.Add(InjectionParameterValue.ToParameter(t));
            }
            return values.ToArray();
        }

        private static Type[] Types(params Type[] types)
        {
            return types;
        }
    }
}
