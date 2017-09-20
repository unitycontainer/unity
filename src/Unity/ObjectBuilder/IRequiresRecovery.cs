// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace ObjectBuilder2
{
    /// <summary>
    /// This interface provides a hook for the builder context to
    /// implement error recovery when a builder strategy throws
    /// an exception. Since we can't get try/finally blocks onto
    /// the call stack for later stages in the chain, we instead
    /// add these objects to the context. If there's an exception,
    /// all the current IRequiresRecovery instances will have
    /// their Recover methods called.
    /// </summary>
    public interface IRequiresRecovery
    {
        /// <summary>
        /// A method that does whatever is needed to clean up
        /// as part of cleaning up after an exception.
        /// </summary>
        /// <remarks>
        /// Don't do anything that could throw in this method,
        /// it will cause later recover operations to get skipped
        /// and play real havoc with the stack trace.
        /// </remarks>
        void Recover();
    }
}
