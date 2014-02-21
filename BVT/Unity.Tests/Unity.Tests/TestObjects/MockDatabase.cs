// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Unity.Tests.TestObjects
{
    internal class MockDatabase
    {
        private string connectionString;
        private bool defaultConstructorCalled;
        private int someNumber;

        public MockDatabase()
        {
            defaultConstructorCalled = true;
        }

        public MockDatabase(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public MockDatabase(int testValue)
        {
            this.someNumber = testValue;
        }

        public MockDatabase(string connectionString, int testValue)
        {
            this.connectionString = connectionString;
            this.someNumber = testValue;
        }
        public static MockDatabase Create(string connectionString)
        {
            return new MockDatabase(connectionString);
        }
        
        public int SomeNumber
        {
            get { return someNumber; }
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
