// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.TestSupport;

namespace Unity.TestSupport
{
    public class ObjectWithTwoConstructorParameters
    {
        private string connectionString;
        private ILogger logger;

        public ObjectWithTwoConstructorParameters(string connectionString, ILogger logger)
        {
            this.connectionString = connectionString;
            this.logger = logger;
        }

        public string ConnectionString
        {
            get { return connectionString; }
        }

        public ILogger Logger
        {
            get { return logger; }
        }
    }
}
