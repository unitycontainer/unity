// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Practices.Unity.Configuration.Tests.TestObjects
{
    class ObjectTakingScalars
    {
        public ObjectTakingScalars(int intValue)
        {
            this.IntValue = intValue;
        }

        internal int IntValue { get; set; }
    }
}
