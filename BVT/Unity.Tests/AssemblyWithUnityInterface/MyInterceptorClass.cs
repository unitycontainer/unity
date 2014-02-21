// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace AssemblyWithUnityInterface
{
    public class MyInterceptorClass : IInterceptionBehavior
    {
        public bool WillExecute
        {
            get { throw new NotImplementedException(); }
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            throw new NotImplementedException();
        }
    }
}
