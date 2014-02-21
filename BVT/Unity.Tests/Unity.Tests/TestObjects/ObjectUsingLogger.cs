// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using Microsoft.Practices.Unity;
namespace Unity.Tests.TestObjects
{
    internal class ObjectUsingLogger
    {
        private ILogger logger;
        private int age;

        [Dependency]
        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }
        [Dependency]
        public int Age
        {
            get { return age; }
            set { age = value; }
        }
    }
}
