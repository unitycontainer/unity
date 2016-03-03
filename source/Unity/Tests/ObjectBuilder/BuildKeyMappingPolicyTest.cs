// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
     
    public class BuildKeyMappingPolicyTest
    {
        [Fact]
        public void PolicyReturnsNewBuildKey()
        {
            var policy = new BuildKeyMappingPolicy(new NamedTypeBuildKey<string>());

            Assert.Equal(new NamedTypeBuildKey<string>(), policy.Map(new NamedTypeBuildKey<object>(), null));
        }
    }
}
