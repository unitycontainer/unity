//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

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
