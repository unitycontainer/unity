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
using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// An implementation of <see cref="IParameterCollection"/> that wraps a provided array
    /// containing the argument values.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1035", Justification = "Not a general purpose collection")]
    [SuppressMessage("Microsoft.Design", "CA1039", Justification = "Not a general purpose collection")]
    public class ParameterCollection : IParameterCollection
    {
        /// <summary>
        /// An internal struct that maps the index in the arguments collection to the
        /// corresponding <see cref="ParameterInfo"/> about that argument.
        /// </summary>
        private struct ArgumentInfo
        {
            public int Index;
            public string Name;
            public ParameterInfo ParameterInfo;

            /// <summary>
            /// Construct a new <see cref="ArgumentInfo"/> object linking the
            /// given index and <see cref="ParameterInfo"/> object.
            /// </summary>
            /// <param name="index">Index into arguments array (zero-based).</param>
            /// <param name="parameterInfo"><see cref="ParameterInfo"/> for the argument at <paramref name="index"/>.</param>
            public ArgumentInfo(int index, ParameterInfo parameterInfo)
            {
                this.Index = index;
                this.Name = parameterInfo.Name;
                this.ParameterInfo = parameterInfo;
            }
        }

        private readonly List<ArgumentInfo> argumentInfo;
        private readonly object[] arguments;

        /// <summary>
        /// Construct a new <see cref="ParameterCollection"/> that wraps the
        /// given array of arguments.
        /// </summary>
        /// <param name="arguments">Complete collection of arguments.</param>
        /// <param name="argumentInfo">Type information about about each parameter.</param>
        /// <param name="isArgumentPartOfCollection">A <see cref="Predicate{ParameterInfo}"/> that indicates
        /// whether a particular parameter is part of the collection. Used to filter out only input
        /// parameters, for example.</param>
        public ParameterCollection(object[] arguments, ParameterInfo[] argumentInfo, Predicate<ParameterInfo> isArgumentPartOfCollection)
        {
            this.arguments = arguments;
            this.argumentInfo = new List<ArgumentInfo>();
            for (int argumentNumber = 0; argumentNumber < argumentInfo.Length; ++argumentNumber)
            {
                if (isArgumentPartOfCollection(argumentInfo[argumentNumber]))
                {
                    this.argumentInfo.Add(new ArgumentInfo(argumentNumber, argumentInfo[argumentNumber]));
                }
            }
        }

        /// <summary>
        /// Fetches a parameter's value by name.
        /// </summary>
        /// <param name="parameterName">parameter name.</param>
        /// <value>value of the named parameter.</value>
        public object this[string parameterName]
        {
            get { return arguments[IndexForInputParameterName(parameterName)]; }

            set { arguments[IndexForInputParameterName(parameterName)] = value; }
        }

        private int IndexForInputParameterName(string paramName)
        {
            for (int i = 0; i < argumentInfo.Count; ++i)
            {
                if (argumentInfo[i].Name == paramName)
                {
                    return i;
                }
            }
            throw new ArgumentException("Invalid parameter Name", "paramName");
        }

        /// <summary>
        /// Gets the name of a parameter based on index.
        /// </summary>
        /// <param name="index">Index of parameter to get the name for.</param>
        /// <value>Name of the requested parameter.</value>
        public object this[int index]
        {
            get { return arguments[argumentInfo[index].Index]; }
            set { arguments[argumentInfo[index].Index] = value; }
        }

        /// <summary>
        /// Gets the ParameterInfo for a particular parameter by index.
        /// </summary>
        /// <param name="index">Index for this parameter.</param>
        /// <returns>ParameterInfo object describing the parameter.</returns>
        public ParameterInfo GetParameterInfo(int index)
        {
            return argumentInfo[index].ParameterInfo;
        }

        /// <summary>
        /// Gets the <see cref="ParameterInfo"/> for the given named parameter.
        /// </summary>
        /// <param name="parameterName">Name of parameter.</param>
        /// <returns><see cref="ParameterInfo"/> for the requested parameter.</returns>
        public ParameterInfo GetParameterInfo(string parameterName)
        {
            return argumentInfo[IndexForInputParameterName(parameterName)].ParameterInfo;
        }

        /// <summary>
        /// Gets the name of a parameter based on index.
        /// </summary>
        /// <param name="index">Index of parameter to get the name for.</param>
        /// <returns>Name of the requested parameter.</returns>
        public string ParameterName(int index)
        {
            return argumentInfo[index].Name;
        }

        /// <summary>
        /// Adds to the collection. This is a read only collection, so this method
        /// always throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="value">Object to add.</param>
        /// <returns>Nothing, always throws.</returns>
        /// <exception cref="NotSupportedException">Always throws this.</exception>
        public int Add(object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Checks to see if the collection contains the given object.
        /// </summary>
        /// <remarks>Tests for the object using object.Equals.</remarks>
        /// <param name="value">Object to find.</param>
        /// <returns>true if object is in collection, false if it is not.</returns>
        public bool Contains(object value)
        {
            return
                argumentInfo.Exists(
                    delegate(ArgumentInfo info) { return arguments[info.Index].Equals(value); });
        }

        /// <summary>
        /// Remove all items in the collection. This collection is fixed-size, so this
        /// method always throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">This is always thrown.</exception>
        public void Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the index of the given object, or -1 if not found.
        /// </summary>
        /// <param name="value">Object to find.</param>
        /// <returns>zero-based index of found object, or -1 if not found.</returns>
        public int IndexOf(object value)
        {
            return argumentInfo.FindIndex(
                delegate(ArgumentInfo info)
                {
                    return arguments[info.Index].Equals(value);
                }
                );
        }

        /// <summary>
        /// Inserts a new item. This is a fixed-size collection, so this method throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="index">Index to insert at.</param>
        /// <param name="value">Always throws.</param>
        /// <exception cref="NotSupportedException">Always throws this.</exception>
        public void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes the given item. This is a fixed-size collection, so this method throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="value">Always throws.</param>
        /// <exception cref="NotSupportedException">Always throws this.</exception>
        public void Remove(object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes the given item. This is a fixed-size collection, so this method throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="index">Always throws.</param>
        /// <exception cref="NotSupportedException">Always throws this.</exception>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Is this collection read only?
        /// </summary>
        /// <value>No, it is not read only, the contents can change.</value>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Is this collection fixed size?
        /// </summary>
        /// <value>Yes, it is.</value>
        public bool IsFixedSize
        {
            get { return true; }
        }

        /// <summary>
        /// Copies the contents of this collection to the given array.
        /// </summary>
        /// <param name="array">Destination array.</param>
        /// <param name="index">index to start copying from.</param>
        public void CopyTo(Array array, int index)
        {
            int destIndex = 0;
            argumentInfo.GetRange(index, argumentInfo.Count - index).ForEach(
                delegate(ArgumentInfo info)
                {
                    array.SetValue(arguments[info.Index], destIndex);
                    ++destIndex;
                }
                );
        }

        /// <summary>
        /// Total number of items in the collection.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return argumentInfo.Count; }
        }

        /// <summary>
        /// Gets a synchronized version of this collection. WARNING: Not implemented completely,
        /// DO NOT USE THIS METHOD.
        /// </summary>
        public object SyncRoot
        {
            get { return this; }
        }

        /// <summary>
        /// Is the object synchronized for thread safety?
        /// </summary>
        /// <value>No, it isn't.</value>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Gets an enumerator object to support the foreach construct.
        /// </summary>
        /// <returns>Enumerator object.</returns>
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < argumentInfo.Count; ++i)
            {
                yield return arguments[argumentInfo[i].Index];
            }
        }
    }
}
