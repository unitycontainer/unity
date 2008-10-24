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

namespace Microsoft.Practices.ObjectBuilder2
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
        /// Tests the given <paramref name="sequence"/>, returning true only if all
        /// elements in <paramref name="sequence"/> satisfies the given predicate.
        /// </summary>
        /// <typeparam name="T">Type of elements in sequence.</typeparam>
        /// <param name="sequence">Sequence to test.</param>
        /// <param name="pred">Predicate to use to test.</param>
        /// <returns>true if all elements satify pred, false if not.</returns>
        public static bool ForAll<T>(IEnumerable<T> sequence, Predicate<T> pred)
        {
            foreach(T item in sequence)
            {
                if(!pred(item))
                {
                    return false;
                }
            }
            return true;
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

        /// <summary>
        /// Concatenate multiple sequences into a single one.
        /// </summary>
        /// <typeparam name="TItem">Type of sequences in the sequence.</typeparam>
        /// <param name="sequences">The sequences to combine.</param>
        /// <returns>The combined sequence.</returns>
        public static IEnumerable<TItem> Concat<TItem>(params IEnumerable<TItem>[] sequences)
        {
            foreach(IEnumerable<TItem> sequence in sequences)
            {
                foreach(TItem item in sequence)
                {
                    yield return item;
                }
            }
        }
    }

    /// <summary>
    /// Static class containing constructor methods for
    /// instances of <see cref="Seq{T}"/>, so that we
    /// get type inference.
    /// </summary>
    public static class Seq
    {
        /// <summary>
        /// Make a new instance of <see cref="Seq{T}"/> that wraps
        /// the given items.
        /// </summary>
        /// <typeparam name="T">Type of items in the sequence.</typeparam>
        /// <param name="items">Items in the sequence.</param>
        /// <returns>The sequence.</returns>
        public static Seq<T> Make<T>(IEnumerable<T> items)
        {
            return new Seq<T>(items);
        }

        /// <summary>
        /// Make a new instance of <see cref="Seq{T}"/> that wraps
        /// the given items.
        /// </summary>
        /// <typeparam name="T">Type of items in the sequence.</typeparam>
        /// <param name="items">Items in the sequence.</param>
        /// <returns>The sequence.</returns>
        public static Seq<T> Collect<T>(params T[] items)
        {
            return new Seq<T>(items);
        }
    }

    /// <summary>
    /// And another helper class that makes it possible to chain sequence operations together.
    /// </summary>
    /// <typeparam name="T">Type of item contained in the sequence.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Adding Collection suffix destroys value of class")]
    public class Seq<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> items;

        /// <summary>
        /// Create a new <see cref="Seq{T}"/> instance wrapping the given IEnumerable.
        /// </summary>
        /// <param name="items">The sequence to wrap.</param>
        public Seq(IEnumerable<T> items)
        {
            this.items = items;
        }

        #region Implementation of IEnumerable

        /// <summary>
        ///                     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///                     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of IEnumerable<T>

        /// <summary>
        ///                     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///                     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion

        #region Sequence operations

        /// <summary>
        /// Given a sequence object, return a list containing those items.
        /// </summary>
        /// <returns>The materialized list.</returns>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "We explicitly want a List here")]
        public List<T> ToList()
        {
            return new List<T>(items);
        }

        /// <summary>
        /// Return an array with the same contents as this sequence.
        /// </summary>
        /// <returns>The materialized array.</returns>
        public T[] ToArray()
        {
            return ToList().ToArray();
        }

        /// <summary>
        /// Returns new sequence containing only the items for which the predicate is true.
        /// </summary>
        /// <param name="predicate">Test to indicate sequence inclusion</param>
        /// <returns>New sequence.</returns>
        public Seq<T> Where(Predicate<T> predicate)
        {
            return new Seq<T>(Sequence.Where(items, predicate));
        }

        /// <summary>
        /// Tests the sequence, returning true if any
        /// element satisfies the given predicate.
        /// </summary>
        /// <param name="predicate">Predicate to use to test for existence.</param>
        /// <returns>true if any elements satify pred, false if not.</returns>
        public bool Exists(Predicate<T> predicate)
        {
            return Sequence.Exists(items, predicate);
        }

        /// <summary>
        /// Tests the sequence, returning true only if all
        /// elements satisfy the given predicate.
        /// </summary>
        /// <param name="predicate">Predicate to use to test.</param>
        /// <returns>true if all elements satify pred, false if not.</returns>
        public bool ForAll(Predicate<T> predicate)
        {
            return Sequence.ForAll(items, predicate);
        }

        /// <summary>
        /// Return a new sequence consisting of the result of running each element through
        /// the given <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TOut">Desired output type.</typeparam>
        /// <param name="converter">Converter delegate.</param>
        /// <returns>New Sequence</returns>
        public Seq<TOut> Map<TOut>(Converter<T, TOut> converter)
        {
            return new Seq<TOut>(Sequence.Map(items, converter));
        }

        /// <summary>
        /// Run a functional Reduce operation. See other methods for examples.
        /// </summary>
        /// <typeparam name="TDest">Type of final output.</typeparam>
        /// <param name="initialValue">Initial value for accumulator.</param>
        /// <param name="reducer">Reduce function.</param>
        /// <returns></returns>
        public TDest Reduce<TDest>(TDest initialValue,
            Sequence.Reducer<T, TDest> reducer)
        {
            return Sequence.Reduce(items, initialValue, reducer);
        }

        /// <summary>
        /// Convert the given sequence to a single string. The items in the string are
        /// separated by the given separator, and each object is converted to a string
        /// using the <paramref name="converter"/> method given.
        /// </summary>
        /// <param name="separator">Separator string.</param>
        /// <param name="converter">Function to convert <typeparamref name="T"/> instances to
        /// strings.</param>
        /// <returns>The collected string.</returns>
        public string ToString(string separator, Converter<T, string> converter)
        {
            return Sequence.ToString(items, separator, converter);
        }

        /// <summary>
        /// Convert the given sequence to a single string. The items in the string are separated
        /// by the given separator, and each object is converted to a string by calling its
        /// <see cref="Object.ToString"/>  method.
        /// </summary>
        /// <param name="separator">Separator string.</param>
        /// <returns>The collected string.</returns>
        public string ToString(string separator)
        {
            return Sequence.ToString(items, separator, delegate(T item) { return item.ToString(); });
        }

        /// <summary>
        /// Return the first item in the given sequence.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the sequence is empty.</exception>
        /// <returns>First item in the sequence.</returns>
        public T First()
        {
            return Sequence.First(items);
        }

        /// <summary>
        /// Execute the given action delegate for each item in the sequence.
        /// </summary>
        /// <param name="action">Action to perform on each item.</param>
        public void ForEach(Action<T> action)
        {
            Sequence.ForEach(items, action);
        }

        /// <summary>
        /// Concatenate multiple sequences with this one to return a single
        /// sequence containing all items.
        /// </summary>
        /// <param name="sequences">Sequences to combine.</param>
        /// <returns>The combined sequence.</returns>
        public Seq<T> Concat(params IEnumerable<T>[] sequences)
        {
            IEnumerable<T>[] newSequences = new IEnumerable<T>[sequences.Length + 1];
            newSequences[0] = items;
            for (int i = 0; i < sequences.Length; ++i)
            {
                newSequences[i + 1] = sequences[i];
            }

            return new Seq<T>(Sequence.Concat(newSequences));
        }
        #endregion
    }
}
