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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    class FakeInterceptionBehavior : IInterceptionBehavior
    {
        public Func<IMethodInvocation, GetNextInterceptionBehaviorDelegate, IMethodReturn> InvokeFunc { get; set; }

        IMethodReturn IInterceptionBehavior.Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            return this.InvokeFunc.Invoke(input, getNext);
        }

        IEnumerable<Type> IInterceptionBehavior.GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        bool IInterceptionBehavior.WillExecute
        {
            get { return true; }
        }
    }
}
