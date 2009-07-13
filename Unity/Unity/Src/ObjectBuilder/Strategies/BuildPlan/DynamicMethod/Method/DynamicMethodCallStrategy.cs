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

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Practices.Unity.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that generates IL to call
    /// chosen methods (as specified by the current <see cref="IMethodSelectorPolicy"/>)
    /// as part of object build up.
    /// </summary>
    public class DynamicMethodCallStrategy : BuilderStrategy
    {
        private static readonly MethodInfo setCurrentOperationToResolvingParameter =
            StaticReflection.GetMethodInfo(() => SetCurrentOperationToResolvingParameter(null, null, null));

        private static readonly MethodInfo setCurrentOperationToInvokingMethod =
            StaticReflection.GetMethodInfo(() => SetCurrentOperationToInvokingMethod(null, null));

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        // FxCop suppression: Validation is done by Guard class
        public override void PreBuildUp(IBuilderContext context)
        {
            DynamicBuildPlanGenerationContext ilContext = (DynamicBuildPlanGenerationContext)(context.Existing);
            IMethodSelectorPolicy selector = context.Policies.Get<IMethodSelectorPolicy>(context.BuildKey);

            foreach (SelectedMethod method in selector.SelectMethods(context))
            {
                string signatureString = GetMethodSignature(method.Method);

                GuardMethodIsNotOpenGeneric(method.Method);
                GuardMethodHasNoOutParams(method.Method);
                GuardMethodHasNoRefParams(method.Method);

                ParameterInfo[] parameters = method.Method.GetParameters();

                ilContext.EmitLoadExisting();

                int i = 0;
                foreach (string key in method.GetParameterKeys())
                {
                    // Set the current operation
                    ilContext.IL.Emit(OpCodes.Ldstr, parameters[i].Name);
                    ilContext.IL.Emit(OpCodes.Ldstr, signatureString);
                    ilContext.EmitLoadContext();
                    ilContext.IL.EmitCall(OpCodes.Call, setCurrentOperationToResolvingParameter, null);

                    // Resolve the parameter
                    ilContext.EmitResolveDependency(parameters[i].ParameterType, key);
                    ++i;
                }

                // Set the current operation
                ilContext.IL.Emit(OpCodes.Ldstr, signatureString);
                ilContext.EmitLoadContext();
                ilContext.IL.EmitCall(OpCodes.Call, setCurrentOperationToInvokingMethod, null);

                // Invoke the injection method
                ilContext.IL.EmitCall(OpCodes.Callvirt, method.Method, null);
                if (method.Method.ReturnType != typeof(void))
                {
                    ilContext.IL.Emit(OpCodes.Pop);
                }
            }

            // Clear the current operation
            ilContext.EmitClearCurrentOperation();
        }

        private static void GuardMethodIsNotOpenGeneric(MethodInfo method)
        {
            if (method.IsGenericMethodDefinition)
            {
                ThrowIllegalInjectionMethod(Resources.CannotInjectOpenGenericMethod, method);
            }
        }

        private static void GuardMethodHasNoOutParams(MethodInfo method)
        {
            if (Sequence.Exists(method.GetParameters(),
                delegate(ParameterInfo param)
                {
                    return param.IsOut;
                }))
            {
                ThrowIllegalInjectionMethod(Resources.CannotInjectMethodWithOutParam, method);
            }
        }

        private static void GuardMethodHasNoRefParams(MethodInfo method)
        {
            if (Sequence.Exists(method.GetParameters(),
                delegate(ParameterInfo param)
                {
                    return param.ParameterType.IsByRef;
                }))
            {
                ThrowIllegalInjectionMethod(Resources.CannotInjectMethodWithOutParam, method);
            }
        }

        private static void ThrowIllegalInjectionMethod(string format, MethodInfo method)
        {
            throw new IllegalInjectionMethodException(
                string.Format(CultureInfo.CurrentCulture,
                    format,
                    method.DeclaringType.Name,
                    method.Name));

        }

        /// <summary>
        /// A helper method used by the generated IL to store the current operation in the build context.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class.")]
        public static void SetCurrentOperationToResolvingParameter(string parameterName, string methodSignature, IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            context.CurrentOperation = new MethodArgumentResolveOperation(
                BuildKey.GetType(context.BuildKey),
                methodSignature, parameterName);
        }

        /// <summary>
        /// A helper method used by the generated IL to store the current operation in the build context.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class.")]
        public static void SetCurrentOperationToInvokingMethod(string methodSignature, IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            context.CurrentOperation = new InvokingMethodOperation(BuildKey.GetType(context.BuildKey), methodSignature);
        }

        private static string GetMethodSignature(MethodBase method)
        {
            string methodName = method.Name;
            ParameterInfo[] parameterInfos = method.GetParameters();
            string[] parameterDescriptions = new string[parameterInfos.Length];

            for (int i = 0; i < parameterInfos.Length; ++i)
            {
                parameterDescriptions[i] = parameterInfos[i].ParameterType.FullName + " " +
                    parameterInfos[i].Name;
            }

            return string.Format(CultureInfo.CurrentCulture,
                "{0}({1})",
                methodName,
                string.Join(", ", parameterDescriptions));
        }
    }
}
