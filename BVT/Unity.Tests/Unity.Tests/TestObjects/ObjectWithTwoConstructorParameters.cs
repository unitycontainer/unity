// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using Microsoft.Practices.Unity;
namespace Unity.Tests.TestObjects
{
    internal class ObjectWithTwoConstructorParameters
    {
        private string connectionString;
        private ILogger logger;
        private ILogger logger1;
        private ILogger logger2;
        private IService service;

        public ObjectWithTwoConstructorParameters(string connectionString, ILogger logger)
        {
            this.connectionString = connectionString;
            this.logger = logger;
        }

        public ObjectWithTwoConstructorParameters(ILogger logger1, ILogger logger2)
        {
            this.logger1 = logger1;
            this.logger2 = logger2;
        }

        public ObjectWithTwoConstructorParameters(IService service, ILogger logger)
        {
            this.service = service;
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

        public ILogger Logger1
        {
            get { return logger1; }
        }

        public ILogger Logger2
        {
            get { return logger2; }
        }

        public IService Service
        {
            get { return service; }
        }
    }
}
