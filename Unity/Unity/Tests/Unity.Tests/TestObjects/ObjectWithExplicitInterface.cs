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

using Microsoft.Practices.Unity.TestSupport;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.Tests.TestObjects
{
    public interface ISomeCommonProperties
    {
        [Dependency]
        ILogger Logger { get; set; }

        [Dependency]
        object SyncObject { get; set; }
    }

    public class ObjectWithExplicitInterface : ISomeCommonProperties
    {
        private ILogger logger;
        private object syncObject;

        private object somethingElse;

        [Dependency]
        public object SomethingElse
        {
            get { return somethingElse; }
            set { somethingElse = value; }
        }

        [Dependency]
        ILogger ISomeCommonProperties.Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        [Dependency]
        object ISomeCommonProperties.SyncObject
        {
            get { return syncObject; }
            set { syncObject = value; }
        }

        public void ValidateInterface()
        {
            Assert.IsNotNull(logger);
            Assert.IsNotNull(syncObject);
        }
    }
}
