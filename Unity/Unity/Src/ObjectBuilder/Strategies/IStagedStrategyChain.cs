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
    /// This interface defines a standard method to convert any 
    /// <see cref="StagedStrategyChain{TStageEnum}"/> regardless
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
