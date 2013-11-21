// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Microsoft.Practices.Unity.Tests.TestNetAssembly
{
    public class DisposableClass : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
