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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents a chain of responsibility for builder strategies.
    /// </summary>
    // FxCop suppression: See IStrategyChain
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class StrategyChain : IStrategyChain
	{
		readonly List<IBuilderStrategy> strategies = new List<IBuilderStrategy>();

        /// <summary>
        /// Initialzie a new instance of the <see cref="StrategyChain"/> class.
        /// </summary>
		public StrategyChain() {}

        /// <summary>
        /// Initialzie a new instance of the <see cref="StrategyChain"/> class with a colleciton of strategies.
        /// </summary>
        /// <param name="strategies">A collection of strategies to initialize the chain.</param>
		public StrategyChain(IEnumerable strategies)
		{
			AddRange(strategies);
		}

        /// <summary>
        /// Adds a strategy to the chain.
        /// </summary>
        /// <param name="strategy">The strategy to add to the chain.</param>
		public void Add(IBuilderStrategy strategy)
		{
			strategies.Add(strategy);
		}

        /// <summary>
        /// Adds strategies to the chain.
        /// </summary>
        /// <param name="strategyEnumerable">The strategies to add to the chain.</param>
        // FxCop suppression: validation is done by Guard class.
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public void AddRange(IEnumerable strategyEnumerable)
		{
            Guard.ArgumentNotNull(strategyEnumerable, "strategyEnumerable");
			foreach (IBuilderStrategy strategy in strategyEnumerable)
				Add(strategy);
		}

        /// <summary>
        /// Reverse the order of the strategy chain.
        /// </summary>
        /// <returns>The reversed strategy chain.</returns>
		public IStrategyChain Reverse()
		{
			List<IBuilderStrategy> reverseList = new List<IBuilderStrategy>(strategies);
			reverseList.Reverse();
			return new StrategyChain(reverseList);
		}


        /// <summary>
        /// Execute this strategy chain against the given context to build up.
        /// </summary>
        /// <param name="context">Context for the build processes.</param>
        /// <returns>The build up object</returns>
        public object ExecuteBuildUp(IBuilderContext context)
        {
            int i = 0;
            try
            {
                for(; i < strategies.Count; ++i)
                {
                    if(context.BuildComplete)
                    {
                        break;
                    }
                    strategies[i].PreBuildUp(context);
                }

                if(context.BuildComplete)
                {
                    --i; // skip shortcutting strategy's post
                }

                for(--i; i >= 0; --i)
                {
                    strategies[i].PostBuildUp(context);
                }
                return context.Existing;
            }
            catch(Exception ex)
            {
                context.RecoveryStack.ExecuteRecovery();
                throw new BuildFailedException(
                    strategies[i], i, context.BuildKey, ex);
            }
        }

        /// <summary>
        /// Execute this strategy chain against the given context,
        /// calling the TearDown methods on the strategies.
        /// </summary>
        /// <param name="context">Context for the teardown process.</param>
        public void ExecuteTearDown(IBuilderContext context)
        {
            int i = 0;

            try
            {
                for (; i < strategies.Count; ++i)
                {
                    if (context.BuildComplete)
                    {
                        --i; // Skip current strategy's post
                        break;
                    }
                    strategies[i].PreTearDown(context);
                }

                for (--i; i >= 0; --i)
                {
                    strategies[i].PostTearDown(context);
                }
            }
            catch(Exception ex)
            {
                context.RecoveryStack.ExecuteRecovery();
                throw new BuildFailedException(
                    strategies[i], i, context.BuildKey, ex);
            }
        }

        #region IEnumerable<IBuilderStrategy> Members

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        IEnumerator<IBuilderStrategy> IEnumerable<IBuilderStrategy>.GetEnumerator()
        {
            return strategies.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public IEnumerator GetEnumerator()
        {
            return strategies.GetEnumerator();
        }

        #endregion
	}
}
