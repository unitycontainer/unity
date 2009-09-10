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
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Microsoft.Practices.Unity.TestSupport
{
    public class DelegateInterceptionBehavior : IInterceptionBehavior
    {
        public static readonly Func<IEnumerable<Type>> NoRequiredInterfaces = () => Type.EmptyTypes;

        private readonly Func<IMethodInvocation, GetNextInterceptionBehaviorDelegate, IMethodReturn> invoke;
        private readonly Func<IEnumerable<Type>> requiredInterfaces;

        public DelegateInterceptionBehavior(Func<IMethodInvocation, GetNextInterceptionBehaviorDelegate, IMethodReturn> invoke)
            : this(invoke, NoRequiredInterfaces)
        { }

        public DelegateInterceptionBehavior(
            Func<IMethodInvocation, GetNextInterceptionBehaviorDelegate, IMethodReturn> invoke,
            Func<IEnumerable<Type>> requiredInterfaces)
        {
            this.invoke = invoke;
            this.requiredInterfaces = requiredInterfaces;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            return this.invoke(input, getNext);
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return this.requiredInterfaces();
        }
    }
}
