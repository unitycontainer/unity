// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
#else
using Xunit;
#endif

namespace Unity.Tests.TestObjects
{
    public class ObjectWithOneDependency
    {
        private object inner;

        public ObjectWithOneDependency(object inner)
        {
            this.inner = inner;
        }

        public object InnerObject
        {
            get { return inner; }
        }

        public void Validate()
        {
            Assert.NotNull(inner);
        }
    }
}
