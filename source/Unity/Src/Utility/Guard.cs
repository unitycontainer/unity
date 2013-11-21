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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Globalization;
using Microsoft.Practices.Unity.Properties;

namespace Microsoft.Practices.Unity.Utility
{
    /// <summary>
    /// A static helper class that includes various parameter checking routines.
    /// </summary>
    public static partial class Guard
    {
        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the given argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException"> if tested value if null.</exception>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="argumentName">Name of the argument being tested.</param>
        public static void ArgumentNotNull(object argumentValue,
                                           string argumentName)
        {
            if (argumentValue == null) throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        /// Throws an exception if the tested string argument is null or the empty string.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if string value is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the string is empty</exception>
        /// <param name="argumentValue">Argument value to check.</param>
        /// <param name="argumentName">Name of argument being checked.</param>
        public static void ArgumentNotNullOrEmpty(string argumentValue,
                                                  string argumentName)
        {
            if (argumentValue == null) throw new ArgumentNullException(argumentName);
            if (argumentValue.Length == 0) throw new ArgumentException(Resources.ArgumentMustNotBeEmpty, argumentName);
        }

        /// <summary>
        /// Verifies that an argument type is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy).
        /// </summary>
        /// <param name="assignmentTargetType">The argument type that will be assigned to.</param>
        /// <param name="assignmentValueType">The type of the value being assigned.</param>
        /// <param name="argumentName">Argument name.</param>
        public static void TypeIsAssignable(Type assignmentTargetType, Type assignmentValueType, string argumentName)
        {
            if (assignmentTargetType == null) throw new ArgumentNullException("assignmentTargetType");
            if (assignmentValueType == null) throw new ArgumentNullException("assignmentValueType");

            if (!assignmentTargetType.GetTypeInfo().IsAssignableFrom(assignmentValueType.GetTypeInfo()))
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.TypesAreNotAssignable,
                    assignmentTargetType,
                    assignmentValueType),
                    argumentName);
            }
        }

        /// <summary>
        /// Verifies that an argument instance is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy, or instance can be 
        /// assigned through a runtime wrapper, as is the case for COM Objects).
        /// </summary>
        /// <param name="assignmentTargetType">The argument type that will be assigned to.</param>
        /// <param name="assignmentInstance">The instance that will be assigned.</param>
        /// <param name="argumentName">Argument name.</param>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "GetType() invoked for diagnostics purposes")]
        public static void InstanceIsAssignable(Type assignmentTargetType, object assignmentInstance, string argumentName)
        {
            if (assignmentTargetType == null) throw new ArgumentNullException("assignmentTargetType");
            if (assignmentInstance == null) throw new ArgumentNullException("assignmentInstance");

#if NETFX_CORE
            if (!assignmentTargetType.GetTypeInfo().IsAssignableFrom(assignmentInstance.GetType().GetTypeInfo()))
#else
            if (!assignmentTargetType.IsInstanceOfType(assignmentInstance))            
#endif            
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.TypesAreNotAssignable,
                        assignmentTargetType,
                        GetTypeName(assignmentInstance)),
                    argumentName);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Need to use exception as flow control here, no other choice")]
        private static string GetTypeName(object assignmentInstance)
        {
            string assignmentInstanceType;
            try
            {
                assignmentInstanceType = assignmentInstance.GetType().FullName;
            }
            catch (Exception)
            {
                assignmentInstanceType = Resources.UnknownType;
            }
            return assignmentInstanceType;
        }
    }
}
