// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents a chain of responsibility for builder strategies partitioned by stages.
    /// </summary>
    /// <typeparam name="TStageEnum">The stage enumeration to partition the strategies.</typeparam>
    public class StagedStrategyChain<TStageEnum> : IStagedStrategyChain
    {
        private readonly StagedStrategyChain<TStageEnum> innerChain;
        private readonly object lockObject = new object();
        private readonly List<IBuilderStrategy>[] stages;

        /// <summary>
        /// Initialize a new instance of the <see cref="StagedStrategyChain{TStageEnum}"/> class.
        /// </summary>
        public StagedStrategyChain()
        {
            stages = new List<IBuilderStrategy>[NumberOfEnumValues()];

            for (int i = 0; i < stages.Length; ++i)
            {
                stages[i] = new List<IBuilderStrategy>();
            }
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="StagedStrategyChain{TStageEnum}"/> class with an inner strategy chain to use when building.
        /// </summary>
        /// <param name="innerChain">The inner strategy chain to use first when finding strategies in the build operation.</param>
        public StagedStrategyChain(StagedStrategyChain<TStageEnum> innerChain)
            : this()
        {
            this.innerChain = innerChain;
        }

        /// <summary>
        /// Adds a strategy to the chain at a particular stage.
        /// </summary>
        /// <param name="strategy">The strategy to add to the chain.</param>
        /// <param name="stage">The stage to add the strategy.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
            Justification = "We're converting an enum to an int, no need for globalization here.")]
        public void Add(IBuilderStrategy strategy,
                        TStageEnum stage)
        {
            lock (lockObject)
            {
                stages[Convert.ToInt32(stage)].Add(strategy);
            }
        }

        /// <summary>
        /// Add a new strategy for the <paramref name="stage"/>.
        /// </summary>
        /// <typeparam name="TStrategy">The <see cref="Type"/> of <see cref="IBuilderStrategy"/></typeparam>
        /// <param name="stage">The stage to add the strategy.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
            Justification = "This is not a new version of Add, it adds a new strategy")]
        public void AddNew<TStrategy>(TStageEnum stage)
            where TStrategy : IBuilderStrategy, new()
        {
            Add(new TStrategy(), stage);
        }

        /// <summary>
        /// Clear the current strategy chain list.
        /// </summary>
        /// <remarks>
        /// This will not clear the inner strategy chain if this instance was created with one.
        /// </remarks>
        public void Clear()
        {
            lock (lockObject)
            {
                foreach (List<IBuilderStrategy> stage in stages)
                {
                    stage.Clear();
                }
            }
        }

        /// <summary>
        /// Makes a strategy chain based on this instance.
        /// </summary>
        /// <returns>A new <see cref="StrategyChain"/>.</returns>
        public IStrategyChain MakeStrategyChain()
        {
            lock (lockObject)
            {
                StrategyChain result = new StrategyChain();

                for (int index = 0; index < stages.Length; ++index)
                {
                    FillStrategyChain(result, index);
                }

                return result;
            }
        }

        private void FillStrategyChain(StrategyChain chain, int index)
        {
            lock (lockObject)
            {
                if (innerChain != null)
                {
                    innerChain.FillStrategyChain(chain, index);
                }
                chain.AddRange(stages[index]);
            }
        }

        private static int NumberOfEnumValues()
        {
            return typeof(TStageEnum).GetTypeInfo().DeclaredFields.Where(f => f.IsPublic && f.IsStatic).Count();
        }
    }
}
