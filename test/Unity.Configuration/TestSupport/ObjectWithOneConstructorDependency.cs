// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Unity.TestSupport
{
    public class ObjectWithOneConstructorDependency
    {
        private ILogger logger;

        public ObjectWithOneConstructorDependency(ILogger logger)
        {
            this.logger = logger;
        }

        public ILogger Logger
        {
            get { return logger; }
        }
    }
}
