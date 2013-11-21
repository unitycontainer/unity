// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.Unity.TestSupport
{
    public class ObjectUsingLogger
    {
        private ILogger logger;

        [Dependency]
        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }
    }
}
