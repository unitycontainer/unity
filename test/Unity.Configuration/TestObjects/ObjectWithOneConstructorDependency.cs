// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Tests.TestObjects
{
    internal class ObjectWithOneConstructorDependency
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
