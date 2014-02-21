// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace Unity.Tests.TestObjects
{
    internal class ObjectWithInjectionMethod
    {
        private string connectionString;
        private ILogger logger;
        private ILogger logger1;
        private ILogger logger2;
        private IService service;

        public void Initialize(string connectionString, ILogger logger)
        {
            this.connectionString = connectionString;
            this.logger = logger;
        }

        public void Initialize(string connectionString, IService service)
        {
            this.connectionString = connectionString;
            this.service = service;
        }

        public void Initialize(ILogger logger1, ILogger logger2)
        {
            this.logger1 = logger1;
            this.logger2 = logger2;
        }

        public void Initialize(IService service, ILogger logger)
        {
            this.logger = logger;
            this.service = service;
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
            set { value = service; }
        }
    }
}
