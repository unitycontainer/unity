// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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

namespace ObjectBuilder2.Tests
{
     
    public class GenericTypeMappingTest
    {
        [Fact]
        public void CanMapGenericTypeToNewGenericType()
        {
            var original = new NamedTypeBuildKey(typeof(IList<int>));
            var expected = new NamedTypeBuildKey(typeof(List<int>));

            IBuildKeyMappingPolicy policy = new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(typeof(List<>)));

            var result = policy.Map(original, null);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CanMapGenericTypeFromNamedTypeBuildKey()
        {
            NamedTypeBuildKey original = new NamedTypeBuildKey(typeof(IList<string>), "test");
            IBuildKeyMappingPolicy policy = new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(typeof(List<>), "test"));

            NamedTypeBuildKey result = policy.Map(original, null);

            Assert.Equal(typeof(List<string>), result.Type);
            Assert.Equal(original.Name, result.Name);
        }

        [Fact]
        public void PolicyThrowsIfWrongNumberOfGenericParameters()
        {
            var original = new NamedTypeBuildKey(typeof(IList<string>));
            IBuildKeyMappingPolicy policy = new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(typeof(Dictionary<,>)));
            try
            {
                policy.Map(original, null);
                Assert.True(false, string.Format("Expected exception"));
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

        [Fact]
        public void PolicyThrowsIfInputIsNotAGeneric()
        {
            IBuildKeyMappingPolicy policy = new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(typeof(List<>)));

            try
            {
                policy.Map(new NamedTypeBuildKey<int>(), null);
                Assert.True(false, string.Format("Expected Exception"));
            }
            catch (ArgumentException)
            {
                // expected
            }
        }
    }
}
