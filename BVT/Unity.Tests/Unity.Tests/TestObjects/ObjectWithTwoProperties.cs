// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using Microsoft.Practices.Unity;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Unity.Tests.TestObjects
{
    // A dummy class to test that property setter injection works
    internal class ObjectWithTwoProperties
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
            Assert.IsNotNull(obj1);
            Assert.IsNotNull(obj2);
            Assert.AreNotSame(obj1, obj2);
        }
    }
}
