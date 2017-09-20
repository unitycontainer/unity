// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Unity.InterceptionExtension;

namespace Unity.TestSupport
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

        /// <summary>
        /// Returns a flag indicating if this behavior will actually do anything when invoked.
        /// </summary>
        /// <remarks>This is used to optimize interception. If the behaviors won't actually
        /// do anything (for example, PIAB where no policies match) then the interception
        /// mechanism can be skipped completely.</remarks>
        public bool WillExecute
        {
            get { return true; }
        }
    }
}
