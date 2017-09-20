// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Unity.TestSupport
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
