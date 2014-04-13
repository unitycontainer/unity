// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// The almost inevitable collection of extra helper methods on
    /// <see cref="IEnumerable{T}"/> to augment the rich set of what
    /// LINQ already gives us.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Execute the provided <paramref name="action"/> on every item in <paramref name="sequence"/>.
        /// </summary>
        /// <typeparam name="TItem">Type of the items stored in <paramref name="sequence"/></typeparam>
        /// <param name="sequence">Sequence of items to process.</param>
        /// <param name="action">Code to run over each item.</param>
        //[SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class")]
        public static void ForEach<TItem>(this IEnumerable<TItem> sequence, Action<TItem> action)
        {
            Guard.ArgumentNotNull(sequence, "sequence");

            foreach (var item in sequence)
            {
                action(item);
            }
        }

        /// <summary>
        /// Create a single string from a sequence of items, separated by the provided <paramref name="separator"/>,
        /// and with the conversion to string done by the given <paramref name="converter"/>.
        /// </summary>
        /// <remarks>This method does basically the same thing as <see cref="string.Join(string,string[])"/>,
        /// but will work on any sequence of items, not just arrays.</remarks>
        /// <typeparam name="TItem">Type of items in the sequence.</typeparam>
        /// <param name="sequence">Sequence of items to convert.</param>
        /// <param name="separator">Separator to place between the items in the string.</param>
        /// <param name="converter">The conversion function to change TItem -&gt; string.</param>
        /// <returns>The resulting string.</returns>
        public static string JoinStrings<TItem>(this IEnumerable<TItem> sequence, string separator, Func<TItem, string> converter)
        {
            var sb = new StringBuilder();
            sequence.Aggregate(sb, (builder, item) =>
            {
                if (builder.Length > 0)
                {
                    builder.Append(separator);
                }
                builder.Append(converter(item));
                return builder;
            });
            return sb.ToString();
        }

        /// <summary>
        /// Create a single string from a sequence of items, separated by the provided <paramref name="separator"/>,
        /// and with the conversion to string done by the item's <see cref='object.ToString'/> method.
        /// </summary>
        /// <remarks>This method does basically the same thing as <see cref="string.Join(string,string[])"/>,
        /// but will work on any sequence of items, not just arrays.</remarks>
        /// <typeparam name="TItem">Type of items in the sequence.</typeparam>
        /// <param name="sequence">Sequence of items to convert.</param>
        /// <param name="separator">Separator to place between the items in the string.</param>
        /// <returns>The resulting string.</returns>
        public static string JoinStrings<TItem>(this IEnumerable<TItem> sequence, string separator)
        {
            return sequence.JoinStrings(separator, item => item.ToString());
        }
    }
}
