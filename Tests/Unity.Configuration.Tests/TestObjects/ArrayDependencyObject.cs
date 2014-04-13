// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.Unity.TestSupport;

namespace Microsoft.Practices.Unity.Configuration.Tests.TestObjects
{
    internal class ArrayDependencyObject
    {
        public ILogger[] Loggers { get; set; }

        public string[] Strings { get; set; }
    }
}
