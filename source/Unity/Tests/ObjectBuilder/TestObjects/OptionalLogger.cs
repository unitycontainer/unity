// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.ObjectBuilder2.Tests.TestDoubles;

namespace Microsoft.Practices.ObjectBuilder2.Tests.TestObjects
{
    class OptionalLogger
    {
        private string logFile;

        public OptionalLogger([Dependency] string logFile)
        {
            this.logFile = logFile;
        }

        public string LogFile
        {
            get { return logFile; }
        }
    }
}
