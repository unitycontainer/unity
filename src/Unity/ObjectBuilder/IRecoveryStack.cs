// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


namespace ObjectBuilder2
{
    /// <summary>
    /// Data structure that stores the set of <see cref="IRequiresRecovery"/>
    /// objects and executes them when requested.
    /// </summary>
    // FxCop suppression: The name ends in stack becuase the semantics are a stack,
    // and we want that to be obvious to users.
    public interface IRecoveryStack
    {
        /// <summary>
        /// Add a new <see cref="IRequiresRecovery"/> object to this
        /// list.
        /// </summary>
        /// <param name="recovery">Object to add.</param>
        void Add(IRequiresRecovery recovery);

        /// <summary>
        /// Return the number of recovery objects currently in the stack.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Execute the <see cref="IRequiresRecovery.Recover"/> method
        /// of everything in the recovery list. Recoveries will execute
        /// in the opposite order of add - it's a stack.
        /// </summary>
        void ExecuteRecovery();
    }
}
