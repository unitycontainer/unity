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
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Practices.Unity.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that generates IL to resolve properties
    /// on an object being built.
    /// </summary>
    public class DynamicMethodPropertySetterStrategy : BuilderStrategy
    {
        private static readonly MethodInfo setCurrentOperationToResolvingPropertyValue =
            StaticReflection.GetMethodInfo(() => SetCurrentOperationToResolvingPropertyValue(null, null));

        private static readonly MethodInfo setCurrentOperationToSettingProperty =
            StaticReflection.GetMethodInfo(() => SetCurrentOperationToSettingProperty(null, null));

        /// <summary>
        /// Called during the chain of responsibility for a build operation.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            DynamicBuildPlanGenerationContext ilContext = (DynamicBuildPlanGenerationContext)(context.Existing);

            IPolicyList resolverPolicyDestination;
            IPropertySelectorPolicy selector = context.Policies.Get<IPropertySelectorPolicy>(context.BuildKey, out resolverPolicyDestination);

            bool shouldClearOperation = false;

            foreach (SelectedProperty property in selector.SelectProperties(context, resolverPolicyDestination))
            {
                shouldClearOperation = true;
                // Set the current operation to resolving
                ilContext.IL.Emit(OpCodes.Ldstr, property.Property.Name);
                ilContext.EmitLoadContext();
                ilContext.IL.EmitCall(OpCodes.Call, setCurrentOperationToResolvingPropertyValue, null);

                // Resolve the property value
                ilContext.EmitLoadExisting();
                ilContext.EmitResolveDependency(property.Property.PropertyType, property.Key);

                // Set the current operation to setting 
                ilContext.IL.Emit(OpCodes.Ldstr, property.Property.Name);
                ilContext.EmitLoadContext();
                ilContext.IL.EmitCall(OpCodes.Call, setCurrentOperationToSettingProperty, null);

                // Call the property setter
                ilContext.IL.EmitCall(OpCodes.Callvirt, GetValidatedPropertySetter(property.Property), null);
            }

            // Clear the current operation
            if (shouldClearOperation)
            {
                ilContext.EmitClearCurrentOperation();
            }
        }

        private static MethodInfo GetValidatedPropertySetter(PropertyInfo property)
        {
            var setter = property.GetSetMethod();
            if(setter == null)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture,
                        Resources.PropertyNotSettable,
                        property.Name, property.DeclaringType.FullName)
                    );
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
