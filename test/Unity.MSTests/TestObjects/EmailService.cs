// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Tests.TestObjects
{
    // A dummy class to support testing type mapping
    public class EmailService : IService
    {
        public bool Disposed = false;
        public void Dispose()
        {
            Disposed = true;
        }
    }
}
