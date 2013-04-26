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

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.TestSupport
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
            Assert.IsNotNull(obj1);
            Assert.IsNotNull(obj2);
            Assert.AreNotSame(obj1, obj2);
        }
    }
}
