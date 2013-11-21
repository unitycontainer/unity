// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Practices.Unity.Configuration.Tests.TestObjects.MyGenericTypes
{
    public class MyPrintService<T> : IGenericService<T>
    {

        #region IMyService Members

        public string ServiceStatus
        {
            get { return String.Format("Available for type: {0}", typeof(T)); }
        }

        #endregion
    }
}
