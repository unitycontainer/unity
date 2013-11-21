// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.Unity.TestSupport
{
    public class MockDatabase
    {
        private string connectionString;
        private bool defaultConstructorCalled;

        public MockDatabase()
        {
            defaultConstructorCalled = true;    
        }

        public MockDatabase(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public static MockDatabase Create(string connectionString)
        {
            return new MockDatabase(connectionString);
        }

        public string ConnectionString
        {
            get { return connectionString; }
        }

        public bool DefaultConstructorCalled
        {
            get { return defaultConstructorCalled; }
        }
    }
}
