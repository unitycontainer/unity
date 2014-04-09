// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// This interface defines a standard method to convert any <see cref="StagedStrategyChain{TStageEnum}"/> regardless
    /// of the stage enum into a regular, flat strategy chain.
    /// </summary>
    public interface IStagedStrategyChain
    {
        /// <summary>
        /// Convert this <see cref="StagedStrategyChain{TStageEnum}"/> into
        /// a flat <see cref="IStrategyChain"/>.
        /// </summary>
        /// <returns>The flattened <see cref="IStrategyChain"/>.</returns>
        IStrategyChain MakeStrategyChain();
    }
}
