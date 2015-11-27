// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace ObjectBuilder2.Tests.TestObjects
{
    public class FileLogger
    {
        private string logFile;

        public FileLogger(string logFile)
        {
            this.logFile = logFile;
        }

        public string LogFile
        {
            get { return logFile; }
        }
    }
}
