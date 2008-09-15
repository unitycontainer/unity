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
    public interface IDal
    {
        void Deposit(double amount);
        void Withdraw(double amount);
    }

    public interface IMonitor
    {
        void Log(string message);
    }

    public class MockDal : MarshalByRefObject, IDal, IMonitor
    {
        private bool throwException;
        private double balance = 0.0;

        public bool ThrowException
        {
            get { return throwException; }
            set { throwException = value; }
        }

        public double Balance
        {
            get { return balance; }
            set { balance = value; }
        }

        public int DoSomething(string x)
        {
            if (throwException)
                throw new InvalidOperationException("Catastrophic");
            return 42;
        }

        #region IDal Members

        public void Deposit(double amount)
        {
        }

        public void Withdraw(double amount)
        {
        }

        #endregion


        #region IMonitor Members

        public void Log(string message)
        {

        }

        #endregion

        [ApplyNoPolicies]
        public string SomethingCritical()
        {
            return "Don't intercept me";
        }
    }
}