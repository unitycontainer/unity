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
        /// x => x.Foo()
        /// </summary>
        /// <param name="expression">Expression describing the method to call.</param>
        /// <returns>Corresponding <see cref="MethodInfo"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Lambda inference at the call site doesn't work without the derived type.")]
        public static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            return GetMethodInfo((Expression) expression);
        }

        private static MethodInfo GetMethodInfo(Expression expression)
        {
            var lambda = (LambdaExpression) expression;
            GuardProperExpressionForm(lambda.Body);

            var call = (MethodCallExpression) lambda.Body;
            return call.Method;
        }

        private static void GuardProperExpressionForm(Expression expression)
        {
            if(expression.NodeType != ExpressionType.Call)
            {
                throw new InvalidOperationException("Invalid expression form passed");
            }
        }
    }
}
