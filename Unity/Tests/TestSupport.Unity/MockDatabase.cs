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
