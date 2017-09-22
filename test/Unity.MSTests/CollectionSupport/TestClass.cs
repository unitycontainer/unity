// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;

namespace Unity.Tests.CollectionSupport
{
    public class TestClass : ITestInterface
    {
        public string ID { get; } = Guid.NewGuid().ToString();
    }
}