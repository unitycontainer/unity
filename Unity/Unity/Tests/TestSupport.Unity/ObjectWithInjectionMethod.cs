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

namespace Microsoft.Practices.Unity.TestSupport
{
    public class ObjectWithInjectionMethod
    {
        public string ConnectionString { get; private set; }

        public ILogger Logger { get; private set; }

        public object MoreData { get; private set; }

        public void Initialize(string connectionString, ILogger logger)
        {
            ConnectionString = connectionString;
            Logger = logger;
        }

        public void MoreInitialization(object data)
        {
            MoreData = data;
        }
    }
}
