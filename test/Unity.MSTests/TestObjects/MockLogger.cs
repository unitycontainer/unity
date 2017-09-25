// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;


namespace Unity.Tests.TestObjects
{
    // A dummy class to support testing type mapping
    public class MockLogger : ILogger, IDisposable
    {
        public bool Disposed = false;
        
        public void Dispose()
        {
            Disposed = true;
        }

        public IService Service
        {
            get;
            set;
        }
    }
}
