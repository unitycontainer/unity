// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
