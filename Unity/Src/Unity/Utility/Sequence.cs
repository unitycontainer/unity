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
using System.Text;

namespace Microsoft.Practices.Unity.Utility
{
    /// <summary>
    /// A series of helper methods to deal with sequences -
    /// objects that implement <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <remarks>LINQ in C# 3.0 does pretty much the same stuff,
    /// but we're keeping C# 2.0 compatibility here.</remarks>
    public static class Sequence
    {
        /// <summary>
        /// Given an <see cref="IEnumerable{T}"/>, return a
        /// new <see cref="List{T}"/> containing the same contents.
        /// </summary>
        /// <typeparam name="T">Type of item store in the sequence.</typeparam>
        /// <param name="seq">Sequence to create list from.</param>
        /// <returns>The new <see cref="List{T}"/></returns>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists",
            Justification = "I specifically want a List<T> here")]
        public static List<T> ToList<T>(IEnumerable<T> seq)
        {
            return new List<T>(seq);
        }

        /// <summary>
        /// Given an <see cref="IEnumerable"/> return a new
        /// <see cref="IEnumerable{TResult}"/> that contains
        /// all the objects in <paramref name="source"/> that
        /// are castable to <typeparamref name="TResult"/>.
        /// </summary>
        /// <typeparam name="TResult">Desired type for objects.</typeparam>
        /// <param name="source">Input sequence.</param>
        /// <returns>New output sequence.</returns>
        public static IEnumerable<TResult> OfType<TResult>(IEnumerable source)
        {
            foreach(object o in source)
            {
                if(o is TResult)
                {
                    yield return (TResult)o;
                }
            }
        }

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
        /// Create an array containing the elements of the given sequence.
        /// </summary>
        /// <typeparam name="T">Type of sequence and array.</typeparam>
        /// <param name="sequence">Input sequence.</param>
        /// <returns>The resulting array.</returns>
        public static T[] ToArray<T>(IEnumerable<T> sequence)
        {
            List<T> result = new List<T>(sequence);
            return result.ToArray();
        }


        /// <summary>
        /// Given a sequence of <typeparamref name="T"/>, returns a sequence
        /// containing those elements that satisfy the given predicate <paramref name="pred"/>.
        /// </summary>
        /// <typeparam name="T">Type of items in the sequence.</typeparam>
        /// <param name="sequence">Source sequence.</param>
        /// <param name="pred">Predicate used to test which items match.</param>
        /// <returns>The sequence of items that satify <paramref name="pred"/>. This
        /// sequence may be empty.</returns>
        public static IEnumerable<T> Where<T>(IEnumerable<T> sequence, Predicate<T> pred)
        {
            foreach(T item in sequence)
            {
                if(pred(item))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Tests the given <paramref name="sequence"/>, returning true if any
        /// element in <paramref name="sequence"/> satisfies the given predicate.
        /// </summary>
        /// <typeparam name="T">Type of elements in sequence.</typeparam>
        /// <param name="sequence">Sequence to test.</param>
        /// <param name="pred">Predicate to use to test for existence.</param>
        /// <returns>true if any elements satify pred, false if not.</returns>
        public static bool Exists<T>(IEnumerable<T> sequence, Predicate<T> pred)
        {
            foreach(T item in sequence)
            {
                if(pred(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Given a sequence of <typeparamref name="TIn"/>, returns a sequence of
        /// <typeparamref name="TOut"/> created by running the items in <paramref name="input"/>
        /// through <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TIn">Type of items in input.</typeparam>
        /// <typeparam name="TOut">Type of items in output.</typeparam>
        /// <param name="input">Input sequence.</param>
        /// <param name="converter">Mapping function.</param>
        /// <returns>The converted output sequence.</returns>
        public static IEnumerable<TOut> Map<TIn, TOut>(IEnumerable<TIn> input, Converter<TIn, TOut> converter)
        {
            foreach(TIn item in input)
            {
                yield return converter(item);
            }
        }

        /// <summary>
        /// A delegate that defines the function passed to the <see cref="Sequence.Reduce{TSource, TDest}"/> methods.
        /// </summary>
        /// <typeparam name="TSource">Type of item being reduced.</typeparam>
        /// <typeparam name="TDest">Type of the accumulator object.</typeparam>
        /// <param name="currentItem">Current item to process.</param>
        /// <param name="accumulator">Current value of the accumulator.</param>
        /// <returns></returns>
        public delegate TDest Reducer<TSource, TDest>(TSource currentItem, TDest accumulator);

        /// <summary>
        /// Run a functional Reduce operation. See other methods for examples.
        /// </summary>
        /// <typeparam name="TSource">Type of inputs.</typeparam>
        /// <typeparam name="TDest">Type of final output.</typeparam>
        /// <param name="sequence">Sequence of input items.</param>
        /// <param name="initialValue">Initial value for accumulator.</param>
        /// <param name="reducer">Reduce function.</param>
        /// <returns></returns>
        public static TDest Reduce<TSource, TDest>(IEnumerable<TSource> sequence,
            TDest initialValue,
            Reducer<TSource, TDest> reducer)
        {
            TDest accumulator = initialValue;
            foreach(TSource item in sequence)
            {
                accumulator = reducer(item, accumulator);
            }
            return accumulator;
        }

        /// <summary>
        /// Convert the given sequence to a single string. The items in the string are
        /// separated by the given separator, and each object is converted to a string
        /// using the <paramref name="converter"/> method given.
        /// </summary>
        /// <typeparam name="T">Type of input sequence.</typeparam>
        /// <param name="sequence">Input sequence.</param>
        /// <param name="separator">Separator string.</param>
        /// <param name="converter">Function to convert <typeparamref name="T"/> instances to
        /// strings.</param>
        /// <returns>The collected string.</returns>
        public static string ToString<T>(IEnumerable<T> sequence, string separator, Converter<T, string> converter)
        {
            StringBuilder sb = new StringBuilder();
            Reduce(sequence,
                sb,
                delegate(T item, StringBuilder accumulator)
                {
                    if(accumulator.Length > 0)
                    {
                        accumulator.Append(separator);
                    }
                    accumulator.Append(converter(item));
                    return accumulator;
                });
            return sb.ToString();
        }

        /// <summary>
        /// Convert the given sequence to a single string. The items in the string are separated
        /// by the given separator, and each object is converted to a string by calling its
        /// <see cref="Object.ToString"/>  method.
        /// </summary>
        /// <typeparam name="T">Type of input sequence.</typeparam>
        /// <param name="sequence">Input sequence.</param>
        /// <param name="separator">Separator string.</param>
        /// <returns>The collected string.</returns>
        public static string ToString<T>(IEnumerable<T> sequence, string separator)
        {
            return ToString(sequence, separator, delegate(T item) { return item.ToString(); });
        }

        /// <summary>
        /// Return the first item in the given sequence.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the sequence is empty.</exception>
        /// <typeparam name="T">Type of items in the sequence.</typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <returns>First item in the sequence.</returns>
        public static T First<T>(IEnumerable<T> sequence)
        {
            foreach(T item in sequence)
            {
                return item;
            }
            throw new InvalidOperationException("Sequence has no items");
        }

        /// <summary>
        /// Execute the given action delegate for each item in the sequence.
        /// </summary>
        /// <typeparam name="TItem">Type of item in the sequence.</typeparam>
        /// <param name="sequence">The sequence of items.</param>
        /// <param name="action">Action to perform on each item.</param>
        public static void ForEach<TItem>(IEnumerable<TItem> sequence, Action<TItem> action)
        {
            foreach(TItem item in sequence)
            {
                action(item);
            }
        }
    }
}
