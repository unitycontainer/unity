// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Unity.Tests.TestObjects
{
    // A dummy class to support testing type mapping
    public class EmailService : IService, IDisposable
    {
        public string ID { get; } = Guid.NewGuid().ToString();

        public bool Disposed = false;
        public void Dispose()
        {
            Disposed = true;
        }
    }

    // A dummy class to support testing type mapping
    public class OtherEmailService : IService, IOtherService, IDisposable
    {
        public bool Disposed = false;
        public void Dispose()
        {
            Disposed = true;
        }
    }

}
