// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.Utility;

namespace ObjectBuilder2
{
    /// <summary>
    /// An implementation of <see cref="IRecoveryStack"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", 
        Justification = "The name ends in stack because the semantics are a stack, and we want that to be obvious to users")]
    public class RecoveryStack : IRecoveryStack
    {
        private Stack<IRequiresRecovery> recoveries = new Stack<IRequiresRecovery>();
        private object lockObj = new object();

        /// <summary>
        /// Add a new <see cref="IRequiresRecovery"/> object to this
        /// list.
        /// </summary>
        /// <param name="recovery">Object to add.</param>
        public void Add(IRequiresRecovery recovery)
        {
            Guard.ArgumentNotNull(recovery, "recovery");
            lock (lockObj)
            {
                recoveries.Push(recovery);
            }
        }

        /// <summary>
        /// Return the number of recovery objects currently in the stack.
        /// </summary>
        public int Count
        {
            get
            {
                lock (lockObj)
                {
                    return recoveries.Count;
                }
            }
        }

        /// <summary>
        /// Execute the <see cref="IRequiresRecovery.Recover"/> method
        /// of everything in the recovery list. Recoveries will execute
        /// in the opposite order of add - it's a stack.
        /// </summary>
        public void ExecuteRecovery()
        {
            while (recoveries.Count > 0)
            {
                recoveries.Pop().Recover();
            }
        }
    }
}
