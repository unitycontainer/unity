// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
#else
using Xunit;
#endif

namespace Unity.TestSupport
{
    public class ObjectWithTwoProperties
    {
        private object obj1;
        private object obj2;

        [Dependency]
        public object Obj1
        {
            get { return obj1; }
            set { obj1 = value; }
        }

        [Dependency]
        public object Obj2
        {
            get { return obj2; }
            set { obj2 = value; }
        }

        public void Validate()
        {
            Assert.NotNull(obj1);
            Assert.NotNull(obj2);
            Assert.NotSame(obj1, obj2);
        }
    }
}
