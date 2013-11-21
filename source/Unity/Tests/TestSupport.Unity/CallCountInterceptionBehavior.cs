// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Microsoft.Practices.Unity.TestSupport
{
    public class CallCountInterceptionBehavior : IInterceptionBehavior
    {
        private int callCount;

        [InjectionConstructor]
        public CallCountInterceptionBehavior()
        {
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            ++callCount;
            return getNext()(input, getNext);
        }

        public int CallCount
        {
            get { return callCount; }
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
