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
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Practices.Unity.Utility
{
    /// <summary>
    /// A set of helper methods to pick through lambdas and pull out
    /// <see cref="MethodInfo"/> from them.
    /// </summary>
    public static class StaticReflection
    {
        /// <summary>
        /// Pull out a <see cref="MethodInfo"/> object from an expression of the form
        /// () => SomeClass.SomeMethod()
        /// </summary>
        /// <param name="expression">Expression describing the method to call.</param>
        /// <returns>Corresponding <see cref="MethodInfo"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Lambda inference at the call site doesn't work without the derived type.")]
        public static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }

        /// <summary>
        /// Pull out a <see cref="MethodInfo"/> object from an expression of the form
        /// x => x.SomeMethod()
        /// </summary>
        /// <typeparam name="T">The type where the method is defined.</typeparam>
        /// <param name="expression">Expression describing the method to call.</param>
        /// <returns>Corresponding <see cref="MethodInfo"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Expressions require nested generics")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Lambda inference at the call site doesn't work without the derived type.")]
        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }

        private static MethodInfo GetMethodInfo(LambdaExpression lambda)
        {
            GuardProperExpressionForm(lambda.Body);

            var call = (MethodCallExpression)lambda.Body;
            return call.Method;
        }

        /// <summary>
        /// Pull out a <see cref="MethodInfo"/> object for the get method from an expression of the form
        /// x => x.SomeProperty
        /// </summary>
        /// <typeparam name="T">The type where the method is defined.</typeparam>
        /// <typeparam name="TProperty">The type for the property.</typeparam>
        /// <param name="expression">Expression describing the property for which the get method is to be extracted.</param>
        /// <returns>Corresponding <see cref="MethodInfo"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Expressions require nested generics")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Lambda inference at the call site doesn't work without the derived type.")]
        public static MethodInfo GetPropertyGetMethodInfo<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var property = GetPropertyInfo<T, TProperty>(expression);

            var getMethod = property.GetGetMethod();
            if (getMethod == null) throw new InvalidOperationException("Invalid expression form passed");

            return getMethod;
        }

        /// <summary>
        /// Pull out a <see cref="MethodInfo"/> object for the set method from an expression of the form
        /// x => x.SomeProperty
        /// </summary>
        /// <typeparam name="T">The type where the method is defined.</typeparam>
        /// <typeparam name="TProperty">The type for the property.</typeparam>
        /// <param name="expression">Expression describing the property for which the set method is to be extracted.</param>
        /// <returns>Corresponding <see cref="MethodInfo"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Expressions require nested generics")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Lambda inference at the call site doesn't work without the derived type.")]
        public static MethodInfo GetPropertySetMethodInfo<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var property = GetPropertyInfo<T, TProperty>(expression);

            var setMethod = property.GetSetMethod();
            if (setMethod == null) throw new InvalidOperationException("Invalid expression form passed");

            return setMethod;
        }

        private static PropertyInfo GetPropertyInfo<T, TProperty>(LambdaExpression lambda)
        {
            var body = lambda.Body as MemberExpression;
            if (body == null) throw new InvalidOperationException("Invalid expression form passed");

            var property = body.Member as PropertyInfo;
            if (property == null) throw new InvalidOperationException("Invalid expression form passed");

            return property;
        }

        /// <summary>
        /// Pull out a <see cref="ConstructorInfo"/> object from an expression of the form () => new SomeType()
        /// </summary>
        /// <typeparam name="T">The type where the constructor is defined.</typeparam>
        /// <param name="expression">Expression invoking the desired constructor.</param>
        /// <returns>Corresponding <see cref="ConstructorInfo"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Expressions require nested generics")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Lambda inference at the call site doesn't work without the derived type.")]
        public static ConstructorInfo GetConstructorInfo<T>(Expression<Func<T>> expression)
        {
            var body = expression.Body as NewExpression;
            if (body == null) throw new InvalidOperationException("Invalid expression form passed");

            return body.Constructor;
        }

        private static void GuardProperExpressionForm(Expression expression)
        {
            if (expression.NodeType != ExpressionType.Call)
            {
                throw new InvalidOperationException("Invalid expression form passed");
            }
        }
    }
}
