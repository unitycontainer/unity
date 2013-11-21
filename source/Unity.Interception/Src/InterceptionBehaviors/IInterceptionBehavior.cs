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
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Interception behaviors implement this interface and are called for each
    /// invocation of the pipelines that they're included in.
    /// </summary>
    public interface IInterceptionBehavior
    {
        /// <summary>
        /// Implement this method to execute your behavior processing.
        /// </summary>
        /// <param name="input">Inputs to the current call to the target.</param>
        /// <param name="getNext">Delegate to execute to get the next delegate in the behavior chain.</param>
        /// <returns>Return value from the target.</returns>
        IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext);

        /// <summary>
        /// Returns the interfaces required by the behavior for the objects it intercepts.
        /// </summary>
        /// <returns>The required interfaces.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IEnumerable<Type> GetRequiredInterfaces();

        /// <summary>
        /// Returns a flag indicating if this behavior will actually do anything when invoked.
        /// </summary>
        /// <remarks>This is used to optimize interception. If the behaviors won't actually
        /// do anything (for example, PIAB where no policies match) then the interception
        /// mechanism can be skipped completely.</remarks>
        bool WillExecute { get; }
    }

    /// <summary>
    /// This delegate type is the type that points to the next
    /// method to execute in the current pipeline.
    /// </summary>
    /// <param name="input">Inputs to the current method call.</param>
    /// <param name="getNext">Delegate to get the next interceptor in the chain.</param>
    /// <returns>Return from the next method in the chain.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1711", Justification = "A delegate is indeed required.")]
    public delegate IMethodReturn InvokeInterceptionBehaviorDelegate(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext);

    /// <summary>
    /// This delegate type is passed to each interceptor's Invoke method.
    /// Call the delegate to get the next delegate to call to continue
    /// the chain.
    /// </summary>
    /// <returns>Next delegate in the interceptor chain to call.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1711", Justification = "A delegate is indeed required.")]
    public delegate InvokeInterceptionBehaviorDelegate GetNextInterceptionBehaviorDelegate();
}
