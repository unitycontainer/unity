// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Properties;
using Unity.Utility;

namespace ObjectBuilder2
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that generates IL to resolve properties
    /// on an object being built.
    /// </summary>
    public class DynamicMethodPropertySetterStrategy : BuilderStrategy
    {
        private static readonly MethodInfo SetCurrentOperationToResolvingPropertyValueMethod =
            StaticReflection.GetMethodInfo(() => SetCurrentOperationToResolvingPropertyValue(null, null));

        private static readonly MethodInfo SetCurrentOperationToSettingPropertyMethod =
            StaticReflection.GetMethodInfo(() => SetCurrentOperationToSettingProperty(null, null));

        /// <summary>
        /// Called during the chain of responsibility for a build operation.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            var dynamicBuildContext = (DynamicBuildPlanGenerationContext)(context.Existing);

            IPolicyList resolverPolicyDestination;
            var selector = context.Policies.Get<IPropertySelectorPolicy>(context.BuildKey, out resolverPolicyDestination);

            bool shouldClearOperation = false;

            foreach (var property in selector.SelectProperties(context, resolverPolicyDestination))
            {
                shouldClearOperation = true;

                var resolvedObjectParameter = Expression.Parameter(property.Property.PropertyType);

                dynamicBuildContext.AddToBuildPlan(
                    Expression.Block(
                        new[] { resolvedObjectParameter },
                        Expression.Call(
                                    null,
                                    SetCurrentOperationToResolvingPropertyValueMethod,
                                    Expression.Constant(property.Property.Name),
                                    dynamicBuildContext.ContextParameter),
                        Expression.Assign(
                                resolvedObjectParameter,
                                dynamicBuildContext.GetResolveDependencyExpression(property.Property.PropertyType, property.Resolver)),
                        Expression.Call(
                                    null,
                                    SetCurrentOperationToSettingPropertyMethod,
                                    Expression.Constant(property.Property.Name),
                                    dynamicBuildContext.ContextParameter),
                        Expression.Call(
                            Expression.Convert(dynamicBuildContext.GetExistingObjectExpression(), dynamicBuildContext.TypeToBuild),
                            GetValidatedPropertySetter(property.Property),
                            resolvedObjectParameter)));
            }

            // Clear the current operation
            if (shouldClearOperation)
            {
                dynamicBuildContext.AddToBuildPlan(dynamicBuildContext.GetClearCurrentOperationExpression());
            }
        }

        private static MethodInfo GetValidatedPropertySetter(PropertyInfo property)
        {
            //todo: Added a check for private to meet original expectations; we could consider opening this up for 
            //      private property injection.
            var setter = property.GetSetMethod(true);
            if (setter == null || setter.IsPrivate)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture,
                        Resources.PropertyNotSettable,
                        property.Name, property.DeclaringType.FullName));
            }
            return setter;
        }

        /// <summary>
        /// A helper method used by the generated IL to store the current operation in the build context.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class.")]
        public static void SetCurrentOperationToResolvingPropertyValue(string propertyName, IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            context.CurrentOperation = new ResolvingPropertyValueOperation(
                context.BuildKey.Type, propertyName);
        }

        /// <summary>
        /// A helper method used by the generated IL to store the current operation in the build context.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class.")]
        public static void SetCurrentOperationToSettingProperty(string propertyName, IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            context.CurrentOperation = new SettingPropertyOperation(
                context.BuildKey.Type, propertyName);
        }
    }
}
