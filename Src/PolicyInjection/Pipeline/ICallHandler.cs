// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Remoting.Messaging;

namespace Unity.InterceptionExtension
{
    /// <summary>
    /// Handlers implement this interface and are called for each
    /// invocation of the pipelines that they're included in.
    /// </summary>
    public interface ICallHandler
    {
        /// <summary>
        /// Implement this method to execute your handler processing.
        /// </summary>
        /// <param name="input">Inputs to the current call to the target.</param>
        /// <param name="getNext">Delegate to execute to get the next delegate in the handler
        /// chain.</param>
        /// <returns>Return value from the target.</returns>
        IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext);

        /// <summary>
        /// Order in which the handler will be executed
        /// </summary>
        int Order { get; set; }
    }

    /// <summary>
    /// This delegate type is the type that points to the next
    /// method to execute in the current pipeline.
    /// </summary>
    /// <param name="input">Inputs to the current method call.</param>
    /// <param name="getNext">Delegate to get the next handler in the chain.</param>
    /// <returns>Return from the next method in the chain.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1711", Justification = "A delegate is indeed required.")]
    public delegate IMethodReturn InvokeHandlerDelegate(IMethodInvocation input, GetNextHandlerDelegate getNext);

    /// <summary>
    /// This delegate type is passed to each handler's Invoke method.
    /// Call the delegate to get the next delegate to call to continue
    /// the chain.
    /// </summary>
    /// <returns>Next delegate in the handler chain to call.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1711", Justification = "A delegate is indeed required.")]
    public delegate InvokeHandlerDelegate GetNextHandlerDelegate();
}
