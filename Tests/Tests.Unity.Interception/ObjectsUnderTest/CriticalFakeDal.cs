// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Tests.ObjectsUnderTest
{
    [ApplyNoPolicies]
    internal partial class CriticalFakeDal
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
            {
                throw new InvalidOperationException("Catastrophic");
            }
            return 42;
        }

        public string SomethingCritical()
        {
            return "Don't intercept me";
        }
    }
}
