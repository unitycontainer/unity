// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest
{
    public class InterfacesOnlyDal : IDal, IMonitor
    {
        public void Deposit(double amount)
        {
            
        }

        #region IDal Members

        public void Withdraw(double amount)
        {
        }

        #endregion

        #region IMonitor Members

        public void Log(string message)
        {
        }

        #endregion
    }
}
