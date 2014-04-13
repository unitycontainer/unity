// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Microsoft.Practices.Unity.TestSupport
{
    public class AdditionalInterfaceBehavior : IInterceptionBehavior
    {
        private static readonly MethodInfo DoNothingMethod = typeof(IAdditionalInterface).GetMethod("DoNothing");
        private bool implicitlyAddInterface = true;

        public AdditionalInterfaceBehavior()
        {
            implicitlyAddInterface = true;
        }

        public AdditionalInterfaceBehavior(bool implicitlyAddInterface)
        {
            this.implicitlyAddInterface = implicitlyAddInterface;
        }

        /// <summary>
        /// Implement this method to execute your behavior processing.
        /// </summary>
        /// <param name="input">Inputs to the current call to the target.</param>
        /// <param name="getNext">Delegate to execute to get the next delegate in the behavior chain.</param>
        /// <returns>Return value from the target.</returns>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            if (input.MethodBase == DoNothingMethod)
            {
                return ExecuteDoNothing(input);
            }
            return getNext()(input, getNext);
        }

        private IMethodReturn ExecuteDoNothing(IMethodInvocation input)
        {
            IMethodReturn returnValue = input.CreateMethodReturn(10);
            return returnValue;
        }

        /// <summary>
        /// Returns the interfaces required by the behavior for the objects it intercepts.
        /// </summary>
        /// <returns>The required interfaces.</returns>
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            if (implicitlyAddInterface)
            {
                return new[] { typeof(IAdditionalInterface) };
            }
            return Type.EmptyTypes;
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
