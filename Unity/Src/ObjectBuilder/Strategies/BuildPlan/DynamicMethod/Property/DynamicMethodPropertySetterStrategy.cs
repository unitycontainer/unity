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
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Practices.ObjectBuilder2.Properties;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that generates IL to resolve properties
    /// on an object being built.
    /// </summary>
    public class DynamicMethodPropertySetterStrategy : BuilderStrategy
    {
        private static MethodInfo throwOnFailedPropertyValueResolution =
            typeof(DynamicMethodPropertySetterStrategy).GetMethod(
                "ThrowOnFailedPropertyValueResolution");

        /// <summary>
        /// Called during the chain of responsibility for a build operation.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
            DynamicBuildPlanGenerationContext ilContext = (DynamicBuildPlanGenerationContext)(context.Existing);

            IPropertySelectorPolicy selector = context.Policies.Get<IPropertySelectorPolicy>(context.BuildKey);
            LocalBuilder resolving = ilContext.IL.DeclareLocal(typeof(bool));
            LocalBuilder currentPropertyName = ilContext.IL.DeclareLocal(typeof(string));

            ilContext.IL.BeginExceptionBlock();

            foreach(SelectedProperty property in selector.SelectProperties(context))
            {
                // Resolve the property value
                ilContext.IL.Emit(OpCodes.Ldstr, property.Property.Name);
                ilContext.IL.Emit(OpCodes.Stloc, currentPropertyName);
                ilContext.IL.Emit(OpCodes.Ldc_I4_1);
                ilContext.IL.Emit(OpCodes.Stloc, resolving);
                ilContext.EmitLoadExisting();
                ilContext.EmitResolveDependency(property.Property.PropertyType, property.Key);

                // Call the property setter
                ilContext.IL.Emit(OpCodes.Ldc_I4_0);
                ilContext.IL.Emit(OpCodes.Stloc, resolving);
                ilContext.IL.EmitCall(OpCodes.Callvirt, property.Property.GetSetMethod(), null);
            }

            // Catch any exceptions in the setting of the properties
            ilContext.IL.BeginCatchBlock(typeof(Exception));
            Label failedWhileResolving = ilContext.IL.DefineLabel();

            ilContext.IL.Emit(OpCodes.Ldloc, resolving);
            ilContext.IL.Emit(OpCodes.Brtrue, failedWhileResolving);
            ilContext.IL.Emit(OpCodes.Rethrow);

            ilContext.IL.MarkLabel(failedWhileResolving);
            ilContext.IL.Emit(OpCodes.Ldloc, currentPropertyName);
            ilContext.IL.EmitCall(OpCodes.Call, throwOnFailedPropertyValueResolution, null);
            ilContext.IL.EndExceptionBlock();
        }

        /// <summary>
        /// A helper method called by the generated IL to throw an exception if a property's
        /// value cannot be resolved.
        /// </summary>
        /// <param name="inner">The actual exception that caused the resolution to fail.</param>
        /// <param name="propertyName">Name of the property that didn't resolve.</param>
        public static void ThrowOnFailedPropertyValueResolution(Exception inner, string propertyName)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Resources.PropertyValueResolutionFailed,
                    propertyName),
                    inner);
        }
    }
}
