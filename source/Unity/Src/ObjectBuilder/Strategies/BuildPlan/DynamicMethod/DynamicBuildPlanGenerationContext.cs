// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicBuildPlanGenerationContext
    {
        private Type typeToBuild;
        private ParameterExpression contextParameter;
        private Queue<Expression> buildPlanExpressions;

        private static readonly MethodInfo ResolveDependency =
         StaticReflection.GetMethodInfo((IDependencyResolverPolicy r) => r.Resolve(null));

        private static readonly MethodInfo GetResolverMethod =
            StaticReflection.GetMethodInfo(() => GetResolver(null, null, null));

        private static readonly MemberInfo getBuildContextExistingObjectProperty =
            StaticReflection.GetMemberInfo((IBuilderContext c) => c.Existing);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeToBuild"></param>
        public DynamicBuildPlanGenerationContext(Type typeToBuild)
        {
            this.typeToBuild = typeToBuild;
            contextParameter = Expression.Parameter(typeof(IBuilderContext), "context");
            buildPlanExpressions = new Queue<Expression>();
        }

        /// <summary>
        /// The type that is to be built with the dynamic build plan.
        /// </summary>
        public Type TypeToBuild
        {
            get { return this.typeToBuild; }
        }

        /// <summary>
        /// The context parameter represeting the <see cref="IBuilderContext"/> used when the build plan is executed.
        /// </summary>
        public ParameterExpression ContextParameter
        {
            get { return this.contextParameter; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        public void AddToBuildPlan(Expression expression)
        {
            this.buildPlanExpressions.Enqueue(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterKey"></param>
        /// <param name="parameterType"></param>
        /// <param name="setOperationExpression"></param>
        /// <returns></returns>
        public Expression CreateParameterExpression(string parameterKey, Type parameterType, Expression setOperationExpression)
        {
            // This intend of this is to create an parameter resolving expression block.  The following
            // psuedo code will hopefully make it clearer as to what we're trying to accomplish (of course actual code
            // trumps comments):
            //  object priorOperation = context.CurrentOperation;
            //  SetCurrentOperation
            //  var resolver = GetResolver([context], [paramType], [key])
            //  var dependencyResult = resolver.ResolveDependency([context]);   
            //  context.CurrentOperation = priorOperation;
            //  dependencyResult ; // return item from Block

            ParameterExpression savedOperationExpression = Expression.Parameter(typeof(object));
            ParameterExpression resolvedObjectExpression = Expression.Parameter(parameterType);
            return
                Expression.Block(
                    new ParameterExpression[] { savedOperationExpression, resolvedObjectExpression },
                    SaveCurrentOperationExpression(savedOperationExpression),
                    setOperationExpression,
                    Expression.Assign(
                        resolvedObjectExpression,
                        GetResolveDependencyExpression(parameterType, parameterKey)),
                    RestoreCurrentOperationExpression(savedOperationExpression),
                    resolvedObjectExpression
                );
        }

        internal Expression GetExistingObjectExpression()
        {
            return Expression.MakeMemberAccess(ContextParameter,
                                                getBuildContextExistingObjectProperty);
        }

        internal Expression GetClearCurrentOperationExpression()
        {
            return Expression.Assign(
                               Expression.Property(ContextParameter, typeof(IBuilderContext).GetTypeInfo().GetDeclaredProperty("CurrentOperation")),
                               Expression.Constant(null));
        }

        internal Expression GetResolveDependencyExpression(Type dependencyType, string dependencyKey)
        {
            return Expression.Convert(
                           Expression.Call(
                               Expression.Call(null,
                                               GetResolverMethod,
                                               ContextParameter,
                                               Expression.Constant(dependencyType, typeof(Type)),
                                               Expression.Constant(dependencyKey, typeof(string))),
                               ResolveDependency,
                               ContextParameter),
                           dependencyType);
        }

        internal DynamicBuildPlanMethod GetBuildMethod()
        {
            Func<IBuilderContext, object> planDelegate =
                (Func<IBuilderContext, object>)
                Expression.Lambda(
                    Expression.Block(
                        buildPlanExpressions.Concat(new Expression[] { GetExistingObjectExpression() })),
                        ContextParameter)
                .Compile();

            return new DynamicBuildPlanMethod((context) =>
            {
                try
                {
                    context.Existing = planDelegate(context);
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            });
        }

        private Expression RestoreCurrentOperationExpression(ParameterExpression savedOperationExpression)
        {
            return Expression.Assign(
                Expression.MakeMemberAccess(
                    this.ContextParameter,
                    typeof(IBuilderContext).GetTypeInfo().GetDeclaredProperty("CurrentOperation")),
                    savedOperationExpression
                    );
        }

        private Expression SaveCurrentOperationExpression(ParameterExpression saveExpression)
        {
            return Expression.Assign(
                saveExpression,
                Expression.MakeMemberAccess(
                    this.ContextParameter,
                    typeof(IBuilderContext).GetTypeInfo().GetDeclaredProperty("CurrentOperation")));
        }

        /// <summary>
        /// Helper method used by generated IL to look up a dependency resolver based on the given key.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="dependencyType">Type of the dependency being resolved.</param>
        /// <param name="resolverKey">Key the resolver was stored under.</param>
        /// <returns>The found dependency resolver.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class")]
        public static IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType, string resolverKey)
        {
            Guard.ArgumentNotNull(context, "context");

            var resolver = context.GetOverriddenResolver(dependencyType);
            return resolver ?? context.Policies.Get<IDependencyResolverPolicy>(resolverKey);
        }
    }
}
