// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// A series of helper methods to deal with sequences -
    /// objects that implement <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class Sequence
    {
        /// <summary>
        /// A function that turns an arbitrary parameter list into an
        /// <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of arguments.</typeparam>
        /// <param name="arguments">The items to put into the collection.</param>
        /// <returns>An array that contains the values of the <paramref name="arguments"/>.</returns>
        public static T[] Collect<T>(params T[] arguments)
        {
            return arguments;
        }

        /// <summary>
        /// Given two sequences, return a new sequence containing the corresponding values
        /// from each one.
        /// </summary>
        /// <typeparam name="TFirstSequenceElement">Type of first sequence.</typeparam>
        /// <typeparam name="TSecondSequenceElement">Type of second sequence.</typeparam>
        /// <param name="sequence1">First sequence of items.</param>
        /// <param name="sequence2">Second sequence of items.</param>
        /// <returns>New sequence of pairs. This sequence ends when the shorter of sequence1 and sequence2 does.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "No other way to do this")]
        public static IEnumerable<Pair<TFirstSequenceElement, TSecondSequenceElement>> Zip<TFirstSequenceElement, TSecondSequenceElement>(IEnumerable<TFirstSequenceElement> sequence1, IEnumerable<TSecondSequenceElement> sequence2)
        {
            var enum1 = sequence1.GetEnumerator();
            var enum2 = sequence2.GetEnumerator();

            while (enum1.MoveNext())
            {
                if (enum2.MoveNext())
                {
                    yield return new Pair<TFirstSequenceElement, TSecondSequenceElement>(enum1.Current, enum2.Current);
                }
                else
                {
                    yield break;
                }
            }
        }
    }
}
