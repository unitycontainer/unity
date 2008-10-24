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
using System.Reflection;
using System.Text;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A utility class that takes a set of <see cref="MethodInfo"/>s
    /// and pulls out shadowed methods, only returning the ones that
    /// are actually accessible to be overriden.
    /// </summary>
    class MethodSorter : IEnumerable<MethodInfo>
    {
        readonly Dictionary<string, List<MethodInfo>> methodsByName = new Dictionary<string, List<MethodInfo>>();
        private readonly Type declaringType;
        
        public MethodSorter(Type declaringType, IEnumerable<MethodInfo> methodsToSort)
        {
            this.declaringType = declaringType;
            GroupMethodsByName(methodsToSort);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<MethodInfo> GetEnumerator()
        {
            foreach (KeyValuePair<string, List<MethodInfo>> methodList in methodsByName)
            {
                if (methodList.Value.Count == 1)
                {
                    yield return methodList.Value[0];
                }
                else
                {
                    foreach (MethodInfo method in RemoveHiddenOverloads(methodList.Value))
                    {
                        yield return method;
                    }
                }
            }            
        }

        /// <summary>
        /// Take the list of methods and put them together into lists index by method name.
        /// </summary>
        /// <param name="methodsToSort">Methods to sort through.</param>
        private void GroupMethodsByName(IEnumerable<MethodInfo> methodsToSort)
        {
            foreach (MethodInfo method in methodsToSort)
            {
                if (!methodsByName.ContainsKey(method.Name))
                {
                    methodsByName[method.Name] = new List<MethodInfo>();
                }
                methodsByName[method.Name].Add(method);
            }
            
        }

        /// <summary>
        /// Given a list of overloads for a method, return only those methods
        /// that are actually visible. In other words, if there's a "new foo" method
        /// somewhere, return only the new one, not the one from the base class
        /// that's now hidden.
        /// </summary>
        /// <param name="methods">Sequence of methods to process.</param>
        /// <returns>Sequence of returned methods.</returns>
        private IEnumerable<MethodInfo> RemoveHiddenOverloads(IEnumerable<MethodInfo> methods)
        {
            // Group the methods by signature
            List<MethodInfo> methodsByParameters = new List<MethodInfo>(methods);
            methodsByParameters.Sort(CompareMethodInfosByParameterLists);
            List<List<MethodInfo>> overloadGroups = new List<List<MethodInfo>>(GroupOverloadedMethods(methodsByParameters));

            foreach (List<MethodInfo> overload in overloadGroups)
            {
                yield return SelectMostDerivedOverload(overload);
            }

        }
        
        /// <summary>
        /// Take a semi-randomly ordered set of methods on a type and
        /// sort them into groups by name and by parameter list.
        /// </summary>
        /// <param name="sortedMethods">The list of methods.</param>
        /// <returns>Sequence of lists of methods, grouped by method name.</returns>
        private static IEnumerable<List<MethodInfo>> GroupOverloadedMethods(IList<MethodInfo> sortedMethods)
        {
            int index = 0;
            while (index < sortedMethods.Count)
            {
                int overloadStart = index;
                List<MethodInfo> overloads = new List<MethodInfo>();
                overloads.Add(sortedMethods[overloadStart]);
                ++index;
                while (index < sortedMethods.Count &&
                    CompareMethodInfosByParameterLists(sortedMethods[overloadStart], sortedMethods[index]) == 0)
                {
                    overloads.Add(sortedMethods[index++]);
                }

                yield return overloads;
            }
        }

        /// <summary>
        /// Given a set of hiding overloads, return only the currently visible one.
        /// </summary>
        /// <param name="overloads">The set of overloads.</param>
        /// <returns>The most visible one.</returns>
        private MethodInfo SelectMostDerivedOverload(IList<MethodInfo> overloads)
        {
            if (overloads.Count == 1)
            {
                return overloads[0];
            }

            int minDepth = int.MaxValue;
            MethodInfo selectedMethod = null;
            foreach (MethodInfo method in overloads)
            {
                int thisDepth = DeclarationDepth(method);
                if (thisDepth < minDepth)
                {
                    minDepth = thisDepth;
                    selectedMethod = method;
                }
            }

            return selectedMethod;
        }

        /// <summary>
        /// Given a method, return a value indicating how deeply in the
        /// inheritance hierarchy the method is declared. Current type = 0,
        /// parent = 1, grandparent = 2, etc.
        /// </summary>
        /// <param name="method">Method to check.</param>
        /// <returns>Declaration depth</returns>
        private int DeclarationDepth(MethodInfo method)
        {
            int depth = 0;
            Type currentType = declaringType;
            while (currentType != null && method.DeclaringType != declaringType)
            {
                ++depth;
                currentType = currentType.BaseType;
            }
            return depth;
        }

        /// <summary>
        /// A <see cref="Comparison{T}"/> implementation that can compare two <see cref="MethodInfo"/>
        /// based on their parameter lists.
        /// </summary>
        /// <param name="left">First <see cref="MethodInfo"/> to compare.</param>
        /// <param name="right">Second <see cref="MethodInfo"/> to compare.</param>
        /// <returns>&lt; 0, 0, or &gt; 0 based on which one is "greater" than the other.</returns>
        private static int CompareMethodInfosByParameterLists(MethodInfo left, MethodInfo right)
        {
            return CompareParameterLists(left.GetParameters(), right.GetParameters());
        }

        /// <summary>
        /// Compare two parameter lists.
        /// </summary>
        /// <param name="left">First parameter list.</param>
        /// <param name="right">Second parameter list.</param>
        /// <returns>&lt; 0, 0, or &gt; 0.</returns>
        private static int CompareParameterLists(ParameterInfo[] left, ParameterInfo[] right)
        {
            if (left.Length != right.Length)
            {
                return left.Length - right.Length;
            }

            for (int i = 0; i < left.Length; ++i)
            {
                int comparison = CompareParameterInfo(left[i], right[i]);
                if (comparison != 0)
                {
                    return comparison;
                }
            }
            return 0;
        }

        /// <summary>
        /// Compare two <see cref="ParameterInfo"/> objects by type.
        /// </summary>
        /// <param name="left">First <see cref="ParameterInfo"/></param>
        /// <param name="right">First <see cref="ParameterInfo"/></param>
        /// <returns>&lt; 0, 0, or &gt; 0</returns>
        private static int CompareParameterInfo(ParameterInfo left, ParameterInfo right)
        {
            if (left.ParameterType == right.ParameterType)
            {
                return 0;
            }
            return left.ParameterType.FullName.CompareTo(right.ParameterType.FullName);
        }

    }
}
