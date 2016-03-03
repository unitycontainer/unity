// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Unity.Utility;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#else
using Xunit;
#endif

namespace Unity.Tests
{
    /// <summary>
    /// Tests around the 
    /// </summary>
     
    public class ParameterMatcherFixture
    {
        [Fact]
        public void EmptyParameterListMatches()
        {
            ParameterMatcher matcher = new ParameterMatcher(Parameters());

            Assert.True(matcher.Matches(Types()));
        }

        [Fact]
        public void MismatchedParameterListsDontMatch()
        {
            ParameterMatcher matcher = new ParameterMatcher(Parameters());
            Assert.False(matcher.Matches(Types(typeof(int))));
        }

        [Fact]
        public void SameLengthDifferentTypesDontMatch()
        {
            ParameterMatcher matcher = new ParameterMatcher(Parameters(typeof(int)));
            Assert.False(matcher.Matches(Types(typeof(string))));
        }

        [Fact]
        public void SameLengthSameTypesMatch()
        {
            ParameterMatcher matcher = new ParameterMatcher(Parameters(typeof(int), typeof(string)));
            Assert.True(matcher.Matches(Types(typeof(int), typeof(string))));
        }

        [Fact]
        public void OpenGenericTypesMatch()
        {
            ParameterMatcher matcher = new ParameterMatcher(Parameters(typeof(ICommand<>), typeof(ICommand<>)));
            Assert.True(matcher.Matches(Types(typeof(ICommand<>), typeof(ICommand<>))));
        }

        private static InjectionParameterValue[] Parameters(params Type[] types)
        {
            List<InjectionParameterValue> values = new List<InjectionParameterValue>();
            foreach (Type t in types)
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
