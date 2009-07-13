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

namespace Microsoft.Practices.ObjectBuilder2
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
        /// and play real havok with the stack trace.
        /// </remarks>
        void Recover();
    }
}
