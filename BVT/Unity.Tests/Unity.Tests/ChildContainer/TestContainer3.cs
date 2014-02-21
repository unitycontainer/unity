// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;

namespace Unity.Tests.ChildContainer
{
    public class TestContainer3 : ITestContainer, IDisposable
    {
        private bool wasDisposed = false;

        public bool WasDisposed
        {
            get { return wasDisposed; }
            set { wasDisposed = value; }
        }
        
        public void Dispose()
        {
            wasDisposed = true;
        }
    }
}