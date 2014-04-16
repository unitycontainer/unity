// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public class BuildKeyMappingPolicyTest
    {
        [TestMethod]
        public void PolicyReturnsNewBuildKey()
        {
            var policy = new BuildKeyMappingPolicy(new NamedTypeBuildKey<string>());

            Assert.AreEqual(new NamedTypeBuildKey<string>(), policy.Map(new NamedTypeBuildKey<object>(), null));
        }
    }
}
